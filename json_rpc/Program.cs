using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace json_rpc
{
    class Program
    {
        /// <summary>
        /// API 参考：http://docs.neo.org/zh-cn/node/api.html
        /// API Reference: http://docs.neo.org/en-us/node/api.html
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine("输入要创建地址的数量");
            var input = Console.ReadLine();
            int.TryParse(input, out int count);
            Console.WriteLine($"创建地址数量：{count}，并发：4线程");
            Task t1 = Task.Factory.StartNew(() => CreateAddress(count / 4, 1));
            Task t2 = Task.Factory.StartNew(() => CreateAddress(count / 4, 2));
            Task t3 = Task.Factory.StartNew(() => CreateAddress(count / 4, 3));
            Task t4 = Task.Factory.StartNew(() => CreateAddress(count - (count / 4) * 3, 4));
            Console.ReadLine();
        }

        public static void CreateAddress(int count, int thread)
        {
            for (int i = 0; i < count; i++)
            {
                var r = PostWebRequest("http://localhost:10332", "{'jsonrpc': '2.0', 'method': 'getnewaddress', 'params': [],  'id': 1}");
                if (string.IsNullOrEmpty(r))
                {
                    Console.WriteLine("请打开neo-cli");
                }
                var address = JObject.Parse(r)["result"];
                if (address != null)
                {
                    Console.WriteLine(address);
                }
                else {
                    var error = JObject.Parse(r)["error"];
                    Console.WriteLine(error);
                }
            }
            Console.WriteLine($"线程{thread}，OK");
        }

        public static string PostWebRequest(string postUrl, string paramData)
        {
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(paramData);
                WebRequest webReq = WebRequest.Create(postUrl);
                webReq.Method = "POST";
                using (Stream newStream = webReq.GetRequestStream())
                {
                    newStream.Write(byteArray, 0, byteArray.Length);
                }
                using (WebResponse response = webReq.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return "";
        }
    }
}
