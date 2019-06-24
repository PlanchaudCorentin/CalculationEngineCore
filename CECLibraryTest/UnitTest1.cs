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
            Assert.True(Connector.add(1, 2) == 3);
            Assert.False(Connector.add(10, 9) == 109);
        }
    }
}