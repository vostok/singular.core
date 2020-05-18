<<<<<<< HEAD:Vostok.Singular.Core/Idempotency/Identifier/IdempotencyIdentifier.cs
﻿using System;

namespace Vostok.Singular.Core.Idempotency.Identifier
=======
﻿namespace Vostok.Singular.Core.Identifier
>>>>>>> Move black list logic to BlackListIdempotencyResolver:Vostok.Singular.Core/Identifier/IdempotencyIdentifier.cs
{
    internal class IdempotencyIdentifier : IIdempotencyIdentifier
    {
        private readonly BlackListIdempotencyResolver blackListIdempotencyResolver;

        public IdempotencyIdentifier(BlackListIdempotencyResolver blackListIdempotencyResolver)
        {
            this.blackListIdempotencyResolver = blackListIdempotencyResolver;
        }

        public bool IsIdempotent(string method, string path)
        {
            if (!blackListIdempotencyResolver.IsIdempotent(method, path))
            {
                return false;
            }

            return true;
        }
    }
}