using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Input;

namespace Remora.UI.ViewModel
{
    public static class TraceViewer
    {
        private static object _lockObject = new object();
        private static Dictionary<string, ControlData> _dataCollection = new Dictionary<string, ControlData>();

        public static ControlData Open
        {
            get
            {
                lock (_lockObject)
                {
                    var Str = "Open";

                    if (!_dataCollection.ContainsKey(Str))
                    {
                        var buttonData = new ButtonData()
                        {
                            SmallImage = new Uri("/Remora.UI;component/Images/Font_16x16.png", UriKind.Relative),
                            ToolTipTitle = "Open (Ctrl+O)",
                            ToolTipDescription = "Open a directory containing trace files."
                        };
                        _dataCollection[Str] = buttonData;
                    }

                    return _dataCollection[Str];
                }
            }
        }
    }
}
