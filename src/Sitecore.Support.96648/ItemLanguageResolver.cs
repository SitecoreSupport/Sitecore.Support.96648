using Sitecore.Globalization;
using Sitecore.Pipelines.HttpRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Support.Pipelines.HttpRequest
{
    public class ItemLanguageResolver : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            string[] sites= new string[]{ "shell","login","admin","service", "system_layouts", "modules_shell", "modules_website", "scheduler", "system", "publisher" };
            if (sites.Contains(Context.Site.Name))
            {
                return;
            }

            if (Context.Item != null)
            {
                if (Context.Item.Versions.GetVersions(false).Count() == 0)
                {
                    Context.Language = Language.Parse(Context.Site.Language);
                }
            }
            else
            {
                Context.Language = Language.Parse(Context.Site.Language);
            }
        }
    }
}
