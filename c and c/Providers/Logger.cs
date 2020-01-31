using CC.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace CC.Providers
{
    public class Logger
    {
        private static string LogFileName = ConfigurationManager.AppSettings["LogPath"].ToString();
        private static string LogTimeFormat = ConfigurationManager.AppSettings["LogTimeFormat"].ToString();

        public static void Log(string eventName, Dictionary<string, object> LogDetails)
        {
            LogDetails.Add("User", App.User.Username);

            var logObject = new LogObject
            {
                EventName = eventName,
                UserId = App.User.UserId,
                EventDetails = LogDetails
            };

            File.WriteAllText($"{LogFileName}{DateTime.Now.ToString(LogTimeFormat)}", JsonConvert.SerializeObject(logObject));
        }

        public static void Log(string eventName, string LogDetail)
        {
            Log(eventName, new Dictionary<string, object> { { "Message", LogDetail } });
        }
    }
}
