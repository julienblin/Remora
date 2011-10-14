using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Remora.Impl;

namespace Remora
{
    public class RemoraAsyncResult : IAsyncResult
    {
        private readonly AsyncCallback _callback;

        public RemoraAsyncResult(AsyncCallback callback, HttpContext context, Object state)
        {
            _callback = callback;
            Context = context;
            AsyncState = state;
        }

        public bool IsCompleted { get; private set; }
        public WaitHandle AsyncWaitHandle { get { return null; } }
        public object AsyncState { get; private set; }
        public bool CompletedSynchronously { get { return false; } }
        public HttpContext Context { get; private set; }

        public void Process()
        {
            Task.Factory.StartNew(() =>
            {
                IRequestProcessor requestProcessor = new RequestProcessor();
                requestProcessor.Process(Context);
                IsCompleted = true;
                _callback(this);
            });
        }
    }
}
