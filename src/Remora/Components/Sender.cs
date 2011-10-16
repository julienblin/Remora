using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using Castle.Core.Logging;
using Remora.Core;
using Remora.Exceptions;
using Remora.Extensions;
using Remora.Pipeline;

namespace Remora.Components
{
    public class Sender : IPipelineComponent
    {
        private ILogger _logger = NullLogger.Instance;
        /// <summary>
        /// Logger
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        private static readonly Regex HttpSchemeRx = new Regex("^http(s)?$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

        public void Proceed(IPipelineComponentInvocation invocation)
        {
            var operation = invocation.Operation;

            if (Logger.IsDebugEnabled)
                Logger.DebugFormat("Preparing to send {0}...", operation);

            if (string.IsNullOrWhiteSpace(operation.OutgoingRequest.Uri))
            {
                throw new UnknownDestinationException(string.Format("Unable to send {0}: no destination uri is defined. Either use a rewrite attribute on the pipeline or create a custom IPipelineComponent that will determine the destination uri.", operation));
            }

            Uri destinationUri;
            try
            {
                destinationUri = new Uri(operation.OutgoingRequest.Uri);
            }
            catch (Exception ex)
            {
                throw new InvalidDestinationUriException(string.Format("The destination uri for {0} is not a valid uri: {1}.", operation, operation.OutgoingRequest.Uri), ex);
            }

            if (!HttpSchemeRx.IsMatch(destinationUri.Scheme))
            {
                throw new InvalidDestinationUriException(string.Format("The destination uri for {0} is not a valid uri: {1}. Remora supports only http(s) destinations.", operation, operation.OutgoingRequest.Uri));
            }

            HttpWebResponse response;
            try
            {
                var webRequest = (HttpWebRequest)WebRequest.Create(destinationUri);
                webRequest.Method = operation.OutgoingRequest.Method ?? "POST";
                SetHttpHeaders(operation.OutgoingRequest, webRequest);

                if (operation.OutgoingRequest.Data != null)
                {
                    webRequest.ContentLength = operation.OutgoingRequest.Data.Length;
                    using (var requestStream = webRequest.GetRequestStream())
                    {
                        requestStream.Write(operation.OutgoingRequest.Data, 0, operation.OutgoingRequest.Data.Length);
                    }
                }

                response = (HttpWebResponse)webRequest.GetResponse();
            }
            catch (Exception ex)
            {
                throw new SendException(string.Format("There as been an error while sending {0} to {1}.", operation, destinationUri), ex);
            }

            operation.IncomingResponse.StatusCode = (int) response.StatusCode;
            operation.OutgoingResponse.StatusCode = (int) response.StatusCode;

            foreach (var header in response.Headers.AllKeys)
            {
                operation.IncomingResponse.HttpHeaders.Add(header, response.Headers[header]);
                operation.OutgoingResponse.HttpHeaders.Add(header, response.Headers[header]);
            }

            using (var stream = response.GetResponseStream())
            {
                operation.IncomingResponse.Data = stream.ReadFully();
                operation.OutgoingResponse.Data = operation.IncomingResponse.Data;
            }
        }

        private static void SetHttpHeaders(IRemoraRequest remoraRequest, HttpWebRequest webRequest)
        {
            foreach (var header in remoraRequest.HttpHeaders)
            {
                switch (header.Value.Trim().ToLowerInvariant())
                {
                    case "accept":
                        webRequest.Accept = header.Value;
                        break;
                    case "connection":
                        // TODO
                        break;
                    case "content-length":
                        break;
                    case "content-type":
                        webRequest.ContentType = header.Value;
                        break;
                    case "expect":
                        webRequest.Expect = header.Value;
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
                        // TODO
                        break;
                    case "referer":
                        webRequest.Referer = header.Value;
                        break;
                    case "transfer-encoding":
                        webRequest.TransferEncoding = header.Value;
                        break;
                    case "user-agent":
                        webRequest.UserAgent = header.Value;
                        break;
                    default:
                        webRequest.Headers.Add(header.Key, header.Value);
                        break;
                }
            }
        }
    }
}
