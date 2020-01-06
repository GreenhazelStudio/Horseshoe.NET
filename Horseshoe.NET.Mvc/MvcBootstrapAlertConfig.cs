using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.WebPages;

using RazorGenerator.Mvc;

namespace Horseshoe.NET.Mvc
{
    public static class MvcBootstrapAlertConfig
    {
        // ref https://www.abstractmethod.co.uk/blog/2017/10/using-razorgenerator-to-share-mvc-views-across-assemblies/
        public static void Register()
        {
            var engine = new PrecompiledMvcEngine(typeof(MvcBootstrapAlertConfig).Assembly);
            ViewEngines.Engines.Insert(0, engine);
            VirtualPathFactoryManager.RegisterVirtualPathFactory(engine);
        }
    }
}
