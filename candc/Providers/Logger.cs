using CC.Constants;
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

        public static string Log(string eventName, Dictionary<string, object> LogDetails)
        {
            var logCode = Guid.NewGuid().ToString();

            LogDetails.Add("User", App.LoggedInUser?.Username);
            LogDetails.Add(LogConsts.LogNumber, logCode);

            var logObject = new LogObject
            {
                EventName = eventName,
                UserId = App.LoggedInUser.UserId,
                EventDetails = LogDetails
            };

            File.WriteAllText($"{LogFileName}{DateTime.Now.ToString(LogTimeFormat)}", JsonConvert.SerializeObject(logObject));
            return logCode;
        }

        public static string Log(string eventName, string LogDetail)
        {
            return Log(eventName, new Dictionary<string, object> { { "Message", LogDetail } });
        }
    }
}
