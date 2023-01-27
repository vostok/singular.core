using Vostok.Clusterclient.Core;

namespace Vostok.Singular.Core.PathPatterns
{
    internal abstract class IdentifierFactory<T>
    {
        public T Create(IClusterClient singularClient, string zone, string service)
        {
            return Create(new SettingsProvider(singularClient, zone, service), zone, service);
        }
        
        protected abstract T Create(SettingsProvider settingsProvider, string zone, string service);
    }
}