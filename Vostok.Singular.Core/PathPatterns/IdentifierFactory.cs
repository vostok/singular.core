using Vostok.Clusterclient.Core;

namespace Vostok.Singular.Core.PathPatterns
{
    internal abstract class IdentifierFactory<T>
    {
        public T Create(IClusterClient singularClient, string zone, string service)
        {
            return Create(new SettingsProvider(singularClient, zone, service));
        }
        
        protected abstract T Create(ISettingsProvider settingsProvider);
    }
}