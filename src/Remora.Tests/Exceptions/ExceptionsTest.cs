#region Licence

// The MIT License
// 
// Copyright (c) 2011 Julien Blin, julien.blin@gmail.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using Remora.Exceptions;

namespace Remora.Tests.Exceptions
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
                return (T) serializer.Deserialize(stream);
            }
        }

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

            Assert.That(() => new ClientCertificateException(), Throws.Nothing);
            Assert.That(() => new ClientCertificateException("message"), Throws.Nothing);
            Assert.That(() => new ClientCertificateException("message", innerException), Throws.Nothing);
            Assert.That(() => SerializeAndDeserialize<ClientCertificateException>(), Throws.Nothing);

            Assert.That(() => new SoapTransformerException(), Throws.Nothing);
            Assert.That(() => new SoapTransformerException("message"), Throws.Nothing);
            Assert.That(() => new SoapTransformerException("message", innerException), Throws.Nothing);
            Assert.That(() => SerializeAndDeserialize<SoapTransformerException>(), Throws.Nothing);

            Assert.That(() => new SoapPlayerException(), Throws.Nothing);
            Assert.That(() => new SoapPlayerException("message"), Throws.Nothing);
            Assert.That(() => new SoapPlayerException("message", innerException), Throws.Nothing);
            Assert.That(() => SerializeAndDeserialize<SoapPlayerException>(), Throws.Nothing);

            Assert.That(() => new SetHttpHeaderException(), Throws.Nothing);
            Assert.That(() => new SetHttpHeaderException("message"), Throws.Nothing);
            Assert.That(() => new SetHttpHeaderException("message", innerException), Throws.Nothing);
            Assert.That(() => SerializeAndDeserialize<SetHttpHeaderException>(), Throws.Nothing);
        }
    }
}