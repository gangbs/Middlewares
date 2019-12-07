using AdysTech.InfluxDB.Client.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Influxdb.Infrastructure;

namespace Influxdb.Test.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = "http://192.168.10.173:8086";
            var dbName = "mydb1";
            IIfxRepository rep = new IfxRepository(url);

            BathInsertTest(rep,dbName);



            System.Console.ReadKey();
        }

        static void QueryAllDbName(IIfxRepository rep)
        {
           var lst= rep.AllDbNamesAsync().Result;
        }

        static void QuerySingleSeries(IIfxRepository rep,string dbName)
        {
            string sql = "select * from rate_test";

            var res = rep.QuerySingleSeriesAsync<StableRateModel>(dbName, sql).Result;
        }

        static void QueryMuiltySeries(IIfxRepository rep, string dbName)
        {
            string sql2 = "select mean(rate) as rate,mean(score) as score from rate_min where time >='2019-10-01 00:00:00' and time<'2019-10-02 00:00:00' group by time(1h),tagId";

            var res2 = rep.QuerySeriesAsync<StableRateModel>(dbName, sql2).Result;
        }


        static void BathInsertTest(IIfxRepository rep, string dbName)
        {
            var datas = new List<StableRateModel>();
            DateTime dt = new DateTime(2019, 1, 1);
            Random r = new Random();
            for (int i = 0; i < 100050; i++)
            {
                datas.Add(new StableRateModel { Time = dt, status = r.Next(10), rate = i+1, score = i+1, tagId = i+1 });
               dt= dt.AddMinutes(1);
            }
                

            Stopwatch sw = new Stopwatch();
            sw.Start();

           var result=  rep.BatchInsertDataAsync<StableRateModel>(dbName, datas).Result;
            sw.Stop();

            System.Console.WriteLine($"插入100000数据，耗时：{sw.ElapsedMilliseconds}毫秒");


        }

    }


    public class StableRateModel: IfxPointBase
    {
        public const string tbName = "rate_test2";
        public double rate { get; set; }
        public double score { get; set; }
        public int status { get; set; }
        public int tagId { get; set; }

        public override IInfluxDatapoint ToPoint()
        {
            var point = new InfluxDatapoint<double>();
            point.UtcTimestamp = this.Time;
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
