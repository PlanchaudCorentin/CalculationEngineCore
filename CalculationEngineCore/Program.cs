using CalculationEngineCoreLibrary;

namespace CalculationEngineCore
{
    class Program
    {
        static void Main(string[] args)
        {
            Connector conn = new Connector("admin", "devproject", "/", "192.168.43.88");
            conn.startConsume("DOTNET_queue");
        }
    }
}