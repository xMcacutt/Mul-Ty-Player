using Microsoft.VisualStudio.TestTools.UnitTesting;
using MulTyPlayerClient.Classes.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MulTyPlayerClient.Classes.Networking;

namespace MulTyPlayerClient.Classes.Networking.Tests
{
    [TestClass()]
    public class InterpolationTests
    {
        [TestMethod()]
        public void PredictFloatTest()
        {
            Assert.AreEqual(Interpolation.PredictFloat(5f, DateTime.UnixEpoch, 10f, DateTime.UnixEpoch.AddMilliseconds(57)), 5f);
        }
    }
}