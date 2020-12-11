using System;
using System.Collections.Generic;
using System.Text;

namespace MyStaging.Metadata
{
    public class TableInfo
    {
        public string Schema { get; set; }
        public string Name { get; set; }
        public TableType Type { get; set; }
        public Type EntityType { get; set; }
        public List<DbFieldInfo> Fields { get; set; } = new List<DbFieldInfo>();
        public List<ConstraintInfo> Constraints { get; set; } = new List<ConstraintInfo>();
    }
}
