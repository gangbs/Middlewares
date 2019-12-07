using AdysTech.InfluxDB.Client.Net;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Influxdb.Infrastructure
{
    public class IfxRepository : IIfxRepository,IDisposable
    {
        InfluxDBClient _client;

        public IfxRepository(string url, string userName = null, string pwd = null)
        {
            if (string.IsNullOrEmpty(userName))
                this._client = new InfluxDBClient(url);
            else
                this._client = new InfluxDBClient(url, userName, pwd);
        }

        public async Task<List<String>> AllDbNamesAsync()
        {
            List<String> dbNames = await _client.GetInfluxDBNamesAsync();
            return dbNames;
        }

        public async Task<bool> CreateDbAync(string dbName)
        {
            bool success = await _client.CreateDatabaseAsync(dbName);
            return success;
        }

        public async Task<bool> InsertDataAsync<T>(string dbName, T data) where T: IfxPointBase
        {
            var point = data.ToPoint();
            var r = await _client.PostPointAsync(dbName, point);
            return r;
        }

        public async Task<bool> BatchInsertDataAsync<T>(string dbName, List<T> datas) where T : IfxPointBase
        {
            var points = datas.ConvertAll<IInfluxDatapoint>(data => data.ToPoint());
            var r = await _client.PostPointsAsync(dbName, points);
            return r;
        }

        public async Task<List<IfxSeriesItem<T>>> QuerySeriesAsync<T>(string dbName, string sql) where T : class
        {
            var lstSeries = new List<IfxSeriesItem<T>>();
            var r = await _client.QueryMultiSeriesAsync(dbName, sql);
            foreach(var s in r)
            {
                var series = new IfxSeriesItem<T>();
                series.SeriesName = s.SeriesName;
                series.Tags = s.Tags;
                series.Datas= s.Entries.ToList().ConvertAll<ExpandoObject>(x => (ExpandoObject)x).ToObject<T>();
                lstSeries.Add(series);
            }
            return lstSeries;
        }

        public async Task<IfxSeriesItem<T>> QuerySingleSeriesAsync<T>(string dbName, string sql) where T : class
        {
            var res = new IfxSeriesItem<T>();
            var r = await _client.QueryMultiSeriesAsync(dbName, sql);
            var series = r.FirstOrDefault();
            if (series == null) return null;
            res.SeriesName = series.SeriesName;
            res.Tags = series.Tags;
            var lstEntry = series.Entries.ToList().ConvertAll<ExpandoObject>(x => (ExpandoObject)x);
            res.Datas = lstEntry.ToObject<T>();
            return res;
        }

        public void Dispose()
        {
            this._client.Dispose();
        }
    }
}
