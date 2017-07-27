using Sitecore.Common;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Pipelines.HttpRequest;
using Sitecore.Sites;
using System.Linq;

namespace Sitecore.Support.Pipelines.HttpRequest
{
    public class ItemLanguageResolver : ItemResolver
    {
        public override void Process(HttpRequestArgs args)
        {

            

            string[] sites= new string[]{ "shell","login","admin","service", "system_layouts", "modules_shell", "modules_website", "scheduler", "system", "publisher" };
            if (sites.Contains(Context.Site.Name))
            {
                base.Process(args);
                return;
            }

            using(new EnforceVersionPresenceDisabler())
            {
                base.Process(args);
            }

            if (Context.Item != null)
            {
                if (Context.Item.Versions.GetVersions(false).Count() == 0)
                {
                    Context.Language = Language.Parse(Context.Site.Language);
                    if (!IsItemEnforceVersionPresenceEnabled(Context.Item))
                    {
                        Context.Item = Context.Database.GetItem(Context.Item.ID);
                    }
                    else
                    {
                        Context.Item = null;
                    }                  
                   
                }
            }
            else
            {
                Context.Language = Language.Parse(Context.Site.Language);
            }
        }

        protected virtual bool IsEnforceVersionPresenceEnabled()
        {
            SiteContext site= Context.Site;
            return !Switcher<bool, EnforceVersionPresenceDisabler>.CurrentValue && site != null && site.SiteInfo.EnforceVersionPresence;
        }

        protected virtual bool IsItemEnforceVersionPresenceEnabled(Item item)
        {
            bool result;
            using (new EnforceVersionPresenceDisabler())
            {
                result = (item != null && item.RuntimeSettings.TemporaryVersion && !item.Name.StartsWith("__") && item[FieldIDs.EnforceVersionPresence] == "1");
            }
            return result;
        }

    }
}
