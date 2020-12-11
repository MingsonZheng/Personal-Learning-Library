using System;
using System.Collections.Generic;
using System.Text;

namespace MyStaging.Interface
{
    public interface ISaveChanged
    {
        /// <summary>
        ///  将当前更改保存到数据库
        /// </summary>
        /// <returns></returns>
        int SaveChange();

        string ToSQL();
    }
}
