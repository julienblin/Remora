﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Remora.Core;

namespace Remora.Pipeline
{
    /// <summary>
    /// Participant in a <see cref="IRemoraOperation"/>
    /// </summary>
    public interface IPipelineComponent
    {
        void Proceed(IPipelineComponentInvocation invocation);
    }
}
