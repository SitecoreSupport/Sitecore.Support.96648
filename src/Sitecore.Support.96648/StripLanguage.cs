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
    public class StripLanguage : PreprocessRequestProcessor
    {
        /// <summary>
        /// Processes the specified arguments.
        /// </summary>
        /// <param name="args">
        /// The arguments.
        /// </param>
        public override void Process(PreprocessRequestArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (!Settings.Languages.AlwaysStripLanguage)
            {
                return;
            }
            Language language = StripLanguage.ExtractLanguage(args.Context.Request);
            if (language == null)
            {
                return;
            }
            if (!Data.Managers.LanguageManager.IsLanguageNameDefined(Factory.GetDatabase(Settings.GetSetting("LanguagesContextDatabase","master")), language.Name))
            {
                return;
            }
            Context.Language = language;
            Context.Data.FilePathLanguage = language;
            StripLanguage.RewriteUrl(args.Context, language);
            Tracer.Info(string.Format("Language changed to \"{0}\" as request url contains language embedded in the file path.", language.Name));
        }

        /// <summary>
        /// Extracts the language from the file path of the current request.
        /// </summary>
        /// <param name="request">
        /// The HTTP request.
        /// </param>
        /// <returns>
        /// The language.
        /// </returns>
        private static Language ExtractLanguage(HttpRequest request)
        {
            Assert.ArgumentNotNull(request, "request");
            string text = WebUtil.ExtractLanguageName(request.FilePath);
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            Language result;
            if (!Language.TryParse(text, out result))
            {
                return null;
            }
            return result;
        }

        /// <summary>
        /// Rewrites the URL after removing the language prefix.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="embeddedLanguage">
        /// The embedded language.
        /// </param>
        private static void RewriteUrl(HttpContext context, Language embeddedLanguage)
        {
            Assert.ArgumentNotNull(context, "context");
            Assert.ArgumentNotNull(embeddedLanguage, "embeddedLanguage");
            HttpRequest request = context.Request;
            string text = request.FilePath.Substring(embeddedLanguage.Name.Length + 1);
            if (!string.IsNullOrEmpty(text) && text.StartsWith(".", StringComparison.InvariantCulture))
            {
                text = string.Empty;
            }
            if (string.IsNullOrEmpty(text))
            {
                text = "/";
            }
            if (!StripLanguage.UseRedirect(text))
            {
                context.RewritePath(text, request.PathInfo, StringUtil.RemovePrefix('?', request.Url.Query));
                return;
            }
            UrlString urlString = new UrlString(text + request.Url.Query);
            urlString["sc_lang"] = embeddedLanguage.Name;
            context.Response.Redirect(urlString.ToString(), true);
        }

        /// <summary>
        /// The use rewrite.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The use rewrite.</returns>
        private static bool UseRedirect(string filePath)
        {
            Assert.IsNotNullOrEmpty(filePath, "filePath");
            return Settings.RedirectUrlPrefixes.Any((string path) => filePath.StartsWith(path, StringComparison.InvariantCulture));
        }
    }
}
