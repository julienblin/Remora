#region License
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
using System.Diagnostics.Contracts;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using Castle.Core.Logging;
using Remora.Configuration;
using Remora.Core;
using Remora.Exceptions;
using Remora.Extensions;
using Remora.Pipeline;

namespace Remora.Components
{
    public class Sender : AbstractPipelineComponent
    {
        public const string ComponentId = @"sender";

        private static readonly Regex HttpSchemeRx = new Regex("^http(s)?$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
        private readonly IRemoraConfig _config;
        private ILogger _logger = NullLogger.Instance;

        public Sender(IRemoraConfig config)
        {
            if(config == null) throw new ArgumentNullException("config");
            Contract.EndContractBlock();

            _config = config;
        }

        /// <summary>
        /// Logger
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        public override void BeginAsyncProcess(IRemoraOperation operation, IComponentDefinition componentDefinition, Action<bool> callback)
        {
            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("Preparing to send {0}...", operation);

            if (operation.Request.Uri == null)
            {
                throw new UnknownDestinationException(string.Format("Unable to send {0}: no destination uri is defined. Either use a rewrite attribute on the pipeline or create a custom IPipelineComponent that will determine the destination uri.", operation));
            }

            if (!HttpSchemeRx.IsMatch(operation.Request.Uri.Scheme))
            {
                throw new InvalidDestinationUriException(string.Format("The destination uri for {0} is not a valid uri: {1}. Remora supports only http(s) destinations.", operation, operation.Request.Uri));
            }

            var webRequest = (HttpWebRequest)WebRequest.Create(operation.Request.Uri);

            if ((operation.ExecutingPipeline != null)
                && (operation.ExecutingPipeline.Definition != null)
                && (!string.IsNullOrEmpty(operation.ExecutingPipeline.Definition.ClientCertificateFilePath))
            )
            { 
                ManageCertificate(webRequest, operation.ExecutingPipeline.Definition.ClientCertificateFilePath, operation.ExecutingPipeline.Definition.ClientCertificatePassword);
            }

            webRequest.Method = operation.Request.Method ?? "POST";
            SetHttpHeaders(operation.Request, webRequest);

            WriteData(operation, webRequest);

            webRequest.BeginGetResponse((result) =>
            {
                webRequest = (HttpWebRequest)result.AsyncState;
                try
                {
                    var response = (HttpWebResponse)webRequest.EndGetResponse(result);
                    ReadResponse(operation, response, componentDefinition);
                    if(Logger.IsDebugEnabled)
                        Logger.DebugFormat("Successfully received response from {0} for {1}.", webRequest.RequestUri, operation);
                }
                catch (Exception ex)
                {
                    var message = string.Format("There has been an error while sending {0} to {1}.", operation, webRequest.RequestUri);
                    Logger.Error(message, ex);
                    operation.Exception = new SendException(message, ex);
                }
                finally
                {
                    callback(false);
                }
            }, webRequest);
        }

        protected virtual void ManageCertificate(HttpWebRequest webRequest, string clientCertificateFilePath, string clientCertificatePassword)
        {
            if(Logger.IsDebugEnabled)
                Logger.DebugFormat("Loading client certificate {0}...", clientCertificateFilePath);
            X509Certificate2 clientCertificate;
            try
            {
                if(string.IsNullOrEmpty(clientCertificatePassword))
                    clientCertificate = new X509Certificate2(clientCertificateFilePath);
                else
                    clientCertificate = new X509Certificate2(clientCertificateFilePath, clientCertificatePassword);

                if (Logger.IsDebugEnabled)
                    Logger.DebugFormat("Client certificate {0} loaded successfully.", clientCertificateFilePath);
            }
            catch (Exception ex)
            {
                throw new ClientCertificateException(string.Format("There has been an error while opening client certificate {0}.", clientCertificateFilePath), ex);
            }

            webRequest.ClientCertificates.Add(clientCertificate);
        }

        protected virtual void ReadResponse(IRemoraOperation operation, HttpWebResponse response, IComponentDefinition componentDefinition)
        {
            operation.Response.StatusCode = (int)response.StatusCode;
            operation.Response.Uri = response.ResponseUri;

            foreach (var header in response.Headers.AllKeys)
            {
                operation.Response.HttpHeaders.Add(header, response.Headers[header]);
            }

            using (var stream = response.GetResponseStream())
            {
                operation.Response.Data = stream.ReadFully(_config.MaxMessageSize);
            }

            ReadEncoding(operation, response, componentDefinition);
        }

        protected virtual void ReadEncoding(IRemoraOperation operation, HttpWebResponse response, IComponentDefinition componentDefinition)
        {
            if(Logger.IsDebugEnabled)
                Logger.DebugFormat("Determining encoding for response from {0} for operation {1}...", response.ResponseUri, operation);

            if ((operation.ExecutingPipeline != null)
                && (operation.ExecutingPipeline.Definition != null)
                && (operation.ExecutingPipeline.Definition.Properties.ContainsKey("forceResponseEncoding"))
                )
            {
                try
                {
                    operation.Response.ContentEncoding = Encoding.GetEncoding(operation.ExecutingPipeline.Definition.Properties["forceResponseEncoding"]);
                    if (Logger.IsDebugEnabled)
                        Logger.DebugFormat("Operation {0}: encoding forced to {1}.", operation, operation.Response.ContentEncoding);
                }
                catch (ArgumentException ex)
                {
                    Logger.ErrorFormat(ex, "There has been an error while loading encoding defined in forceResponseEncoding property: {0}", operation.ExecutingPipeline.Definition.Properties["forceResponseEncoding"]);
                    throw;
                }
            }
            else
            {
                var encoding = Encoding.UTF8; // default;
                if (!string.IsNullOrEmpty(response.CharacterSet))
                {
                    try
                    {
                        encoding = Encoding.GetEncoding(response.CharacterSet);

                        if (Logger.IsDebugEnabled)
                            Logger.DebugFormat("Operation {0}: loaded encoding {1} from character set: {2}", operation, encoding.EncodingName, response.CharacterSet);
                    }
                    catch (ArgumentException ex)
                    {
                        Logger.WarnFormat(ex, "Operation {0}: unable to load a proper encoding for character set {1}", operation, response.CharacterSet);
                    }
                }
                else
                {
                    if (Logger.IsDebugEnabled)
                        Logger.DebugFormat("Operation {0}: using default encoding {0}", operation, encoding.EncodingName);
                }

                operation.Response.ContentEncoding = encoding;
            }
        }

        protected virtual void WriteData(IRemoraOperation operation, HttpWebRequest webRequest)
        {
            if (operation.Request.Data != null)
            {
                webRequest.ContentLength = operation.Request.Data.Length;
                using (var requestStream = webRequest.GetRequestStream())
                {
                    requestStream.Write(operation.Request.Data, 0, operation.Request.Data.Length);
                }
            }
        }

        protected virtual void SetHttpHeaders(IRemoraRequest remoraRequest, HttpWebRequest webRequest)
        {
            foreach (var header in remoraRequest.HttpHeaders)
            {
                switch (header.Key.Trim().ToLowerInvariant())
                {
                    case "accept":
                        webRequest.Accept = header.Value;
                        break;
                    case "connection":
                        break;
                    case "content-length":
                        break;
                    case "content-type":
                        webRequest.ContentType = header.Value;
                        break;
                    case "expect":
                        break;
                    case "date":
                        DateTime dateValue;
                        if (DateTime.TryParse(header.Value, out dateValue))
                            webRequest.Date = dateValue;
                        break;
                    case "host":
                        webRequest.Host = header.Value;
                        break;
                    case "if-modified-since":
                        DateTime ifModifiedSinceValue;
                        if (DateTime.TryParse(header.Value, out ifModifiedSinceValue))
                            webRequest.IfModifiedSince = ifModifiedSinceValue;
                        break;
                    case "range":
                        break;
                    case "referer":
                        webRequest.Referer = header.Value;
                        break;
                    case "transfer-encoding":
                        break;
                    case "user-agent":
                        webRequest.UserAgent = header.Value;
                        break;
                    default:
                        try
                        {
                            webRequest.Headers.Add(header.Key, header.Value);
                        }
                        catch (Exception ex)
                        {
                            Logger.ErrorFormat(ex, "Error while setting header {0}={1}", header.Key, header.Value);
                            throw;
                        }
                        break;
                }
            }
        }
    }
}
