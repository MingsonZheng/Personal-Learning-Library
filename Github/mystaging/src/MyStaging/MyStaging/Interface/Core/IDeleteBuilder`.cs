using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;

namespace MyStaging.Interface.Core
{
    /// <summary>
    ///  数据库更新对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDeleteBuilder<T> : ISaveChanged where T : class
    {
        IDeleteBuilder<T> Where(string expression);

        IDeleteBuilder<T> Where(string formatCommad, params object[] pValue);

        IDeleteBuilder<T> Where(Expression<Func<T, bool>> predicate);

        IDeleteBuilder<T> Where<TResult>(Expression<Func<TResult, bool>> predicate);

        IDeleteBuilder<T> Where<TResult>(string alisName, Expression<Func<TResult, bool>> predicate);

        void AddParameter(string field, object value);

        void AddParameter(params DbParameter[] parameters);
    }
}
