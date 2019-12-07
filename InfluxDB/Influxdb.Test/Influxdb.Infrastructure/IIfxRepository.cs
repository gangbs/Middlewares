using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Influxdb.Infrastructure
{
    public interface IIfxRepository
    {
        Task<List<String>> AllDbNamesAsync();
        Task<bool> CreateDbAync(string dbName);
        Task<bool> InsertDataAsync<T>(string dbName, T data) where T : IfxPointBase;
        Task<bool> BatchInsertDataAsync<T>(string dbName, List<T> datas) where T: IfxPointBase;
        Task<List<IfxSeriesItem<T>>> QuerySeriesAsync<T>(string dbName, string sql) where T : class;
        Task<IfxSeriesItem<T>> QuerySingleSeriesAsync<T>(string dbName, string sql) where T : class;
    }
}
