using System;
using System.Collections.Generic;
using System.Text;

namespace MyStaging.Metadata
{
    /// <summary>
    ///  Union query type
    /// </summary>
    public enum UnionType
    {
        /// <summary>
        ///  Inner join
        /// </summary>
        INNER_JOIN,
        /// <summary>
        ///  Left join
        /// </summary>
        LEFT_JOIN,
        /// <summary>
        ///  Right join
        /// </summary>
        RIGHT_JOIN,
        /// <summary>
        ///  Left outer join
        /// </summary>
        LEFT_OUTER_JOIN,
        /// <summary>
        ///  Right outer join
        /// </summary>
        RIGHT_OUTER_JOIN
    }
}
