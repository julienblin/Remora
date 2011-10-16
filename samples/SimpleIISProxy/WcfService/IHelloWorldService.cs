using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfService
{
    [ServiceContract]
    public interface IHelloWorldService
    {
        [OperationContract]
        string Hello(string name);
    }
}
