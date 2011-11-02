﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.Host.Configuration
{
    public interface IRemoraHostConfig
    {
        IServiceConfig ServiceConfig { get; }

        IEnumerable<IListenerConfig> ListenerConfigs { get; }
    }
}
