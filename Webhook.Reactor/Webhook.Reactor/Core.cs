using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http;

namespace Webhook.Reactor
{
    public class Core
    {
        public bool ScanWebhook(string Webhook)
        {
            bool Result = false;

            try
            {
                using (var Client = new WebClient { Proxy = null })
                {
                    string WebhookContent = Client.DownloadString(Webhook);
                    var JsonData = JsonConvert.DeserializeObject<JToken>(WebhookContent);

                    if (JsonData["type"]?.ToString() == "1")
                    {
                        Result = true;
                    }
                }
            }
            catch
            {
                Result = false;
            }

            return Result;
        }

        public string RetrieveDetails(string Webhook)
        {
            string Data = null;

            if (ScanWebhook(Webhook) == true)
            {
                var Client = new WebClient();
                Client.Proxy = null;

                string WebhookContent = Client.DownloadString(Webhook);
                var JsonData = JsonConvert.DeserializeObject<JToken>(WebhookContent);

                return string.Concat(new string[]
                {
                    $"Type: {JsonData["type"]?.ToString()}\n",
                    $"ID: {JsonData["id"]?.ToString()}\n",
                    $"Name: {JsonData["name"]?.ToString()}\n",
                    $"Avatar: {JsonData["avatar"]?.ToString()}\n",
                    $"Channel_ID: {JsonData["channel_id"]?.ToString()}\n",
                    $"Guild_ID: {JsonData["guild_id"]?.ToString()}\n",
                    $"Application_ID: {JsonData["application_ID"]?.ToString()}\n",
                    $"Token: {JsonData["token"]?.ToString()}\n",
                });
            }

            return Data;
        }

        public void SendMessage(string Webhook, string Message, string AvatarUrl = null)
        {
            HttpClient Client = new HttpClient();
            Dictionary<string, string> Contents = new Dictionary<string, string>
                {
                    { "content", Message },
                    { "username", "Webhook" },
                    { "avatar_url", AvatarUrl }
                };
            Client.PostAsync(Webhook, new FormUrlEncodedContent(Contents)).GetAwaiter().GetResult();
        }

        public bool DeleteWebhook(string Webhook)
        {
            bool Result = false;

            WebRequest Request = WebRequest.Create(Webhook);
            Request.Method = "DELETE";

            HttpWebResponse Response = (HttpWebResponse)Request.GetResponse();
            if (Response.StatusDescription.Contains("No Content"))
            {
                Result = true;
            }
            return Result;
        }
    }
}
