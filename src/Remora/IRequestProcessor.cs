using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Remora
{
    public interface IRequestProcessor
    {
        void Process(HttpContext context);
    }
}
