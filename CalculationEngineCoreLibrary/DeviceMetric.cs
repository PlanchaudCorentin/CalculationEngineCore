namespace CalculationEngineCoreLibrary
{
    public class DeviceMetric
    {
        public DeviceMetric()
        {
        }

        private double metricValue;
        private string deviceType;
        private string macAddress;
        private string metricDate;

        public double MetricValue
        {
            get => metricValue;
            set => metricValue = value;
        }

        public string DeviceType
        {
            get => deviceType;
            set => deviceType = value;
        }

        public string MacAddress
        {
            get => macAddress;
            set => macAddress = value;
        }

        public string MetricDate
        {
            get => metricDate;
            set => metricDate = value;
        }

        public double mean(double d)
        {
            return (d + metricValue) / 2;
        }
    }
}