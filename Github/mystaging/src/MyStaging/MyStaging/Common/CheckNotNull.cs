using System;
using System.Collections.Generic;
using System.Text;

namespace MyStaging.Common
{
    public class CheckNotNull
    {
        public static void NotNull<T>(T model, string parameterName)
        {
            if (model == null)
                throw new ArgumentNullException(parameterName);
        }

        public static void NotEmpty<T>(IReadOnlyList<T> value, string parameterName)
        {
            NotNull(value, parameterName);

            if (value.Count == 0)
            {
                throw new ArgumentException(parameterName);
            }
        }

        public static void NotEmpty(string value, string parameterName)
        {
            if (value is null)
            {
                throw new ArgumentNullException(parameterName);
            }
            else if (value.Trim().Length == 0)
            {
                throw new ArgumentException(parameterName);
            }
        }
    }
}
