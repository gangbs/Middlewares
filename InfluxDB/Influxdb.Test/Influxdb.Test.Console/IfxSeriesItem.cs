using System;
using System.Collections.Generic;
using System.Text;

namespace Influxdb.Test.Console
{
    public class IfxSeriesItem<T> where T : class
    {
        public string SeriesName { get; set; }

        public IReadOnlyDictionary<string, string> Tags { get; set; }

        public IReadOnlyList<T> Datas { get; set; }
    }
}
