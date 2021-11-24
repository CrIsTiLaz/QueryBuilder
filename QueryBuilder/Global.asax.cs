using Q.Core.Db.Tools.Impl.MsSqlServer.MetadataExtractor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace QueryBuilder
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            MsSqlDbSchemaExtractor m = new MsSqlDbSchemaExtractor();
            var x = m.ExtractMetadata("Server = localhost; Database = DbExplorer; Trusted_Connection = True");
        }
    }
}
