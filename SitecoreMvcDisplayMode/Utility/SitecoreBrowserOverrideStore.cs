using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.WebPages;

namespace SitecoreMvcDisplayMode.Utility
{
    public class SitecoreBrowserOverrideStore : BrowserOverrideStore
    {
        private static readonly string _defaultGuid = ConfigurationManager.AppSettings["Sitecore.Device.DefaultGuid"];
        private static readonly string _appleGuid = ConfigurationManager.AppSettings["Sitecore.Device.AppleGuid"];
        private static readonly string _androidGuid = ConfigurationManager.AppSettings["Sitecore.Device.AndroidGuid"];
        private static readonly string _desktopGuid = ConfigurationManager.AppSettings["Sitecore.Device.DesktopGuid"];
        private static readonly string _mobileGuid = ConfigurationManager.AppSettings["Sitecore.Device.MobileGuid"];

        private const string AppleUserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A403 Safari/8536.25";
        private const string AndroidUserAgent = "Mozilla/5.0 (Linux; U; Android 4.0.1; ja-jp; Galaxy Nexus Build/ITL41D) AppleWebKit/534.30 (KHTML, like Gecko) Version/4.0 Mobile Safari/534.30";
        private const string DesktopUserAgent = "Mozilla/4.0 (compatible; MSIE 6.1; Windows XP)";
        private const string MobileUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows CE; IEMobile 8.12; MSIEMobile 6.0)";

        private static readonly string _cookieName = ConfigurationManager.AppSettings["Sitecore.Device.CookieName"];

        public override string GetOverriddenUserAgent(HttpContextBase httpContext)
        {
            var guid = GetGuidFromCookie(httpContext);

            if (guid == _appleGuid)
            {
                return AppleUserAgent;
            }
            if (guid == _androidGuid)
            {
                return AndroidUserAgent;
            }
            if (guid == _mobileGuid)
            {
                return MobileUserAgent;
            }
            if (guid == _desktopGuid)
            {
                return DesktopUserAgent;
            }

            return null;
        }

        public override void SetOverriddenUserAgent(HttpContextBase httpContext, string userAgent)
        {
            var guid = GetGuidFromUserAgent(userAgent);

            var browserOverrideCookie = new HttpCookie(_cookieName, guid);

            if (userAgent == null)
            {
                browserOverrideCookie.Expires = DateTime.Now.AddDays(-1);
            }

            httpContext.Response.Cookies.Remove(_cookieName);
            httpContext.Response.Cookies.Add(browserOverrideCookie);
        }

        private static string GetGuidFromCookie(HttpContextBase httpContext)
        {
            var responseCookies = httpContext.Response.Cookies;

            var cookieNames = responseCookies.AllKeys;

            if (cookieNames.Any(p => string.Equals(p, _cookieName, StringComparison.OrdinalIgnoreCase)))
            {
                var currentOverriddenBrowserCookie = responseCookies[_cookieName];

                if (currentOverriddenBrowserCookie != null && currentOverriddenBrowserCookie.Value != null)
                {
                    return currentOverriddenBrowserCookie.Value;
                }

                return null;
            }

            var requestOverrideCookie = httpContext.Request.Cookies[_cookieName];

            if (requestOverrideCookie != null)
            {
                return requestOverrideCookie.Value;
            }

            return null;
        }

        private static string GetGuidFromUserAgent(string userAgent)
        {
            if (userAgent == null)
            {
                return _defaultGuid;
            }

            if (userAgent.Contains("iPhone"))
            {
                return _appleGuid;
            }

            if (userAgent.Contains("Android") && userAgent.Contains("Mobile"))
            {
                return _androidGuid;
            }

            return _defaultGuid;
        }
    }
}