using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using NUnit.Framework;
using Remora.Host.Exceptions;

namespace Remora.Host.Tests.Exceptions
{
    [TestFixture]
    public class ExceptionsTest : BaseTest
    {
        private T SerializeAndDeserialize<T>()
            where T : new()
        {
            var reference = new T();
            using (var stream = new MemoryStream())
            {
                var serializer = new BinaryFormatter();
                serializer.Serialize(stream, reference);
                stream.Position = 0;
                return (T)serializer.Deserialize(stream);
            }
        }

        [Test]
        public void It_should_be_possible_to_create_exceptions()
        {
            var innerException = new Exception();

            Assert.That(() => new RemoraHostConfigException(), Throws.Nothing);
            Assert.That(() => new RemoraHostConfigException("message"), Throws.Nothing);
            Assert.That(() => new RemoraHostConfigException("message", innerException), Throws.Nothing);
            Assert.That(() => SerializeAndDeserialize<RemoraHostConfigException>(), Throws.Nothing);
        }
    }
}
