using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using NUnit.Framework;
using Remora.Exceptions;

namespace Remora.Tests.Exceptions
{
    [TestFixture]
    public class ExceptionsTest : BaseTest
    {
        [Test]
        public void It_should_be_possible_to_create_exceptions()
        {
            var innerException = new Exception();

            Assert.That(() => new InvalidConfigurationException(), Throws.Nothing);
            Assert.That(() => new InvalidConfigurationException("message"), Throws.Nothing);
            Assert.That(() => new InvalidConfigurationException("message", innerException), Throws.Nothing);
            Assert.That(() => SerializeAndDeserialize<InvalidConfigurationException>(), Throws.Nothing);

            Assert.That(() => new InvalidDestinationUriException(), Throws.Nothing);
            Assert.That(() => new InvalidDestinationUriException("message"), Throws.Nothing);
            Assert.That(() => new InvalidDestinationUriException("message", innerException), Throws.Nothing);
            Assert.That(() => SerializeAndDeserialize<InvalidDestinationUriException>(), Throws.Nothing);

            Assert.That(() => new MaxMessageSizeException(), Throws.Nothing);
            Assert.That(() => new MaxMessageSizeException("message"), Throws.Nothing);
            Assert.That(() => new MaxMessageSizeException("message", innerException), Throws.Nothing);
            Assert.That(() => SerializeAndDeserialize<MaxMessageSizeException>(), Throws.Nothing);

            Assert.That(() => new SendException(), Throws.Nothing);
            Assert.That(() => new SendException("message"), Throws.Nothing);
            Assert.That(() => new SendException("message", innerException), Throws.Nothing);
            Assert.That(() => SerializeAndDeserialize<SendException>(), Throws.Nothing);

            Assert.That(() => new UnknownDestinationException(), Throws.Nothing);
            Assert.That(() => new UnknownDestinationException("message"), Throws.Nothing);
            Assert.That(() => new UnknownDestinationException("message", innerException), Throws.Nothing);
            Assert.That(() => SerializeAndDeserialize<UnknownDestinationException>(), Throws.Nothing);

            Assert.That(() => new UrlRewriteException(), Throws.Nothing);
            Assert.That(() => new UrlRewriteException("message"), Throws.Nothing);
            Assert.That(() => new UrlRewriteException("message", innerException), Throws.Nothing);
            Assert.That(() => SerializeAndDeserialize<UrlRewriteException>(), Throws.Nothing);
        }

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
    }
}
