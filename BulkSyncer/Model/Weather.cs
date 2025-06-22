using System;
using System.Text.Json.Serialization;

namespace BulkSyncer.Model
{
    public class Weather
    {
        public string Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF { get; set; }

        public string Summary { get; set; }
    }
}
