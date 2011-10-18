using System.IO;
using NUnit.Framework;
using Remora.Exceptions;
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
            for (var i = 0; i < sampleDataSize; i++)
            {
                sampleData[i] = 5;
            }

            var testStream = new MemoryStream(sampleData);

            var result = testStream.ReadFully(0);

            Assert.That(result, Is.EqualTo(sampleData));
        }

        [Test]
        public void It_should_throw_MaxMessageSizeException_if_limit_is_reached()
        {
            const int sampleDataSize = 90000;
            var sampleData = new byte[sampleDataSize];
            for (var i = 0; i < sampleDataSize; i++)
            {
                sampleData[i] = 5;
            }

            var testStream = new MemoryStream(sampleData);

            Assert.That(() => testStream.ReadFully(10), 
                Throws.Exception.TypeOf<MaxMessageSizeException>()
                .With.Message.Contains("10")
            );
        }
    }
}
