using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MyStaging.Common;
using MyStaging.Core;
using MyStaging.DataAnnotations;
using MyStaging.Interface;
using MyStaging.Metadata;
using Npgsql;

namespace MyStaging.PostgreSQL.Generals
{
    public class GeneralFactory : IGeneralFactory
    {
        private ProjectConfig config;

        public void Initialize(ProjectConfig config)
        {
            this.config = config;
            Tables = new List<TableInfo>();

            #region dir

            CheckNotNull.NotEmpty(config.ContextName, nameof(config.ContextName));

            if (config.Mode == GeneralInfo.Db)
            {
                CheckNotNull.NotEmpty(config.OutputDir, nameof(config.OutputDir));
                Config = new GeneralConfig
                {
                    OutputDir = config.OutputDir,
                    ProjectName = config.ContextName,
                    ModelPath = config.OutputDir
                };

                if (!Directory.Exists(Config.ModelPath))
                    Directory.CreateDirectory(Config.ModelPath);
            }
            #endregion

            #region Schemas
            string[] filters = new string[this.Filters.Count];
            for (int i = 0; i < Filters.Count; i++)
            {
                filters[i] = $"'{Filters[i]}'";
            }

            string sql = $@"SELECT schema_name FROM information_schema.schemata WHERE SCHEMA_NAME NOT IN({string.Join(",", filters)}) ORDER BY SCHEMA_NAME; ";
            List<string> schemas = new List<string>();
            SQLContext.ExecuteDataReader(dr =>
            {
                schemas.Add(dr[0].ToString());
            }, CommandType.Text, sql);
            #endregion

            #region Tables
            foreach (var schema in schemas)
            {
                string _sqltext = $@"SELECT table_name,'table' as type FROM INFORMATION_SCHEMA.tables WHERE table_schema='{schema}' AND table_type='BASE TABLE'
UNION ALL
SELECT table_name,'view' as type FROM INFORMATION_SCHEMA.views WHERE table_schema = '{schema}'";
                SQLContext.ExecuteDataReader(dr =>
                {
                    var table = new TableInfo()
                    {
                        Schema = schema,
                        Name = dr["table_name"].ToString(),
                        Type = dr["type"].ToString() == "table" ? TableType.Table : TableType.View
                    };
                    GetFields(table);
                    Tables.Add(table);
                }, CommandType.Text, _sqltext);

            }
            #endregion
        }

        public void DbFirst(ProjectConfig config)
        {
            Initialize(config);
            GenerateMapping();

            // Generral Entity
            foreach (var table in Tables)
            {
                Console.WriteLine("[{0}]{1}.{2}", table.Type, table.Schema, table.Name);
                EntityGeneral td = new EntityGeneral(Config, table);
                td.Create();
            }
        }

        public void CodeFirst(ProjectConfig config)
        {
            Initialize(config);

            StringBuilder sb = new StringBuilder();
            List<TableInfo> tables = new List<TableInfo>();

            var fileName = config.ContextName + ".dll";
            var dir = System.IO.Directory.GetCurrentDirectory();

            var providerFile = System.IO.Directory.GetFiles(dir, fileName, SearchOption.AllDirectories).FirstOrDefault();
            if (string.IsNullOrEmpty(providerFile))
                throw new FileNotFoundException($"在 {dir} 搜索不到文件 {fileName}");

            var types = Assembly.LoadFrom(providerFile).GetTypes();
            List<TableInfo> entitys = new List<TableInfo>();
            foreach (var t in types)
            {
                var tableAttribute = t.GetCustomAttribute<TableAttribute>();
                if (tableAttribute == null)
                    continue;

                entitys.Add(new TableInfo
                {
                    Name = tableAttribute.Name,
                    Schema = tableAttribute.Schema,
                    EntityType = t
                });
            }

            foreach (var ent in entitys)
            {
                SerializeField(ent, ent.EntityType);

                var table = Tables.Where(f => f.Schema == ent.Schema && f.Name == ent.Name).FirstOrDefault();
                if (table == null) // CREATE
                    DumpTable(ent, ref sb);
                else // ALTER
                    DumpAlter(ent, table, ref sb);
            }

            // 删除实体
            foreach (var table in Tables)
            {
                if (entitys.Where(f => f.Schema == table.Schema && f.Name == table.Name).FirstOrDefault() == null)
                {
                    sb.AppendLine($"DROP TABLE {MyStagingUtils.GetTableName(table, ProviderType.PostgreSQL)};");
                }
            }

            var sql = sb.ToString();

            if (string.IsNullOrEmpty(sql))
            {
                Console.WriteLine("数据模型没有可执行的更改.");
            }
            else
            {
                Console.WriteLine("------------------SQL------------------");
                Console.WriteLine(sql);
                Console.WriteLine("------------------SQL END------------------");
                SQLContext.ExecuteNonQuery(CommandType.Text, sql);
            }
        }

        private void DumpAlter(TableInfo newTable, TableInfo oldTable, ref StringBuilder sb)
        {
            var alterSql = $"ALTER TABLE {MyStagingUtils.GetTableName(newTable, ProviderType.PostgreSQL)}";

            // 常规
            foreach (var newFi in newTable.Fields)
            {
                var oldFi = oldTable.Fields.Where(f => f.Name == newFi.Name).FirstOrDefault();
                var notNull = newFi.NotNull ? "NOT NULL" : "NULL";
                var realType = newFi.DbTypeFull ?? newFi.DbType;
                if (oldFi == null)
                {
                    sb.AppendLine($"{alterSql} ADD \"{newFi.Name}\" {realType};");
                    sb.AppendLine($"{alterSql} MODIFY \"{newFi.Name}\" {realType} {notNull};");
                }
                else
                {
                    if (oldFi.DbTypeFull != newFi.DbTypeFull)
                        sb.AppendLine($"{alterSql} ALTER \"{newFi.Name}\" TYPE {realType};");
                    if (oldFi.NotNull != newFi.NotNull)
                        sb.AppendLine($"{alterSql} MODIFY \"{newFi.Name}\" {realType} {notNull};");
                }
            }

            // 移除旧字段
            foreach (var oldFi in oldTable.Fields)
            {
                var newFi = newTable.Fields.Where(f => f.Name == oldFi.Name).FirstOrDefault();
                if (newFi == null)
                {
                    sb.AppendLine($"{alterSql} DROP COLUMN \"{oldFi.Name}\";");
                }
            }

            // 检查旧约束
            foreach (var c in oldTable.Constraints)
            {
                // PK
                var constraint = newTable.Fields.Where(f => f.Name == c.Field && f.PrimaryKey).FirstOrDefault();
                if (constraint == null)
                {
                    sb.AppendLine($"{alterSql} DROP CONSTRAINT {c.Name};");
                }

                // SEQ
                var seq = oldTable.Fields.Where(f => f.Name == c.Field && f.AutoIncrement).FirstOrDefault();
                if (seq != null)
                {
                    // 旧 increment 在新的同步中被删除
                    if (newTable.Fields.Where(f => f.Name == seq.Name && f.AutoIncrement).FirstOrDefault() == null)
                    {
                        var indexOf = seq.ColumnDefault.IndexOf("'") + 1;
                        var lastIndexOf = seq.ColumnDefault.LastIndexOf("'");
                        var seqName = seq.ColumnDefault[indexOf..lastIndexOf];

                        sb.AppendLine($"{alterSql} ALTER COLUMN {seq.Name} SET DEFAULT null;");
                        sb.AppendLine($"DROP SEQUENCE IF EXISTS {seqName};");
                    }
                }
            }

            // 检查新约束
            foreach (var fi in newTable.Fields)
            {
                if (!fi.PrimaryKey)
                    continue;

                // PK
                var constraint = oldTable.Constraints.Where(f => f.Field == fi.Name).FirstOrDefault();
                if (constraint == null)
                {
                    sb.AppendLine($"{alterSql} ADD CONSTRAINT pk_{newTable.Name} PRIMARY KEY({fi.Name});");
                }

                // SEQ
                if (fi.AutoIncrement)
                {
                    if (oldTable.Fields.Where(f => f.Name == fi.Name && f.AutoIncrement).FirstOrDefault() == null)
                    {
                        var seqName = $"{ newTable.Name }_{ fi.Name}_seq";
                        sb.AppendLine($"CREATE SEQUENCE {seqName} START WITH 1;");
                        sb.AppendLine($"ALTER TABLE \"{newTable.Schema}\".\"{newTable.Name}\" ALTER COLUMN \"{fi.Name}\" SET DEFAULT nextval('{seqName}'::regclass);");
                    }
                }
            }
        }

        private void SerializeField(TableInfo table, Type type)
        {
            var properties = MyStagingUtils.GetDbFields(type);
            foreach (var pi in properties)
            {
                var fi = new DbFieldInfo
                {
                    Name = pi.Name
                };
                var customAttributes = pi.GetCustomAttributes();
                var genericAttrs = customAttributes.Select(f => f.GetType()).ToArray();
                var pk = pi.GetCustomAttribute<PrimaryKeyAttribute>();
                fi.PrimaryKey = pk != null;
                if (fi.PrimaryKey)
                {
                    fi.AutoIncrement = pk.AutoIncrement;
                }

                if (pi.PropertyType.Name == "Nullable`1")
                {
                    fi.NotNull = false;
                    fi.CsType = pi.PropertyType.GenericTypeArguments[0].Name;
                }
                else
                {
                    fi.CsType = pi.PropertyType.Name;
                    if (pi.PropertyType == typeof(string))
                    {
                        fi.NotNull = fi.PrimaryKey || genericAttrs.Where(f => f == typeof(RequiredAttribute)).FirstOrDefault() != null;
                    }
                    else
                    {
                        fi.NotNull = pi.PropertyType.IsValueType;
                    }
                }

                var columnAttribute = customAttributes.Where(f => f.GetType() == typeof(ColumnAttribute)).FirstOrDefault();
                if (columnAttribute != null)
                {
                    var colAttribute = ((ColumnAttribute)columnAttribute);
                    fi.DbType = fi.DbTypeFull = colAttribute.TypeName;
                }
                else
                {
                    fi.DbType = PgsqlType.GetDbType(fi.CsType.Replace("[]", ""));
                    fi.DbTypeFull = GetFullDbType(fi);
                }
                fi.IsArray = fi.CsType.Contains("[]");

                table.Fields.Add(fi);
            }
        }

        private void DumpTable(TableInfo table, ref StringBuilder sb)
        {
            var tableName = MyStagingUtils.GetTableName(table, ProviderType.PostgreSQL);
            sb.AppendLine($"CREATE TABLE {tableName}");
            sb.AppendLine("(");
            int length = table.Fields.Count;
            for (int i = 0; i < length; i++)
            {
                var fi = table.Fields[i];

                sb.AppendFormat("  \"{0}\" {1}{2} {3} {4} {5}",
                    fi.Name,
                    fi.DbTypeFull ?? fi.DbType,
                    fi.IsArray ? "[]" : "",
                    fi.PrimaryKey ? "PRIMARY KEY" : "",
                    fi.PrimaryKey || fi.NotNull ? "NOT NULL" : "NULL",
                    (i + 1 == length) ? "" : ","
                    );
                sb.AppendLine();
            }
            sb.AppendLine(")");
            sb.AppendLine("WITH (OIDS=FALSE);");

            // SEQ
            foreach (var fi in table.Fields)
            {
                if (!fi.AutoIncrement) continue;

                var seqName = $"{ table.Name }_{ fi.Name}_seq";
                sb.AppendLine();
                sb.AppendLine($"--{seqName} SEQUENCE");
                sb.AppendLine($"ALTER TABLE {tableName} ALTER COLUMN {fi.Name} SET DEFAULT null;");
                sb.AppendLine($"DROP SEQUENCE IF EXISTS {seqName};");
                sb.AppendLine($"CREATE SEQUENCE {seqName} START WITH 1;");
                sb.AppendLine($"ALTER TABLE {tableName} ALTER COLUMN {fi.Name} SET DEFAULT nextval('{seqName}'::regclass);");
                sb.AppendLine("-- SEQUENCE END");
            }
        }

        private string GetFullDbType(DbFieldInfo fi)
        {
            string fullType = null;
            if (fi.Length > 0)
            {
                if (fi.Length != 255 && fi.CsType == "String")
                    fullType = $"{fi.DbType}({fi.Length})";
                else if (fi.CsType != "String" && fi.Numeric_scale > 0)
                {
                    fullType = $"{fi.DbType}({fi.Length},{fi.Numeric_scale})";
                }
            }

            return fullType;
        }

        public void GenerateMapping()
        {
            string _sqltext = @"
select a.oid,a.typname,b.nspname from pg_type a 
INNER JOIN pg_namespace b on a.typnamespace = b.oid 
where a.typtype = 'e' order by oid asc";

            List<EnumTypeInfo> enums = new List<EnumTypeInfo>();
            SQLContext.ExecuteDataReader(dr =>
            {
                enums.Add(new EnumTypeInfo()
                {
                    Oid = Convert.ToInt32(dr["oid"]),
                    TypeName = dr["typname"].ToString(),
                    NspName = dr["nspname"].ToString()
                });
            }, System.Data.CommandType.Text, _sqltext);

            if (enums.Count > 0)
            {
                string _fileName = Path.Combine(Config.ModelPath, "_Enums.cs");
                using StreamWriter writer = new StreamWriter(File.Create(_fileName), System.Text.Encoding.UTF8);
                writer.WriteLine("using System;");
                writer.WriteLine();
                writer.WriteLine($"namespace {Config.ProjectName}.Model");
                writer.WriteLine("{");

                for (int i = 0; i < enums.Count; i++)
                {
                    var item = enums[i];
                    writer.WriteLine($"\tpublic enum {item.TypeName}");
                    writer.WriteLine("\t{");
                    string sql = $"select oid,enumlabel from pg_enum WHERE enumtypid = {item.Oid} ORDER BY oid asc";
                    SQLContext.ExecuteDataReader(dr =>
                    {
                        string c = i < enums.Count ? "," : "";
                        writer.WriteLine($"\t\t{dr["enumlabel"]}{c}");
                    }, CommandType.Text, sql);
                    writer.WriteLine("\t}");
                }
                writer.WriteLine("}");
            }

            var contextName = $"{ Config.ProjectName }DbContext";
            string _startup_file = Path.Combine(Config.OutputDir, $"{contextName}.cs");
            using (StreamWriter writer = new StreamWriter(File.Create(_startup_file), System.Text.Encoding.UTF8))
            {
                writer.WriteLine($"using {Config.ProjectName}.Model;");
                writer.WriteLine("using System;");
                writer.WriteLine("using Npgsql;");
                writer.WriteLine("using MyStaging.Core;");
                writer.WriteLine("using MyStaging.Common;");
                writer.WriteLine("using MyStaging.Metadata;");
                writer.WriteLine("using System.Text.Json;");
                writer.WriteLine();
                writer.WriteLine($"namespace {Config.ProjectName}");
                writer.WriteLine("{");
                writer.WriteLine($"\tpublic partial class {contextName} : DbContext");
                writer.WriteLine("\t{");
                writer.WriteLine($"\t\tpublic {contextName}(StagingOptions options) : base(options, ProviderType.PostgreSQL)");
                writer.WriteLine("\t\t{");
                writer.WriteLine("\t\t}");
                writer.WriteLine();
                writer.WriteLine($"\t\tstatic {contextName}()");
                writer.WriteLine("\t\t{");
                writer.WriteLine("\t\t\tType[] jsonTypes = { typeof(JsonElement) };");
                writer.WriteLine("\t\t\tNpgsqlNameTranslator translator = new NpgsqlNameTranslator();");
                writer.WriteLine("\t\t\tNpgsqlConnection.GlobalTypeMapper.UseJsonNet(jsonTypes);");

                foreach (var table in Tables)
                {
                    if (table.Name == "geometry_columns")
                    {
                        writer.WriteLine($"\t\t\tNpgsqlConnection.GlobalTypeMapper.UseLegacyPostgis();");
                        break;
                    }
                }

                if (enums.Count > 0)
                {
                    writer.WriteLine();
                    foreach (var item in enums)
                    {
                        writer.WriteLine($"\t\t\tNpgsqlConnection.GlobalTypeMapper.MapEnum<{item.TypeName}>(\"{item.NspName}.{item.TypeName}\", translator);");
                    }
                }

                writer.WriteLine("\t\t}"); // InitializerMapping end
                writer.WriteLine();

                foreach (var table in Tables)
                {
                    var tableName = MyStagingUtils.ToUpperPascal(table.Name);
                    writer.WriteLine($"\t\tpublic DbSet<{tableName}> {tableName} {{ get; set; }}");
                }

                writer.WriteLine("\t}"); // class end
                writer.WriteLine("\tpublic partial class NpgsqlNameTranslator : INpgsqlNameTranslator");
                writer.WriteLine("\t{");
                writer.WriteLine("\t\tpublic string TranslateMemberName(string clrName) => clrName;");
                writer.WriteLine("\t\tpublic string TranslateTypeName(string clrTypeName) => clrTypeName;");
                writer.WriteLine("\t}");
                writer.WriteLine("}"); // namespace end
            }
        }

        private void GetFields(TableInfo table)
        {
            string _sqltext = @"SELECT a.oid
                                            ,c.attnum as num
                                            ,c.attname as field
                                            ,c.attnotnull as notnull
                                            ,d.description as comment
                                            ,(case when e.typcategory ='G' then e.typname when e.typelem = 0 then e.typname else e2.typname end) as type
                                            ,(case when e.typelem = 0 then e.typtype else e2.typtype end) as data_type
                                            ,COALESCE((
                                            case 
                                            when (case when e.typcategory ='G' then e.typname when e.typelem = 0 then e.typname else e2.typname end) in ('numeric','int2','int4','int8','float4','float8') then f.numeric_precision
                                            when (case when e.typcategory ='G' then e.typname when e.typelem = 0 then e.typname else e2.typname end) in ('timestamp','timestamptz','interval','time','date','timetz') then f.datetime_precision
                                            when f.character_maximum_length is null then 0
                                            else f.character_maximum_length 
                                            end
                                            ),0) as length
                                            ,COALESCE((
                                            case 
                                            when (case when e.typcategory ='G' then e.typname when e.typelem = 0 then e.typname else e2.typname end) in ('numeric') then f.numeric_scale
                                            else 0
                                            end
                                            ),0) numeric_scale
                                            ,e.typcategory
                                            ,f.udt_schema
                                            ,f.column_default
                                                                            from  pg_class a 
                                                                            inner join pg_namespace b on a.relnamespace=b.oid
                                                                            inner join pg_attribute c on attrelid = a.oid
                                                                            LEFT OUTER JOIN pg_description d ON c.attrelid = d.objoid AND c.attnum = d.objsubid and c.attnum > 0
                                                                            inner join pg_type e on e.oid=c.atttypid
                                                                            left join pg_type e2 on e2.oid=e.typelem
                                                                            inner join information_schema.columns f on f.table_schema = b.nspname and f.table_name=a.relname and column_name = c.attname
                                                                            WHERE b.nspname='{0}' and a.relname='{1}';";

            _sqltext = string.Format(_sqltext, table.Schema, table.Name);
            SQLContext.ExecuteDataReader(dr =>
            {
                DbFieldInfo fi = new DbFieldInfo
                {
                    Oid = Convert.ToInt32(dr["oid"]),
                    Name = dr["field"].ToString(),
                    Length = Convert.ToInt32(dr["length"].ToString()),
                    NotNull = Convert.ToBoolean(dr["notnull"]),
                    Comment = dr["comment"].ToString(),
                    Numeric_scale = Convert.ToInt32(dr["numeric_scale"].ToString()),
                    ColumnDefault = dr["column_default"].ToString(),
                };

                var udt_schema = dr["udt_schema"].ToString();
                var typcategory = dr["typcategory"].ToString();
                var dbtype = dr["type"].ToString();
                fi.DbType = typcategory == "E" ? udt_schema + "." + dbtype : dbtype;
                fi.IsArray = typcategory == "A";
                fi.CsType = PgsqlType.SwitchToCSharp(dbtype);
                fi.AutoIncrement = fi.ColumnDefault != null && fi.ColumnDefault.StartsWith("nextval('") && fi.ColumnDefault.EndsWith("'::regclass)");

                string _notnull = "";
                if (
                fi.CsType != "string"
                && fi.CsType != "byte[]"
                && fi.CsType != "JsonElement"
                && !fi.IsArray
                && fi.CsType != "System.Net.IPAddress"
                && fi.CsType != "System.Net.NetworkInformation.PhysicalAddress"
                && fi.CsType != "System.Xml.Linq.XDocument"
                && fi.CsType != "System.Collections.BitArray"
                && fi.CsType != "object"
                )
                    _notnull = fi.NotNull ? "" : "?";

                string _array = fi.IsArray ? "[]" : "";
                fi.RelType = $"{fi.CsType}{_notnull}{_array}";

                if (fi.RelType == "string" && (fi.Length != 0 && fi.Length != 255))
                    fi.DbTypeFull = $"{fi.DbType}({fi.Length})";
                else if (fi.Numeric_scale > 0)
                {
                    fi.DbTypeFull = ($"{fi.DbType}({fi.Length},{fi.Numeric_scale})");
                }
                else if (PgsqlType.ContrastType(fi.DbType) == null)
                {
                    fi.DbTypeFull = fi.DbType;
                }

                table.Fields.Add(fi);
            }, CommandType.Text, _sqltext);

            if (table.Type == TableType.Table)
                GetPrimarykey(table);
        }

        private void GetPrimarykey(TableInfo table)
        {
            string _sqltext = $@"select a.constraint_name,b.column_name 
                                              from information_schema.table_constraints a
                                              inner join information_schema.constraint_column_usage b on a.constraint_name=b.constraint_name
                                              where a.table_schema || '.' || a.table_name='{table.Schema}.{table.Name}' and a.constraint_type='PRIMARY KEY'";

            SQLContext.ExecuteDataReader(dr =>
            {
                var constaint = new ConstraintInfo
                {
                    Field = dr["column_name"].ToString(),
                    Name = dr["constraint_name"].ToString(),
                    Type = ConstraintType.PK
                };

                table.Constraints.Add(constaint);
                var tDbFieldInfo = table.Fields.FirstOrDefault(f => f.Name == constaint.Field);
                if (tDbFieldInfo != null)
                    tDbFieldInfo.PrimaryKey = true;

            }, CommandType.Text, _sqltext);
        }

        #region Properties
        public List<string> Filters { get; set; } = new List<string>() {
               "geometry_columns",
               "raster_columns",
               "spatial_ref_sys",
               "raster_overviews",
               "us_gaz",
               "topology",
               "zip_lookup_all",
               "pg_toast",
               "pg_temp_1",
               "pg_toast_temp_1",
               "pg_catalog",
               "information_schema",
               "tiger",
               "tiger_data"
        };
        public GeneralConfig Config { get; set; }
        public List<TableInfo> Tables { get; set; }
        private SQLExecute SQLContext => new SQLExecute(new NpgsqlConnection(config.ConnectionString), null);
        #endregion
    }
}
