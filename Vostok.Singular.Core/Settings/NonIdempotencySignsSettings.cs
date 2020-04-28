using System;
using System.Collections.Generic;

namespace Vostok.Singular.Core.Settings
{
    [Serializable]
    internal class NonIdempotencySignsSettings
    {
        public List<NonIdempotencySignSettings> Signs = new List<NonIdempotencySignSettings>();
    }
}