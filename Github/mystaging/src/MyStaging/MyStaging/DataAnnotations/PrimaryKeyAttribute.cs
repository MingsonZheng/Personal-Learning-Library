using System;
using System.Collections.Generic;
using System.Text;

namespace MyStaging.DataAnnotations
{
    public class PrimaryKeyAttribute : Attribute
    {
        public bool AutoIncrement { get; set; }
    }
}
