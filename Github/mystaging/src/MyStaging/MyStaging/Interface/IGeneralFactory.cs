using System;
using System.Collections.Generic;
using System.Text;
using MyStaging.Metadata;

namespace MyStaging.Interface
{
    public interface IGeneralFactory
    {
        void DbFirst(ProjectConfig config);
        void CodeFirst(ProjectConfig config);
    }
}
