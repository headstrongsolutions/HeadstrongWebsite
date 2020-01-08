using System.Collections.Generic;
using System.Linq;

namespace Headstrong.Models
{
    public class JarvisConfigurationModel
    {
        public TemperatureReadingsConfigurationModel TemperatureReadings { get; set; }
    }
    public class TemperatureReadingsConfigurationModel
    {
        public string BaseAddress { get; set; }
        public string TemperatureUrl { get; set; }
    }
}
