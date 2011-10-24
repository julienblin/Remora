using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.UI
{
    public interface ICommandNotifier
    {
        void CommandFinished(ICommand cmd);
    }
}
