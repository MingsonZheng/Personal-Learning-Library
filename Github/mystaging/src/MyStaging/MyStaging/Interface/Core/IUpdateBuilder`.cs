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
    public interface IUpdateBuilder<T> where T : class
    {
        IUpdateBuilder<T> Where(string expression);

        IUpdateBuilder<T> Where(string formatCommad, params object[] pValue);

        IUpdateBuilder<T> Where(Expression<Func<T, bool>> predicate);

        IUpdateBuilder<T> Where<TResult>(Expression<Func<TResult, bool>> predicate);

        IUpdateBuilder<T> Where<TResult>(string alisName, Expression<Func<TResult, bool>> predicate);

        void AddParameter(string field, object value);

        void AddParameter(params DbParameter[] parameters);

        IUpdateBuilder<T> SetValue<TResult>(Expression<Func<T, TResult>> selector, object value);

        IUpdateBuilder<T> SetValue(string field, object value);

        IUpdateBuilder<T> SetIncrement(string field, decimal value);

        IUpdateBuilder<T> SetArrayAppend(string field, object value);

        IUpdateBuilder<T> SetArrayRemove(string field, object value);

        T SaveChange();

        string ToSQL();
    }
}
