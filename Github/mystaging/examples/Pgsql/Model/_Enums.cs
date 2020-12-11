using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pgsql.Model
{
    public enum et_data_state
    {
        正常,
        删除,
    }
    public enum et_role
    {
        管理员,
        普通成员,
        群主,
    }
}
