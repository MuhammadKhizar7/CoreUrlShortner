using System;

namespace CoreUrlShortner.Extentions;

public static class UrlExtention
{
    static readonly HttpClient client = new HttpClient();
    public static bool CheckURLValid(this string url) => Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
        && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps) && Uri.IsWellFormedUriString(url, UriKind.Absolute);

    public static async Task<bool> CheckUrlExitAsync(this string url)
    {
        try
        {
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {

            return false;
        }
    }

}
