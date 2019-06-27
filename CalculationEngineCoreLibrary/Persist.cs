using System.Collections.Generic;
using CalculationEngineDBC;

namespace CalculationEngineCoreLibrary
{
    public class Persist
    {
        public static void persistData(Dictionary<string, DeviceStats> dictionary)
        {
            DatabaseConnection dbcon = DatabaseConnection.Instance();
            string[] args = {"m_a", "d_t", "t_s", "c_v"};
            if (dbcon.connect("192.168.43.146",
                "3306",
                "statistics",
                "dev",
                "devproject"))
            {
                foreach (KeyValuePair<string, DeviceStats> x in dictionary)
                {
                    string[] values =
                        {x.Value.MacAddress, x.Value.DeviceType, x.Value.Timestamp, x.Value.CValue.ToString()
                            .Replace(",", ".")};
                    dbcon.executeStoredProcedure("persist_stat", args, values);
                }
                dbcon.Close();
            } 
        }
    }
}