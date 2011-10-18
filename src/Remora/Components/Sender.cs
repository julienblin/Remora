using System;
using System.Diagnostics.Contracts;
using System.Net;
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
        public const string SenderComponentId = @"sender";

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

        public override void BeginAsyncProcess(IRemoraOperation operation, Action<bool> callback)
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
            webRequest.Method = operation.Request.Method ?? "POST";
            SetHttpHeaders(operation.Request, webRequest);

            WriteData(operation, webRequest);

            webRequest.BeginGetResponse((result) =>
            {
                webRequest = (HttpWebRequest)result.AsyncState;
                try
                {
                    var response = (HttpWebResponse)webRequest.EndGetResponse(result);
                    ReadResponse(operation, response);
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

        protected virtual void ReadResponse(IRemoraOperation operation, HttpWebResponse response)
        {
            operation.Response.StatusCode = (int)response.StatusCode;

            foreach (var header in response.Headers.AllKeys)
            {
                operation.Response.HttpHeaders.Add(header, response.Headers[header]);
            }

            using (var stream = response.GetResponseStream())
            {
                operation.Response.Data = stream.ReadFully(_config.MaxMessageSize);
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
