using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MyStaging.Core
{
    /// <summary>
    ///  数据库语句执行对象，抽象类
    /// </summary>
    public class SQLExecute
    {
        private readonly static Dictionary<DbType, byte> dbTypes = new Dictionary<DbType, byte>
        {
            { DbType.Int16,0},
            { DbType.Int32,0},
            { DbType.Int64,0},
            { DbType.UInt16,0},
            { DbType.UInt32,0},
            { DbType.UInt64,0},
            { DbType.Decimal,0},
            { DbType.Double,0},
            { DbType.Boolean,0},
            { DbType.VarNumeric,0},
            { DbType.Currency,0},
            { DbType.Byte,0},
            { DbType.Single,0}
        };

        #region Identity
        public DbConnection Connection { get; set; }
        public DbTransaction Transaction { get; set; }
        /// <summary>
        ///  日志输出对象
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        ///  默认构造函数
        /// </summary>
        public SQLExecute() { }

        /// <summary>
        ///  构造函数
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="transaction">事务</param>
        public SQLExecute(DbConnection connection, DbTransaction transaction) : this(null, connection, transaction)
        {
            Connection = connection;
        }

        /// <summary>
        ///  构造函数
        /// </summary>
        /// <param name="logger">日志输出对象</param>
        /// <param name="connection">数据库连接</param>
        /// <param name="transaction">事务</param>
        public SQLExecute(ILogger logger, DbConnection connection, DbTransaction transaction)
        {
            Logger = logger ?? new LoggerFactory().CreateLogger<SQLExecute>();
            Connection = connection;
            this.Transaction = transaction;
        }

        /// <summary>
        ///  构造函数
        /// </summary>
        /// <param name="logger">日志输出对象</param>
        public SQLExecute(ILogger logger)
        {
            Logger = logger ?? new LoggerFactory().CreateLogger<SQLExecute>();
        }
        #endregion

        /// <summary>
        ///  将 DbParameter 附加到待执行的 DbCommand 中
        /// </summary>
        /// <param name="command">DbCommand 对象</param>
        /// <param name="commandParameters">DbParameter 数组</param>
        public void AttachParameters(DbCommand command, DbParameter[] commandParameters)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandParameters == null) return;

            foreach (DbParameter p in commandParameters)
            {
                if (p == null) continue;
                if ((p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input) && p.Value == null)
                    p.Value = DBNull.Value;

                command.Parameters.Add(p);
            }

        }

        /// <summary>
        ///  构造 DbCommand 对象，初始化连接操作
        /// </summary>
        /// <param name="commandType">CommandType 类型</param>
        /// <param name="commandText">待执行的 SQL 语句</param>
        /// <param name="parameters">DbParameter 参数列表</param>
        public DbCommand PrepareCommand(CommandType commandType, string commandText, DbParameter[] parameters)
        {
            DbCommand command;
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");
            command = Connection.CreateCommand();
            command.Transaction = this.Transaction;
            OpenConnection(command);
            command.CommandText = commandText;
            command.CommandType = commandType;

            if (parameters != null)
                AttachParameters(command, parameters);

            return command;
        }

        /// <summary>
        ///  执行查询，并返回第一行数据的第一列的值
        /// </summary>
        /// <param name="commandType">CommandType 类型</param>
        /// <param name="commandText">待执行的 SQL 语句</param>
        /// <param name="parameters">DbParameter 参数列表</param>
        /// <returns></returns>
        public object ExecuteScalar(CommandType commandType, string commandText, params DbParameter[] parameters)
        {
            object retval = null;
            DbCommand command = null;
            try
            {
                command = PrepareCommand(commandType, commandText, parameters);
                OpenConnection(command);

                retval = command.ExecuteScalar();
                if (retval is DBNull) return null;
            }
            catch (SocketException se)
            {
                ExceptionOutPut(command, se);
                throw se;
            }
            catch (Exception ex)
            {
                ExceptionOutPut(command, ex);
                throw ex;
            }
            finally
            {
                Clear(command);
            }

            return retval;
        }

        /// <summary>
        ///  执行查询，并返回第一行数据的第一列的值
        /// </summary>
        /// <param name="commandType">CommandType 类型</param>
        /// <param name="commandText">待执行的 SQL 语句</param>
        /// <param name="parameters">DbParameter 参数列表</param>
        /// <returns></returns>
        public async Task<object> ExecuteScalarAsync(CommandType commandType, string commandText, params DbParameter[] parameters)
        {
            object retval = null;
            DbCommand command = null;
            try
            {
                command = PrepareCommand(commandType, commandText, parameters);
                OpenConnection(command);

                retval = await command.ExecuteScalarAsync();
            }
            catch (SocketException se)
            {
                ExceptionOutPut(command, se);
                throw se;
            }
            catch (Exception ex)
            {
                ExceptionOutPut(command, ex);
                throw ex;
            }
            finally
            {
                Clear(command);
            }
            return retval;
        }

        /// <summary>
        ///  执行查询，并返回受影响的行数
        /// </summary>
        /// <param name="commandType">CommandType 类型</param>
        /// <param name="commandText">待执行的 SQL 语句</param>
        /// <param name="parameters">DbParameter 参数列表</param>
        /// <returns></returns>
        public int ExecuteNonQuery(CommandType commandType, string commandText, params DbParameter[] parameters)
        {
            int retval = 0;
            DbCommand command = null;
            try
            {
                command = PrepareCommand(commandType, commandText, parameters);
                OpenConnection(command);
                retval = command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ExceptionOutPut(command, ex);
                throw ex;
            }
            finally
            {
                Clear(command);
            }

            return retval;
        }

        /// <summary>
        ///  执行查询，并返回受影响的行数
        /// </summary>
        /// <param name="commandType">CommandType 类型</param>
        /// <param name="commandText">待执行的 SQL 语句</param>
        /// <param name="parameters">DbParameter 参数列表</param>
        /// <returns></returns>
        public async Task<int> ExecuteNonQueryAsync(CommandType commandType, string commandText, params DbParameter[] parameters)
        {
            int retval = 0;
            DbCommand command = null;
            try
            {
                command = PrepareCommand(commandType, commandText, parameters);
                OpenConnection(command);
                retval = await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                ExceptionOutPut(command, ex);
                throw ex;
            }
            finally
            {
                Clear(command);
            }
            return retval;
        }

        /// <summary>
        ///  执行查询，并从返回的流中读取数据，传入委托中
        /// </summary>
        /// <param name="action">处理数据的委托函数</param>
        /// <param name="commandType">CommandType 类型</param>
        /// <param name="commandText">待执行的 SQL 语句</param>
        /// <param name="parameters">DbParameter 参数列表</param>
        public void ExecuteDataReader(Action<DbDataReader> action, CommandType commandType, string commandText, params DbParameter[] parameters)
        {
            DbCommand command = null;
            try
            {
                command = PrepareCommand(commandType, commandText, parameters);
                OpenConnection(command);

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    action?.Invoke(reader);
                };
            }
            catch (Exception ex)
            {
                ExceptionOutPut(command, ex);
                throw ex;
            }
            finally
            {
                Clear(command);
            }
        }

        public DbDataReader ExecuteDataReader(CommandType commandType, string commandText, params DbParameter[] parameters)
        {
            DbCommand command = null;
            try
            {
                command = PrepareCommand(commandType, commandText, parameters);
                OpenConnection(command);
                return command.ExecuteReader();
            }
            catch (Exception ex)
            {
                ExceptionOutPut(command, ex);
                throw ex;
            }
        }

        public List<TResult> ExecuteDataReader<TResult>(CommandType commandType, string commandText, params DbParameter[] parameters)
        {
            List<TResult> list = new List<TResult>();
            DbCommand command = null;
            try
            {
                command = PrepareCommand(commandType, commandText, parameters);
                OpenConnection(command);

                var objType = typeof(TResult);
                var properties = new List<PropertyInfo>();
                var pis = objType.GetProperties();
                for (int j = 0; j < pis.Length; j++)
                {
                    if (pis[j].GetCustomAttribute(typeof(NotMappedAttribute)) == null)
                    {
                        properties.Add(pis[j]);
                    }
                }

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    TResult obj = (TResult)Activator.CreateInstance(objType);
                    foreach (var pi in properties)
                    {
                        var value = reader[pi.Name];
                        if (value != DBNull.Value)
                            pi.SetValue(obj, value);
                    }

                    list.Add(obj);
                };
            }
            catch (Exception ex)
            {
                ExceptionOutPut(command, ex);
                throw ex;
            }
            finally
            {
                Clear(command);
            }

            return list;
        }

        /// <summary>
        ///  执行查询，并从返回的流中读取数据，传入委托中
        /// </summary>
        /// <param name="action">处理数据的委托函数</param>
        /// <param name="commandType">CommandType 类型</param>
        /// <param name="commandText">待执行的 SQL 语句</param>
        /// <param name="commandParameters">DbParameter 参数列表</param>
        public async Task ExecuteDataReaderAsync(Action<DbDataReader> action, CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            DbCommand command = null;
            try
            {
                command = PrepareCommand(commandType, commandText, commandParameters);
                OpenConnection(command);

                using var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    action?.Invoke(reader);
                };
            }
            catch (Exception ex)
            {
                ExceptionOutPut(command, ex);
                throw;
            }
            finally
            {
                Clear(command);
            }
        }

        public void ExecuteDataReaderPipe(Action<DbDataReader> action, CommandType commandType, string commandText, DbParameter[] parameters)
        {
            DbCommand command = null;
            try
            {
                command = PrepareCommand(commandType, commandText, parameters);
                OpenConnection(command);
                DbDataReader reader;
                using (reader = command.ExecuteReader())
                {
                    action?.Invoke(reader);
                }
            }
            catch (Exception ex)
            {
                ExceptionOutPut(command, ex);
                throw;
            }
            finally
            {
                Clear(command);
            }
        }

        /// <summary>
        ///  输出异常信息
        /// </summary>
        /// <param name="command">DbCommand 对象</param>
        /// <param name="ex">异常信息</param>
        protected void ExceptionOutPut(DbCommand command, Exception ex)
        {
            if (command == null)
                return;

            DbParameterCollection coll = command.Parameters;
            string ps = string.Empty;
            string sql = command.CommandText;
            if (coll != null)
            {
                for (int i = 0; i < coll.Count; i++)
                {
                    var item = coll[i];
                    ps += $"{ item.ParameterName}:{item.Value},";
                }

                for (int i = 0; i < coll.Count; i++)
                {
                    var para = coll[i];
                    var isString = dbTypes.ContainsKey(para.DbType);
                    var val = string.Format("{0}{1}{0}", isString ? "'" : "", para.Value.ToString());
                    sql = sql.Replace("@" + para.ParameterName, val);
                }
            }

            ex.Data["DbConnection"] = command.Connection.ConnectionString;
            ex.Data["CommandText"] = sql;
            ex.Data["Parameters"] = ps;

            if (Logger != null)
                Logger.LogError(new EventId(111111), ex, "数据库执行出错：===== \n {0}\n{1}", sql, ps);
            else
                Console.WriteLine("数据库执行出错：===== \n {0}\n{1}", sql, ps);
        }

        /// <summary>
        ///  打开数据库连接
        /// </summary>
        /// <param name="cmd"></param>
        private void OpenConnection(DbCommand cmd)
        {
            if (cmd.Connection.State == ConnectionState.Closed)
                cmd.Connection.Open();
        }

        /// <summary>
        ///  释放连接，清理资源
        /// </summary>
        /// <param name="cmd"></param>
        private void Clear(DbCommand cmd)
        {
            if (cmd != null)
            {
                if (cmd.Transaction == null && cmd.Connection.State != ConnectionState.Closed)
                {
                    this.Connection.Close();
                }
                cmd.Parameters?.Clear();
            }
        }
    }
}
