using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfService
{
    public class HelloWorldService : IHelloWorldService
    {
        public string Hello(string name)
        {
            return string.Format("Hello, {0}", name);
        }
    }
}
