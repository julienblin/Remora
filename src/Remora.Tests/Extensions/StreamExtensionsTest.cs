using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Remora.Extensions;

namespace Remora.Tests.Extensions
{
    [TestFixture]
    public class StreamExtensionsTest : BaseTest
    {
        [Test]
        public void It_should_correctly_read_streams()
        {
            const int sampleDataSize = 90000;
            var sampleData = new byte[sampleDataSize];
            for (int i = 0; i < sampleDataSize; i++)
            {
                sampleData[i] = 5;
            }

            var testStream = new MemoryStream(sampleData);

            var result = testStream.ReadFully();

            Assert.That(result, Is.EqualTo(sampleData));
        }
    }
}
