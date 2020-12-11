using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace MyStaging.Interface
{
    public interface IStagingConnection
    {
        /// <summary>
        ///  
        /// </summary>
        /// <param name="name"></param>
        /// <param name="readOnly"></param>
        /// <returns></returns>
        DbConnection GetConnection(string name, bool readOnly);

        /// <summary>
        /// 刷新数据库连接
        /// </summary>
        /// <param name="name"></param>
        /// <param name="master"></param>
        /// <param name="slaves"></param>
        void Refresh(string name, string master, params string[] slaves);
    }
}
