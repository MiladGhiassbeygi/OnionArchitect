using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Web.Settings
{
    public class SwaggerSettings
    {
        public SwaggerInfo Info { get; set; }
        public SwaggerXmlfile XmlFile { get; set; }
    }

    public class SwaggerInfo
    {
        public string Version { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public SwaggerContact Contact { get; set; }
    }

    public class SwaggerContact
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
    }

    public class SwaggerXmlfile
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
