using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MyStaging.Common;
using MyStaging.Metadata;

namespace MyStaging.PostgreSQL.Generals
{
    public class EntityGeneral
    {
        #region identity
        private readonly TableInfo table;
        private readonly GeneralConfig config;
        #endregion

        public EntityGeneral(GeneralConfig config, TableInfo table)
        {
            this.config = config;
            this.table = table;
        }

        public void Create()
        {
            string _classname = CreateName();
            string _fileName = $"{config.ModelPath}/{_classname}.cs";
            using StreamWriter writer = new StreamWriter(File.Create(_fileName), System.Text.Encoding.UTF8);
            writer.WriteLine("using System;");
            writer.WriteLine("using System.Linq;");
            writer.WriteLine("using System.Text.Json;");
            writer.WriteLine("using NpgsqlTypes;");
            writer.WriteLine("using System.ComponentModel.DataAnnotations;");
            writer.WriteLine("using System.ComponentModel.DataAnnotations.Schema;");
            writer.WriteLine("using MyStaging.DataAnnotations;");
            writer.WriteLine();
            writer.WriteLine($"namespace {config.ProjectName}.Model");
            writer.WriteLine("{");
            writer.WriteLine($"\t[Table(name: \"{this.table.Name}\", Schema = \"{table.Schema}\")]");
            writer.WriteLine($"\tpublic partial class {_classname}");
            writer.WriteLine("\t{");

            foreach (var fi in table.Fields)
            {
                if (!string.IsNullOrEmpty(fi.Comment))
                {
                    writer.WriteLine("\t\t/// <summary>");
                    writer.WriteLine($"\t\t/// {fi.Comment}");
                    writer.WriteLine("\t\t/// </summary>");
                }

                var autoincrement = fi.AutoIncrement ? "(AutoIncrement = true)" : "";

                if (fi.PrimaryKey)
                    writer.WriteLine($"\t\t[PrimaryKey{autoincrement}]");
                if (fi.NotNull && fi.RelType == "string" && !fi.PrimaryKey)
                    writer.WriteLine("\t\t[Required]");
                if (!string.IsNullOrEmpty(fi.DbTypeFull))
                    writer.WriteLine($"\t\t[Column(TypeName = \"{fi.DbTypeFull}\")]");

                writer.WriteLine($"\t\tpublic {fi.RelType} {fi.Name} {{ get; set; }}");
            }
            writer.WriteLine("\t}");
            writer.WriteLine("}");
            writer.Flush();
        }

        private string CreateName(string separator = "")
        {
            var tableName = MyStagingUtils.ToUpperPascal(table.Name);
            string className;
            if (table.Schema == "public")
            {
                className = tableName;
            }
            else
            {
                className = $"{MyStagingUtils.ToUpperPascal(table.Schema)}{separator}{tableName}";
            }

            return className;
        }
    }
}
