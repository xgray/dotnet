using System;
using Xunit;

using Thrift.Net;

namespace Tests
{
    public class Tests
    {
        [Fact]
        public void Test1() 
        {
            Simple simple = Simple.Create();
            Console.WriteLine(Proto<Simple>.ToXml(simple));
            Console.WriteLine(Proto<Simple>.ToJson(simple));
            Complex complex = Complex.Create();
            Console.WriteLine(Proto<Complex>.ToXml(complex));
            Console.WriteLine(Proto<Complex>.ToJson(complex));
        }
    }
}
