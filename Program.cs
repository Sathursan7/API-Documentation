using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using MySql.Data.MySqlClient;
using System.Data;
using Newtonsoft.Json;

namespace GetMothodWithHttp
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsumeWebAPI().Wait();
        }


        static async Task ConsumeWebAPI()
        {
            
            string server = "localhost";
            string database = "endofdate";
            string usernam = "root";
            string password = "";
            string constring = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + usernam + ";" + "PASSWORD=" + password + ";";
            

            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://api.marketstack.com/v1/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            

            HttpResponseMessage response = await httpClient.GetAsync("eod/latest?access_key=7387fe5f652429f7ff1a48926a2f7c69&symbols=AAPL");
            if (response.IsSuccessStatusCode)
            {
                dynamic result = await response.Content.ReadAsStringAsync();
                
                Rootobject rootobject = JsonConvert.DeserializeObject<Rootobject>(result);
                
                foreach (var item in rootobject.data)
                {
                    Console.WriteLine("${0}\t${1}\t${2}\t${3}\t{4}\t{5}\t${6}\t{7}\t{8}\t{9}\t{10}\t{11}"
                        , item.open
                        , item.high
                        , item.low
                        , item.close
                        , item.volume
                        , item.adj_high
                        , item.adj_close
                        , item.split_factor
                        , item.dividend
                        , item.symbol
                        , item.exchange
                        , item.date
                        ) ;
                   
                    MySqlConnection conn = new MySqlConnection(constring);
                    DateTime date = DateTime.Now;

                    conn.Open();
                    string query = "Insert Into tableeodprice(Open, High, Low, Close,Volume,adjClose,split_factor,dividend,symbol,exchange,date) " +
                        "values('"+item.open+"', '"+item.high+"','"+item.low+ "','"+item.close+ "','"+item.volume+ "','"+item.adj_high+ "','"+item.split_factor+ "','"+item.dividend+ "','"+item.symbol+ "','"+item.exchange+ "','"+item.date.Date.ToString("yyyyMMdd")+"')";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    int value = cmd.ExecuteNonQuery();
                    Console.WriteLine(value + "Sucessfully added into the Database ");
                    conn.Close();
                    
                }
                Console.ReadKey();
               
            }
        }


        public class Rootobject
        {
            public Pagination pagination { get; set; }
            public List<Datum> data { get; set; }
        }

        public class Pagination
        {
            public int limit { get; set; }
            public int offset { get; set; }
            public int count { get; set; }
            public int total { get; set; }
        }

        public class Datum
        {
            public float open { get; set; }
            public float high { get; set; }
            public float low { get; set; }
            public float close { get; set; }
            public float volume { get; set; }
            public object adj_high { get; set; }
            public object adj_low { get; set; }
            public float adj_close { get; set; }
            public object adj_open { get; set; }
            public object adj_volume { get; set; }
            public float split_factor { get; set; }
            public float dividend { get; set; }
            public string symbol { get; set; }
            public string exchange { get; set; }
            public DateTime date { get; set; }
        }
    }
}
