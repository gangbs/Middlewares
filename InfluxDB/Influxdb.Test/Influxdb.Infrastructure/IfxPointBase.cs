using AdysTech.InfluxDB.Client.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace Influxdb.Infrastructure
{
    public abstract class IfxPointBase
    {
        public string MeasurementName { get; set; }
        public DateTime Time { get; set; }
        public TimePrecision Precision { get; set; }
        public IInfluxRetentionPolicy Retention { get; set; }
        public abstract IInfluxDatapoint ToPoint();
    }
}
