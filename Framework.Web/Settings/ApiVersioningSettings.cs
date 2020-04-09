using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Web.Settings
{
    public class ApiVersioningSettings
    {
        public bool AssumeDefaultVersionWhenUnspecified { get; set; }
        public int DefaultApiVersion { get; set; }
    }
}
