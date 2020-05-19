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
        private readonly IclResolver iclResolver;

        public IdempotencyIdentifier(
            BlackListIdempotencyResolver blackListIdempotencyResolver,
            IclResolver iclResolver
        )
        {
            this.blackListIdempotencyResolver = blackListIdempotencyResolver;
            this.iclResolver = iclResolver;
        }

        public bool IsIdempotent(string method, string path)
        {
            return blackListIdempotencyResolver.IsIdempotent(method, path) && iclResolver.IsIdempotent(method, path);
        }
    }
}