using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

namespace ConsoleAppForTest
{
    class Program
    {
        static void Main()
        {
            PostMessage("123");
            //return;
            //MakeRequest();
            Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();
        }

        private async static void PostMessage(string message)
        {
            HttpClient client;
            HttpResponseMessage response;

            bool IsReplyReceived = false;

            client = new HttpClient();
            client.BaseAddress = new Uri("https://directline.botframework.com/api/conversations/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("BotConnector", "uc0nyMvM0NI.cwA.wYs.B8F1M7cEBm9StsTDG8pmuOjhnxeCJd2LdvNlKVfBgro");
            response = await client.GetAsync("/api/tokens/");
            if (response.IsSuccessStatusCode)
            {
                var conversation = new Conversation();
                //response = await client.PostAsJsonAsync("/api/conversations/", conversation);
                response = await client.PostAsync("/api/conversations/", null);
                if (response.IsSuccessStatusCode)
                {
                    Conversation ConversationInfo = response.Content.ReadAsAsync(typeof(Conversation)).Result as Conversation;
                    string conversationUrl = ConversationInfo.conversationId + "/messages/";
                    Message msg = new Message() { text = message };
                    response = await client.PostAsJsonAsync(conversationUrl, msg);
                    if (response.IsSuccessStatusCode)
                    {
                        response = await client.GetAsync(conversationUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            MessageSet BotMessage = response.Content.ReadAsAsync(typeof(MessageSet)).Result as MessageSet;
                            //string Messages = BotMessage;
                            IsReplyReceived = true;
                        }
                    }
                }

            }
            //return IsReplyReceived;
        }

        static async void MakeRequest()
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString("我要一张上海的机票");

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "f338d46de48742f994cda51278a8d02e");

            var uri = "https://api.projectoxford.ai/luis/v1/application?id=37af8540-1208-4ff8-8fc6-bc8057fda50d&q=" + "我要一张去广州的机票";

            var response = await client.GetAsync(uri);

            string JSON = await response.Content.ReadAsStringAsync();

            Lusi ro = JsonHelper.Deserialize<Lusi>(JSON);

            return;
        }
    }





    [DataContract]
    public class Intent
    {
        [DataMember]
        public string intent { get; set; }
        [DataMember]
        public double score { get; set; }
    }

    [DataContract]
    public class Resolution
    {
        [DataMember]
        public string date { get; set; }
    }

    [DataContract]
    public class Entity
    {
        [DataMember]
        public string entity { get; set; }
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public int startIndex { get; set; }
        [DataMember]
        public int endIndex { get; set; }
        [DataMember]
        public double score { get; set; }
        [DataMember]
        public Resolution resolution { get; set; }
    }

    [DataContract]
    public class Lusi
    {
        [DataMember]
        public string query { get; set; }
        [DataMember]
        public List<Intent> intents { get; set; }
        [DataMember]
        public List<Entity> entities { get; set; }
    }


    public class JsonHelper
    {
        /// <summary>
        /// 将JSON字符串反序列化成数据对象
        /// </summary>
        /// <typeparam name="T">数据对象类型</typeparam>
        /// <param name="json">JSON字符串</param>
        /// <returns>返回数据对象</returns>
        public static T Deserialize<T>(string json)
        {
            var _Bytes = Encoding.Unicode.GetBytes(json);
            using (MemoryStream _Stream = new MemoryStream(_Bytes))
            {
                var _Serializer = new DataContractJsonSerializer(typeof(T));
                return (T)_Serializer.ReadObject(_Stream);
            }
        }

        /// <summary>
        /// 将object序列化成JSON字符串 
        /// </summary>
        /// <param name="instance">被序列化对象</param>
        /// <returns>返回json字符串</returns>
        public static string Serialize(object instance)
        {
            using (MemoryStream _Stream = new MemoryStream())
            {
                var _Serializer = new DataContractJsonSerializer(instance.GetType());
                _Serializer.WriteObject(_Stream, instance);
                _Stream.Position = 0;
                using (StreamReader _Reader = new StreamReader(_Stream))
                { return _Reader.ReadToEnd(); }
            }
        }
    }

    public class Conversation
    {
        public string conversationId { get; set; }
        public string token { get; set; }
        public string eTag { get; set; }
    }

    public class MessageSet
    {
        public Message[] messages { get; set; }
        public string watermark { get; set; }
        public string eTag { get; set; }
    }

    public class Message
    {
        public string id { get; set; }
        public string conversationId { get; set; }
        public DateTime created { get; set; }
        public string from { get; set; }
        public string text { get; set; }
        public string channelData { get; set; }
        public string[] images { get; set; }
        public Attachment[] attachments { get; set; }
        public string eTag { get; set; }
    }

    public class Attachment
    {
        public string url { get; set; }
        public string contentType { get; set; }
    }

}
