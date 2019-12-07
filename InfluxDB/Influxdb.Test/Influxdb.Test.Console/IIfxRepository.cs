using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Influxdb.Test.Console
{
    public interface IIfxRepository
    {
        Task<bool> InsertDataAsync<T>(string dbName, T data);
        Task<bool> BatchInsertDataAsync<T>(string dbName, List<T> datas);
        Task<List<IfxSeriesItem<T>>> QuerySeriesAsync<T>(string dbName, string sql) where T : class;
        Task<IfxSeriesItem<T>> QuerySingleSeriesAsync<T>(string dbName, string sql) where T : class;
    }
}
