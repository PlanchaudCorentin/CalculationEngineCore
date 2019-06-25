namespace CalculationEngineCoreLibrary
{
    public class DeviceStats
    {
        private string macAddress;
        private string deviceType;
        private double cValue;
        private string timestamp;

        public DeviceStats(string macAddress, string deviceType, double cValue, string timestamp)
        {
            this.macAddress = macAddress;
            this.deviceType = deviceType;
            this.cValue = cValue;
            this.timestamp = timestamp;
        }

        public string MacAddress
        {
            get => macAddress;
            set => macAddress = value;
        }

        public string DeviceType
        {
            get => deviceType;
            set => deviceType = value;
        }

        public double CValue
        {
            get => cValue;
            set => cValue = value;
        }

        public string Timestamp
        {
            get => timestamp;
            set => timestamp = value;
        }
    }
}