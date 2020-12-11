using System;
using System.Collections.Generic;
using System.Text;

namespace MyStaging.Metadata
{
    public class DbFieldInfo
    {
        public int Oid { get; set; }
        public string Name { get; set; }
        public long Length { get; set; }
        public int Numeric_scale { get; set; }
        public string Comment { get; set; }
        public string CsType { get; set; }
        public string RelType { get; set; }
        public string DbType { get; set; }
        public string DbTypeFull { get; set; }
        public bool PrimaryKey { get; set; }
        public bool IsArray { get; set; }
        public bool NotNull { get; set; }
        public bool AutoIncrement { get; set; }
        public string ColumnDefault { get; set; }
    }
}
