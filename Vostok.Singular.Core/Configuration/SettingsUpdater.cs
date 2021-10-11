using System;
using System.IO;
using System.Threading.Tasks;
using Vostok.Clusterclient.Core;
using Vostok.Clusterclient.Core.Model;
using Vostok.Configuration.Binders;
using Vostok.Configuration.Sources.Json;
using Vostok.Singular.Core.Configuration;

namespace Vostok.Singular.Core.PathPatterns
{
    internal class SettingsUpdater
    {
        private readonly DefaultSettingsBinder settingsBinder = new DefaultSettingsBinder();
        private readonly IClusterClient singularClient;

        public SettingsUpdater(IClusterClient singularClient)
        {
            this.singularClient = singularClient;
        }

        public async Task<SettingsUpdaterResult> UpdateAsync(string environment, string service, SettingsUpdaterResult previousResult = null)
        {
            var request = Request.Get("_settings/versioned")
                .WithAdditionalQueryParameter("service", service)
                .WithAdditionalQueryParameter("environment", environment)
                .WithAdditionalQueryParameter("currentVersion", previousResult?.Version)
                .WithAdditionalQueryParameter("isApiVersion", previousResult?.IsApiVersion);

            var clusterResult = await singularClient.SendAsync(request).ConfigureAwait(false);
            var response = clusterResult.Response;
            var content = await ReadContentAsync(response).ConfigureAwait(false);
            if (response.Code == ResponseCode.NotModified)
            {
                if (previousResult == null)
                    throw new Exception("Received unexpected 'NotModified' response from server although no current version was sent in request.");
                return new SettingsUpdaterResult(false, previousResult.Version, previousResult.IsApiVersion, null);
            }
            if (response.Code == ResponseCode.Ok)
            {
                var versionedRawSettings = settingsBinder.Bind<VersionedSettings>(JsonConfigurationParser.Parse(content));
                if (versionedRawSettings == null)
                    throw new Exception($"Received content was bind to `Null`. Content: {content}");
                return new SettingsUpdaterResult(true, versionedRawSettings.Version, versionedRawSettings.IsApiVersion, versionedRawSettings.Settings);
            }

            var errorMessage = $"Failed to update idempotency settings from singular for '{service}' service in '{environment}' environment. Response code = {response.Code}.";
            if (!string.IsNullOrEmpty(content))
                errorMessage += $" Error = {content}";
            throw new Exception(errorMessage);
        }

        private static async Task<string> ReadContentAsync(Response response)
        {
            if (!response.HasContent)
                return response.Code == ResponseCode.Ok
                    ? throw new Exception("Received an empty 200 response from server.")
                    : string.Empty;

            string content;
            if (!response.HasStream)
                content = response.Content.ToString();
            else
                using (var reader = new StreamReader(response.Stream))
                    content = await reader.ReadToEndAsync().ConfigureAwait(false);
            return content;
        }
    }
}