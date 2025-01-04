using Newtonsoft.Json.Linq;
using Services;

namespace Tasks
{
    class EventManager
    {
        private static JObject data;
        private static HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };

        public static async Task FetchEvents()
        {
            const string url = "https://iranbroker.net/wp-admin/admin-ajax.php";
            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("action", "ibwCalenderGetData"),
                new KeyValuePair<string, string>("ibwDataType", "day"),
                new KeyValuePair<string, string>("ibwDataKey", "this"),
                new KeyValuePair<string, string>("ibwImpactsKeys[]", "low"),
                new KeyValuePair<string, string>("ibwImpactsKeys[]", "medium"),
                new KeyValuePair<string, string>("ibwImpactsKeys[]", "high"),
                new KeyValuePair<string, string>("ibwImpactsKeys[]", "holiday")
            };

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new FormUrlEncodedContent(formData)
                };

                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    data = JObject.Parse(jsonResponse);
                }
                else
                {
                    NotificationManager.ShowNotification(0, "Request failed", $"Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                NotificationManager.ShowNotification(0, "WebRequest Error", ex.Message);
            }
        }

        public static async Task StartEventProcessingTask()
        {
            while (true)
            {
                ProcessEvents();
                await Task.Delay(60000);
            }
        }

        private static void ProcessEvents()
        {
            try
            {
                if (data["data"]?.HasValues != true) return;

                var eventsArray = data["data"]?.First?.First as JArray;
                if (eventsArray == null || eventsArray.Count == 0)
                {
                    NotificationManager.ShowNotification(0, "No Events", "No events found in the data.");
                    return;
                }

                var impactTranslations = new Dictionary<string, string>
                {
                    { "low", "کم" },
                    { "medium", "متوسط" },
                    { "high", "زیاد" },
                    { "holiday", "تعطیلات" }
                };

                foreach (var newsItem in eventsArray)
                {
                    DateTime newsTime = DateTime.Parse(newsItem["timestamp_raw"]?.ToString() ?? string.Empty);
                    string currency = newsItem["currency"]?.ToString();
                    string title = newsItem["title"]["fa"]?.ToString();
                    string impact = newsItem["impact"]?.ToString();
                    impact = impactTranslations.GetValueOrDefault(impact, "");
                    string id = newsItem["eventId"]?.ToString();

                    TimeSpan timeUntilEvent = newsTime - DateTime.Now;

                    bool isEventUpcoming = timeUntilEvent.TotalSeconds > 0 && timeUntilEvent.TotalSeconds <= 300;

                    if (isEventUpcoming)
                    {
                        string notificationTitle = $"{currency} [{impact}]";
                        NotificationManager.ShowNotification(int.Parse(id), notificationTitle, title);
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationManager.ShowNotification(0, "JSON Parsing Error", ex.Message);
            }
        }
    }
}
