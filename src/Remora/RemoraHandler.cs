using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using log4net;

namespace Remora
{
    public class RemoraHandler : IHttpAsyncHandler
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RemoraHandler).Name);

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback callback, object extraData)
        {
            if(Logger.IsDebugEnabled)
                Logger.DebugFormat("BeginProcessRequest for {0}", context.Request.Url);
            var result = new RemoraAsyncResult(callback, context, extraData);
            result.Process();
            return result;
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            if (Logger.IsDebugEnabled)
            {
                var asyncResult = (RemoraAsyncResult) result;
                Logger.DebugFormat("EndProcessRequest for {0}", asyncResult.Context.Request.Url);
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            throw new InvalidOperationException("This http handler is asynchronous.");
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}
