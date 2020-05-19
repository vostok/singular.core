namespace Vostok.Singular.Core.Identifier
{
    /// <summary>
    /// ICL - Idempotency Control List, аналог ACL (Access Control List)
    /// </summary>
    internal class IclResolver
    {
        public IclResolver(
            IIclCache iclCache
            )
        {
        }

        public bool IsIdempotent(string method, string path)
        {
            return true;
        }
    }
}