using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace MyStaging.Interface
{
    public interface IQueryPipeLine
    {
        Type ResultType { get; set; }
        string CommandText { get; set; }
        List<DbParameter> Parameters { get; set; }
    }

    public class QueryPipeLine
    {
        public Type ResultType { get; set; }
        public string CommandText { get; set; }
        public List<DbParameter> Parameters { get; set; }
    }
}
