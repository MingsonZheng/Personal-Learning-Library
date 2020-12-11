using System;
using System.Collections.Generic;
using System.Text;

namespace MyStaging.Interface.Core
{
    /// <summary>
    ///  数据库更新对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IInsertBuilder<T> : ISaveChanged where T : class
    {
        T Add(T model);

        /// <summary>
        ///  调用该方法必须在最后调用 SaveChange()，否则不会将数据保存到数据库
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        IInsertBuilder<T> AddRange(List<T> items);
    }
}
