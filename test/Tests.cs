using System;
using Xunit;

using Thrift.Net;
using Bench;

namespace Tests
{
    public class Tests
    {
        [Fact]
        public void TestSimpleBytes() 
        {
            Simple simple1 = Simple.Create();
            byte[] buf1 = ThriftSerializer<Simple>.ToBytes(simple1);
            Simple simple2 = ThriftSerializer<Simple>.FromBytes(buf1);
            byte[] buf2 = ThriftSerializer<Simple>.ToBytes(simple2);
            
            Assert.Equal(buf1, buf2);
        }

        [Fact]
        public void TestComplexBytes() 
        {
            Complex complex1 = Complex.Create();
            byte[] buf1 = ThriftSerializer<Complex>.ToBytes(complex1);
            Complex complex2 = ThriftSerializer<Complex>.FromBytes(buf1);
            byte[] buf2 = ThriftSerializer<Complex>.ToBytes(complex2);
            
            Assert.Equal(buf1, buf2);
        }

        [Fact]
        public void TestSimpleXml() 
        {
            Simple simple1 = Simple.Random();
            string xml1 = Proto<Simple>.ToXml(simple1);
            Simple simple2 = Proto<Simple>.FromXml(xml1);
            string xml2 = Proto<Simple>.ToXml(simple2);
            Assert.Equal(xml1, xml2);
            Console.WriteLine(xml1);
        }

        [Fact]
        public void TestComplexXml() 
        {
            Complex complex1 = Complex.Create();
            string xml1 = Proto<Complex>.ToXml(complex1);
            Complex complex2 = Proto<Complex>.FromXml(xml1);
            string xml2 = Proto<Complex>.ToXml(complex2);
            Assert.Equal(xml1, xml2);
        }

        [Fact]
        public void TestSimpleJson() 
        {
            Simple simple1 = Simple.Create();
            string json1 = Proto<Simple>.ToJson(simple1);
            Simple simple2 = Proto<Simple>.FromJson(json1);
            string json2 = Proto<Simple>.ToJson(simple2);
        }

        [Fact]
        public void TestComplexJson() 
        {
            Complex complex1 = Complex.Create();
            string json1 = Proto<Complex>.ToJson(complex1);
            Complex complex2 = Proto<Complex>.FromJson(json1);
            string json2 = Proto<Complex>.ToJson(complex2);
            Assert.Equal(json1, json2);
        }
    }
}
