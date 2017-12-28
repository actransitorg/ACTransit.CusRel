using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

using ACTransit.Framework.Web.Infrastructure;

namespace ACTransit.Framework.Web.Auth
{
    public static class OAuth
    {
        private static readonly WebLogger WebLogger;

        static OAuth()
        {
            WebLogger = new WebLogger(typeof(OAuth));
        }

        public static async Task<AuthToken> GetAuthTokenAsync(string oAuthServerEndPoint, string apiKey, string clientSecret)
        {
            var authToken = new AuthToken();

            //Connect to Facebook servers
            using (var client = new HttpClient())
            {
                var authData = new[]
                {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("client_id", apiKey),
                    new KeyValuePair<string, string>("client_secret", clientSecret)
                };

                //Retrieve authentication token from Twitter oauth2 server
                using (var content = new FormUrlEncodedContent(authData))
                {
                    content.Headers.Clear();
                    content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    try
                    {
                        authToken = await client.PostAsync(oAuthServerEndPoint, content)
                            .Result.Content.ReadAsAsync<AuthToken>();
                    }
                    catch (HttpRequestException ex)
                    {
                        WebLogger.Error(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error in GetAuthTokenAsync", ex);
                    }
                }
            }

            return authToken;
        }
    }
}
