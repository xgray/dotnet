using System;
using Xunit;

using Thrift.Net;

namespace Tests
{
    public class Tests
    {
        [Fact]
        public void TestXml() 
        {
            Simple simple1 = Simple.Create();
            string xml1 = Proto<Simple>.ToXml(simple1);
            Simple simple2 = Proto<Simple>.FromXml(xml1);
            string xml2 = Proto<Simple>.ToXml(simple2);
            Assert.Equal(xml1, xml2);
        }

        [Fact]
        public void TestJson() 
        {
            Simple simple1 = Simple.Create();
            string json1 = Proto<Simple>.ToJson(simple1);
            Simple simple2 = Proto<Simple>.FromJson(json1);
            string json2 = Proto<Simple>.ToJson(simple2);
            Assert.Equal(json1, json2);
        }
    }
}
