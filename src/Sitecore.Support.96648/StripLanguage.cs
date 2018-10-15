using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Pipelines.PreprocessRequest;
using Sitecore.Text;
using Sitecore.Web;
using System;
using System.Linq;
using System.Web;

namespace Sitecore.Support.Pipelines.PreprocessRequest
{
    /// <summary>
    /// The strip language.
    /// </summary>
    public class StripLanguage : Sitecore.Pipelines.PreprocessRequest.StripLanguage
    {

        protected override bool IsValidForStrippingFromUrl(Language language, PreprocessRequestArgs args)
        {
            var db = Factory.GetDatabase(Settings.GetSetting("LanguagesContextDatabase", "master"));
            return base.IsValidForStrippingFromUrl(language, args) && Data.Managers.LanguageManager.IsLanguageNameDefined(db, language.Name);
        }
    }
}
