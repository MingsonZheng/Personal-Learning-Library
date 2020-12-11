using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;
using MyStaging.Metadata;

namespace MyStaging.Interface.Core
{
    public interface ISelectBuilder<T> where T : class
    {
        ISelectBuilder<T> Page(int page, int size);

        ISelectBuilder<T> GroupBy(string groupByText);

        ISelectBuilder<T> OrderBy(string sortText);

        ISelectBuilder<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector);

        ISelectBuilder<T> OrderBy<TSource, TKey>(Expression<Func<TSource, TKey>> keySelector);

        ISelectBuilder<T> OrderByDescing<TKey>(Expression<Func<T, TKey>> keySelector);

        ISelectBuilder<T> OrderByDescing<TSource, TKey>(Expression<Func<TSource, TKey>> keySelector);

        ISelectBuilder<T> Having(string havingText);

        long Count();

        TResult Max<TResult>(string field);

        TResult Max<TResult>(Expression<Func<T, TResult>> selector);

        TResult Max<TSource, TResult>(Expression<Func<TSource, TResult>> selector);

        TResult Min<TResult>(string field);

        TResult Min<TResult>(Expression<Func<T, TResult>> selector);

        TResult Min<TSource, TResult>(Expression<Func<TSource, TResult>> selector);

        TResult Sum<TResult>(string field);

        TResult Sum<TResult>(Expression<Func<T, TResult>> selector);

        TResult Sum<TSource, TResult>(Expression<Func<TSource, TResult>> selector);

        TResult Avg<TResult>(string field);

        TResult Avg<TResult>(Expression<Func<T, TResult>> selector);

        TResult Avg<TSource, TResult>(Expression<Func<TSource, TResult>> selector);

        TResult ToScalar<TResult>(string field);

        TResult ToOne<TResult>(params string[] fields);

        TResult ToOne<TResult>(bool cacheing, params string[] fields);

        T ToOne(params string[] fields);

        List<T> ToList(params string[] fields);

        List<TResult> ToList<TResult>(params string[] fields);

        List<TResult> ExecuteReader<TResult>(string cmdText);

        ISelectBuilder<T> Where(string expression);

        ISelectBuilder<T> Where(string formatCommad, params object[] pValue);

        ISelectBuilder<T> Where(Expression<Func<T, bool>> predicate);

        ISelectBuilder<T> Where<TResult>(Expression<Func<TResult, bool>> predicate);

        ISelectBuilder<T> Where<TResult>(string alisName, Expression<Func<TResult, bool>> predicate);

        void AddParameter(string field, object value);

        void AddParameter(params DbParameter[] parameters);

        #region 连接查询

        ISelectBuilder<T> Union<TModel>(string alisName, UnionType unionType, Expression<Func<T, TModel, bool>> predicate);

        ISelectBuilder<T> Union<TModel1, TModel2>(string alisName, string unionAlisName, UnionType unionType, Expression<Func<TModel1, TModel2, bool>> predicate);

        ISelectBuilder<T> InnerJoin<TModel>(string alisName, Expression<Func<T, TModel, bool>> predicate);

        ISelectBuilder<T> InnerJoin<TModel1, TModel2>(string alisName, string unionAlisName, Expression<Func<TModel1, TModel2, bool>> predicate);

        ISelectBuilder<T> LeftJoin<TModel>(string alisName, Expression<Func<T, TModel, bool>> predicate);

        ISelectBuilder<T> LeftJoin<TModel1, TModel2>(string alisName, string unionAlisName, Expression<Func<TModel1, TModel2, bool>> predicate);

        ISelectBuilder<T> LeftOuterJoin<TModel>(string alisName, Expression<Func<T, TModel, bool>> predicate);

        ISelectBuilder<T> LeftOuterJoin<TModel1, TModel2>(string alisName, string unionAlisName, Expression<Func<TModel1, TModel2, bool>> predicate);

        ISelectBuilder<T> RightJoin<TModel>(string alisName, Expression<Func<T, TModel, bool>> predicate);

        ISelectBuilder<T> RightJoin<TModel1, TModel2>(string alisName, string unionAlisName, Expression<Func<TModel1, TModel2, bool>> predicate);

        ISelectBuilder<T> RightOuterJoin<TModel>(string alisName, Expression<Func<T, TModel, bool>> predicate);

        ISelectBuilder<T> RightOuterJoin<TModel1, TModel2>(string alisName, string unionAlisName, Expression<Func<TModel1, TModel2, bool>> predicate);

        #endregion

        int ExecuteNonQuery(string cmdText);

        int ExecuteNonQuery(string cmdText, DbParameter[] parameters);

        ISelectBuilder<T> ByMaster();

        object ExecuteScalarSlave(CommandType commandType, string commandText, params DbParameter[] commandParameters);

        void ExecuteDataReader(Action<DbDataReader> action, CommandType commandType, string commandText, params DbParameter[] parameters);

        void ExecuteDataReaderSlave(Action<DbDataReader> action, CommandType commandType, string commandText, params DbParameter[] parameters);

        List<List<dynamic>> ExecutePipeLine(bool master, params IQueryPipeLine[] contexts);

        void ExcutePipeResult(IQueryPipeLine[] contexts, DbDataReader dr, int pipeLine, List<List<dynamic>> result);
    }
}
