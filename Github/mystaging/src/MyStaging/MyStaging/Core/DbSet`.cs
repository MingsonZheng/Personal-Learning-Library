using System;
using System.Collections.Generic;
using System.Text;
using MyStaging.Interface.Core;

namespace MyStaging.Core
{
    public class DbSet<T> where T : class
    {
        public ISelectBuilder<T> Select { get; set; }
        public IInsertBuilder<T> Insert { get; set; }
        public IUpdateBuilder<T> Update { get; set; }
        public IDeleteBuilder<T> Delete { get; set; }
    }
}
