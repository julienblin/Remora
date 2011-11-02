using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remora.Host.Configuration
{
    public interface IServiceConfig
    {
        string ServiceName { get; }

        string DisplayName { get; }

        string Description { get; }

        ServiceConfigRunAs RunAs { get; }

        string Username { get; }

        string Password { get; }
    }

    public enum ServiceConfigRunAs
    {
        LocalService,
        LocalSystem,
        NetworkService,
        User
    }
}
