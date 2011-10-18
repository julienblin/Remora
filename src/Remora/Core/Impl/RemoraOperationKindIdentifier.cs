using System;
using System.Diagnostics.Contracts;
using Castle.Core.Logging;

namespace Remora.Core.Impl
{
    public class RemoraOperationKindIdentifier : IRemoraOperationKindIdentifier
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

        #region IRemoraOperationKindIdentifier Members

        public RemoraOperationKind Identify(IRemoraOperation operation)
        {
            if(operation == null) throw new ArgumentNullException("operation");
            Contract.EndContractBlock();

            if(Logger.IsDebugEnabled)
                Logger.DebugFormat("Identifying operation {0}...", operation);

            if (operation.Request.HttpHeaders.ContainsKey("SOAPAction"))
            {
                if (Logger.IsDebugEnabled)
                    Logger.DebugFormat("Operation {0} is kind of {1}.", operation, RemoraOperationKind.Soap);
                return RemoraOperationKind.Soap;
            }


            if(Logger.IsDebugEnabled)
                Logger.DebugFormat("Unable to identify operation {0}.", operation);
            return RemoraOperationKind.Unknown;
        }

        #endregion
    }
}
