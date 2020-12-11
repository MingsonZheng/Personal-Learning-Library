using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace MyStaging.Metadata
{
    public class ProjectConfig
    {
        public string OutputDir { get; set; }
        public string ContextName { get; set; }
        public string ConnectionString { get; set; }
        public string Provider { get; set; }
        public GeneralInfo Mode { get; set; }
        public Assembly ProviderAssembly { get; set; }
    }
}
