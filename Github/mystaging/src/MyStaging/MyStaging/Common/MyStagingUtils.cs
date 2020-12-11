using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using MyStaging.Metadata;

namespace MyStaging.Common
{
    public class MyStagingUtils
    {
        public static Dictionary<ProviderType, string> SQLSeparator = new Dictionary<ProviderType, string>
        {
            {ProviderType.MySql,"`" },
            {ProviderType.PostgreSQL,"\"" },
        };
        public static List<PropertyInfo> GetDbFields(Type type)
        {
            var properties = new List<PropertyInfo>();
            var pis = type.GetProperties();
            for (int j = 0; j < pis.Length; j++)
            {
                PropertyInfo pi = pis[j];
                var attr = pi.GetCustomAttribute(typeof(NotMappedAttribute));
                if (attr != null) continue;
                properties.Add(pi);
            }

            return properties;
        }

        /// <summary>
        ///  根据传入的实体对象获得数据库架构级表的映射名称
        /// </summary>
        /// <returns></returns>
        public static string GetMapping(Type t, ProviderType providerType)
        {
            TypeInfo type = t.GetTypeInfo();
            string tableName;
            if (type.GetCustomAttribute(typeof(TableAttribute)) is TableAttribute table)
            {
                tableName = GetTableName(table.Schema, table.Name, providerType);
            }
            else
                throw new NotSupportedException("在表连接实体上找不到特性 TableAttribute ，请确认数据库实体模型");

            return tableName;
        }

        public static string GetTableName(TableInfo table, ProviderType providerType) => GetTableName(table.Schema, table.Name, providerType);

        private static string GetTableName(string schema, string name, ProviderType providerType)
        {
            var separator = SQLSeparator[providerType];
            if (string.IsNullOrEmpty(schema))
                return $"{separator}{name}{separator}";
            else
                return $"{separator}{schema}{separator}.{separator}{name}{separator}";
        }

        /// <summary>
        ///  复制两个对象的属性值
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="targetObj">待赋值的目标对象</param>
        /// <param name="sourceObj">复制的源对象</param>
        /// <param name="flags">指定属性搜索范围</param>
        public static void CopyProperty<T>(T targetObj, T sourceObj, BindingFlags flags = BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Default | BindingFlags.Public)
        {
            PropertyInfo[] properties = sourceObj.GetType().GetProperties(flags);

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo pi = properties[i];
                if (pi.CanWrite)
                    pi.SetValue(targetObj, pi.GetValue(sourceObj, null), null);
            }
        }

        /// <summary>
        ///  获取表达式成员名称
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static string GetMemberName<TSource, TResult>(Expression<Func<TSource, TResult>> selector)
        {
            MemberExpression exp;
            if (selector.Body.NodeType == ExpressionType.Convert)
            {
                exp = (MemberExpression)((UnaryExpression)selector.Body).Operand;
            }
            else
                exp = (MemberExpression)selector.Body;

            return exp.Member.Name;
        }

        /// <summary>
        ///  将首字母转大写
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToUpperPascal(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            string _first = text.Substring(0, 1).ToUpper();
            string _value = text.Substring(1);

            return $"{_first}{_value}";
        }

        /// <summary>
        ///  将首字母转小写
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ToLowerPascal(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            string _first = text.Substring(0, 1).ToLower();
            string _value = text.Substring(1);

            return $"{_first}{_value}";
        }
    }
}
