using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Remora
{
    public class RemoraHandler : IHttpAsyncHandler
    {
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
            var result = new RemoraAsyncResult(cb, context, extraData);
            result.Process();
            return result;
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            
        }
    }
}
