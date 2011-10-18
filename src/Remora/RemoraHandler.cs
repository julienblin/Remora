using System;
using System.Web;

namespace Remora
{
    public class RemoraHandler : IHttpAsyncHandler
    {
        #region IHttpAsyncHandler Members

        public void ProcessRequest(HttpContext context)
        {
            throw new InvalidOperationException("This http handler is asynchronous.");
        }

        public bool IsReusable
        {
            get { return true; }
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            Bootstraper.Init();

            var result = new RemoraAsyncResult(cb, context, extraData, Bootstraper.Container);
            result.Process();
            return result;
        }

        public void EndProcessRequest(IAsyncResult result)
        {
        }

        #endregion
    }
}
