using EKonsulatConsole;
using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EKonsulatConsole.UnitTest
{
    [TestClass()]
    public class MainUnitTest
    {
        [TestMethod()]
        public void CanPingTestNotNull()
        {
            string testProxy = "211.162.0.163:80";
            Helper helper = new Helper();
            bool test = helper.CanPing(testProxy);
            Assert.IsNotNull(test);
        }

        [TestMethod()]
        public void CanPingTestResult()
        {
            string testProxy = "211.162.0.163";
            Helper helper = new Helper();
            bool test = helper.CanPing(testProxy);
            Assert.IsTrue(test);
        }

        [TestMethod()]
        public void IsProxyFileExist()
        {
            string file = @"C:\Users\Vik\Documents\Visual Studio 2015\Projects\EKosulat\WpfVisaPolandUkraine\EKonsulatConsole\bin\Debug\Proxy.txt";
            var test = File.Exists(file);
            Assert.IsTrue(test);
        }
    }
}

