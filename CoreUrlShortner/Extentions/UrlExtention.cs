using System;

namespace CoreUrlShortner.Extentions
{
    public static class UrlExtention
    {
        public static bool CheckURLValid(this string url) => Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) 
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps) && Uri.IsWellFormedUriString(url, UriKind.Absolute);

    }
}
