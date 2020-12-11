using System;
using System.Collections.Generic;
using System.Text;

namespace MyStaging.Metadata
{
    public class ConstraintInfo
    {
        public string Field { get; set; }
        public string Name { get; set; }
        public ConstraintType Type { get; set; }
    }
}
