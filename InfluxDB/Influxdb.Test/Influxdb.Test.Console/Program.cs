using AdysTech.InfluxDB.Client.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Influxdb.Test.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = "http://192.168.10.173:8086";
            var dbName = "mydb1";
            var client = GetClient(url);
            //var data = new StableRateModel { time = DateTime.Now.AddHours(-1), tagId = 1, status = 0, rate = 99, score = 100 };
            //var r = InsertDataAsync(client, dbName, data).Result;

            //var sw = new Stopwatch();
            //var datas = GenDatas(10);
            //sw.Start();
            //var r = BatchInsertDataAsync(client, dbName, datas).Result;
            //sw.Stop();
            //long t = sw.ElapsedMilliseconds;
            //System.Console.WriteLine($"批量插入耗时：{t}毫秒");

            //string sql = "select max(rate) from rate_test group by tagId";

            //string sql = "select * from rate_test group by tagId";
            string sql = "select * from rate_min where time>='2018-10-01T00:00:00Z' and time<'2018-10-02T00:00:00Z' group by tagId";
            var r = QueryAsync(client, dbName,sql);


            System.Console.ReadKey();
        }


        static InfluxDBClient GetClient(string url)
        {
            InfluxDBClient client = new InfluxDBClient(url);
            return client;
        }

        static async Task<bool> InsertDataAsync(InfluxDBClient client, string dbName,StableRateModel data)
        {
            var point = data.ToPoint();
            var r = await client.PostPointAsync(dbName, point);
            return r;
        }

        static async Task<bool> BatchInsertDataAsync(InfluxDBClient client, string dbName, List<StableRateModel> datas)
        {
            var points = datas.ConvertAll<InfluxDatapoint<double>>(data => data.ToPoint());
            var r= await client.PostPointsAsync(dbName, points);
            return r;
        }

        static List<StableRateModel> GenDatas(int size)
        {
            var dt = DateTime.Now.AddDays(-1);
            var r = new Random();
            var lst = new List<StableRateModel>();
            for(int i=0;i<size;i++)
            {
                var data = new StableRateModel { time=dt, tagId=r.Next(1,10), status=r.Next(0,1), rate=r.Next(0,100),score=r.Next(10,80)};
                lst.Add(data);
                //dt = dt.AddMinutes(-1);
            }
            return lst;
        }


        static async Task<List<StableRateModel>> QueryAsync(InfluxDBClient client,string dbName,string sql)
        {
            var r = await client.QueryMultiSeriesAsync(dbName, sql);       
            var lstEntry = r.FirstOrDefault()?.Entries.ToList().ConvertAll<ExpandoObject>(x => (ExpandoObject)x);
            var lst = lstEntry.ToObject<StableRateModel>();
            return lst; 
        }


        public static List<StableRateModel> GetFromDynArry(List<dynamic> lstDyn)
        {
            ExpandoObject eo0 = new ExpandoObject();

            eo0.TryAdd("name", "yyg");

            var dic = eo0.ToDictionary(x => x.Key, x => x.Value);
            
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(lstDyn);
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<List<StableRateModel>>(json);
            return obj;
        }

    }


    public class StableRateModel
    {
        public const string tbName = "rate_test";
        public DateTime time { get; set; }
        public double rate { get; set; }
        public double score { get; set; }
        public int status { get; set; }
        public int tagId { get; set; }

        public InfluxDatapoint<double> ToPoint()
        {
            var point = new InfluxDatapoint<double>();
            point.UtcTimestamp = this.time;
            point.Tags.Add(nameof(StableRateModel.tagId), this.tagId.ToString());
            point.Tags.Add(nameof(StableRateModel.status), this.status.ToString());
            //point.Fields.Add(nameof(StableRateModel.rate), new InfluxValueField(85.23));
            //point.Fields.Add(nameof(StableRateModel.score), new InfluxValueField(95.32));
            point.Fields.Add(nameof(StableRateModel.rate), this.rate);
            point.Fields.Add(nameof(StableRateModel.score), this.score);
            point.MeasurementName = StableRateModel.tbName;
            point.Precision = TimePrecision.Seconds;
            point.Retention = new InfluxRetentionPolicy();
            return point;
        }

    }

    
}
