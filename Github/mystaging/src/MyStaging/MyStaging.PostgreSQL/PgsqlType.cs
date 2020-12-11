using System;
using System.Collections.Generic;
using System.Text;

namespace MyStaging.PostgreSQL
{
    /// <summary>
    /// pgsql 数据库类型转换管理对象
    /// </summary>
    public class PgsqlType
    {
        private readonly static Dictionary<string, string> csharpTypes = new Dictionary<string, string> {
                { "uuid", "Guid" },
                { "oid", "uint"},
                { "xid", "uint"},
                { "cid", "uint"},
                { "integer", "int"},
                { "serial", "int"},
                { "serial4", "int"},
                { "int4", "int"},
                { "oidvector","uint[]" },
                { "serial2", "short"},
                { "smallint", "short"},
                { "int2", "short"},
                { "money", "decimal"},
                { "numeric", "decimal"},
                { "real", "decimal"},
                { "float4","double"},
                { "float8","double"},
                { "int8", "long"},
                { "serial8", "long"},
                { "bigserial", "long"},
                { "name", "string"},
                { "varchar", "string"},
                { "bpchar", "string"},
                { "text", "string"},
                { "bool", "bool" },
                { "bytea", "byte[]" },
                { "bit", "byte" },
                { "timetz", "DateTimeOffset" },
                { "time", "TimeSpan"},
                { "interval", "TimeSpan"},
                { "date", "DateTime"},
                { "timestamptz", "DateTime"},
                { "timestamp", "DateTime"},
                { "json", "JsonElement"},
                { "jsonb", "JsonElement"},
                { "geometry", "object"},
                { "path", "NpgsqlPath"},
                { "line", "NpgsqlLine"},
                { "polygon", "NpgsqlPolygon"},
                { "circle", "NpgsqlCircle"},
                { "point", "NpgsqlPoint"},
                { "box", "NpgsqlBox"},
                { "lseg", "NpgsqlLSeg"},
                { "inet", "System.Net.IPAddress"},
                { "macaddr", "System.Net.NetworkInformation.PhysicalAddress"},
                { "xml", "System.Xml.Linq.XDocument"},
                { "varbit", "System.Collections.BitArray"}
        };
        private readonly static Dictionary<string, string> dbTypes = new Dictionary<string, string> {
                { "Guid","uuid" },
                { "Int16", "int2"},
                { "Int32", "int4"},
                { "Int64", "int8"},
                { "UInt16", "int2"},
                { "UInt32", "int4"},
                { "UInt64", "int8"},
                { "Decimal", "numeric"},
                { "Double","float8"},
                { "Single","float4"},
                { "Boolean", "bool" },
                { "Byte","bit" },
                { "SByte","bytea" },
                { "Char","char" },
                { "String","varchar" },
                { "DateTimeOffset","timetz" },
                { "TimeSpan","interval"},
                { "DateTime", "timestamp"},
                { "JsonElement", "jsonb"},
                { "Object", "geometry"},
                { "NpgsqlPath", "path"},
                { "NpgsqlLine", "line"},
                { "NpgsqlPolygon", "polygon"},
                { "NpgsqlCircle", "circle"},
                { "NpgsqlPoint", "point"},
                { "NpgsqlBox", "box"},
                { "NpgsqlLSeg", "lseg"},
                { "IPAddress", "inet"},
                { "PhysicalAddress", "macaddr"},
                { "XDocument", "xml"},
                { "BitArray", "varbit"}
        };
        private readonly static Dictionary<string, string> contrastTypes = new Dictionary<string, string> {
                { "uuid", "Guid" },
                { "int4", "int"},
                { "int2", "short"},
                { "real", "decimal"},
                { "int8", "long"},
                { "varchar", "string"},
                { "bool", "boolean" },
                { "bytea", "byte[]" },
                { "bit", "byte" },
                { "timetz", "DateTimeOffset" },
                { "interval", "TimeSpan"},
                { "timestamp", "DateTime"},
                { "jsonb", "JsonElement"},
                { "geometry", "object"},
                { "path", "NpgsqlPath"},
                { "line", "NpgsqlLine"},
                { "polygon", "NpgsqlPolygon"},
                { "circle", "NpgsqlCircle"},
                { "point", "NpgsqlPoint"},
                { "box", "NpgsqlBox"},
                { "lseg", "NpgsqlLSeg"},
                { "inet", "System.Net.IPAddress"},
                { "macaddr", "System.Net.NetworkInformation.PhysicalAddress"},
                { "xml", "System.Xml.Linq.XDocument"},
                { "varbit", "System.Collections.BitArray"}
        };

        public static string SwitchToCSharp(string type)
        {
            if (csharpTypes.ContainsKey(type))
                return csharpTypes[type];
            else
                return type;
        }

        public static string ContrastType(string type)
        {
            foreach (var k in contrastTypes.Keys)
            {
                if (k == type)
                {
                    return contrastTypes[k];
                }
            }
            return null;
        }

        public static string GetDbType(string csType)
        {
            foreach (var k in dbTypes.Keys)
            {
                if (k == csType)
                {
                    return dbTypes[k];
                }
            }
            return null;
        }
    }
}
