namespace CalculationEngineCoreLibrary
{
    public class DeviceStats
    {
        private string macAddress;
        private string deviceType;
        private double cValue;

        public DeviceStats(string macAddress, string deviceType, double cValue)
        {
            this.macAddress = macAddress;
            this.deviceType = deviceType;
            this.cValue = cValue;
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
    }
}