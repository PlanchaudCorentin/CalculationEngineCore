using System;
using CalculationEngineCoreLibrary;
using Xunit;

namespace CECLibraryTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            double a = Connector.getMeanFromPreviousIteration((1.0 + 2.0) / 2.0, 3, 2);
            double b = Math.Round((1.0 + 2 + 3) / 3, 2);
            Assert.True(a == b);
            double c = Math.Round(((1.0 + 2) / 2 + 3) / 2, 2);
            double d = Math.Round((1.0 + 2 + 3) / 3, 2);
            Assert.False(c == d);
            
        }
    }
}