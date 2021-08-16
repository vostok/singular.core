using System;
using System.Collections.Generic;
using Vostok.Clusterclient.Core.Model;
using Vostok.Commons.Time;
using Vostok.Configuration.Primitives;
using Vostok.Singular.Core.PathPatterns.BlackList.Settings;
using Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules.Settings;

namespace Vostok.Singular.Core
{
    [Serializable]
    public class SingularSettings
    {
        public WsDefaultsSettings WsDefaults = new WsDefaultsSettings();

        public WsClientSettings WsClient = new WsClientSettings();

        public WsServerSettings WsServer = new WsServerSettings();

        public DefaultsSettings Defaults = new DefaultsSettings();

        public RequestTransformationSettings RequestTransformation = new RequestTransformationSettings();

        public ResponseTransformationSettings ResponseTransformation = new ResponseTransformationSettings();

        public ResponseStreamingSettings ResponseStreaming = new ResponseStreamingSettings();

        public BodyLimitsSettings BodyLimits = new BodyLimitsSettings();

        public RequestStreamingSettings RequestStreaming = new RequestStreamingSettings();

        public ClientSettings Client = new ClientSettings();

        public NonIdempotencySignsSettings NonIdempotencySigns = new NonIdempotencySignsSettings();

        public IdempotencySettings IdempotencySettings = new IdempotencySettings();

        public PathPatternSettings PathPatternSigns = new PathPatternSettings();

        public Dictionary<string, SingularSettings> SettingsAliases = new Dictionary<string, SingularSettings>();

        public LeadershipModelSettings LeadershipModel = new LeadershipModelSettings();

        public StickinessModelSettings StickinessModel = new StickinessModelSettings();

        public KestrelSettings Kestrel = new KestrelSettings();

        #region WebsocketSettings


        [Serializable]
        public class WsDefaultsSettings
        {
            public TimeSpan ConnectionTimeBudget = 30.Seconds();
        }

        [Serializable]
        public class WsClientSettings
        {
            public TimeSpan KeepAliveInterval = 2.Minutes();

            public int MaxReplicasPerRequest = 3;

            public DataSize BufferSize = 16.Kilobytes();

            public double LocalDatacenterBoostModifier = 3.0d;

            public double LocalDatacenterBoostMinWeight = 0.75d;

            public TimeSpan LocalHealthRegenerationDuration = 2.Minutes();

            public double LocalHealthUpMultiplier = 1.5d;

            public double LocalHealthDownMultiplier = 0.6d;

            public double LocalHealthMinimumValue = 0.05d;

            public TimeSpan MinTimeoutForHealthTuning = 1.Seconds();

            public bool UseLocalReplicaHealth = true;

            public bool UseDatacenterWeightModifiers = true;

            public int RetryAttemptsCount = 3;

            public TimeSpan RetryDelay = 100.Milliseconds();

            public bool DeduplicateRequestUrl = true;

            public List<int> AdditionalAcceptedCodes = new List<int>
            {
                10000,
                10001
            };

            public List<int> AdditionalRejectedCodes = new List<int>
            {
                20000,
                20001
            };
        }

        [Serializable]
        public class WsServerSettings
        {
            public TimeSpan KeepAliveInterval = 2.Minutes();
        }

        #endregion

        #region ResponseStreamingSettings

        [Serializable]
        public class ResponseStreamingSettings
        {
            public bool Enabled = true;

            public DataSize MinBodySize = 256.Kilobytes();

            public DataSize CopyBufferSize = 64.Kilobytes();
        }

        #endregion

        #region DefaultSettings

        [Serializable]
        public class DefaultsSettings
        {
            public string Zone = "default";

            public TimeSpan TimeBudget = 30.Seconds();

            public RequestPriority Priority = RequestPriority.Ordinary;
        }

        #endregion

        #region RequestTransformationSettings

        [Serializable]
        public class RequestTransformationSettings
        {
            public bool DeduplicateRequestUrl = true;

            public string UrlPrefixToCut = string.Empty;

            public string UrlPrefixToAdd = string.Empty;

            public List<RewriteSettings> RewriteSettings = new List<RewriteSettings>();
        }

        #endregion

        #region ResponseTransformationSettings

        [Serializable]
        public class ResponseTransformationSettings
        {
            public bool AddSingularReplicaHeader = true;

            public bool AddSingularBackendHeader = true;

            public bool AddHeadersWithInternalTopologyForNginxRequests = true;
        }

        #endregion

        #region BodyLimitsSettings

        [Serializable]
        public class BodyLimitsSettings
        {
            public DataSize? MaximumRequestBodySize = 4.Gigabytes();

            public DataSize? MaximumResponseBodySize = 10.Gigabytes();
        }

        #endregion

        #region ClientSettings

        [Serializable]
        public class ClientSettings
        {
            public int ConnectionAttempts = 2;

            public bool ConnectionTimeoutEnabled = true;

            public TimeSpan ConnectionTimeout = 750.Milliseconds();

            public LocalWeightSettings LocalWeightSettings = new LocalWeightSettings()
            {
                InitialWeight = 1.0d,
                MinWeight = 0.001d,
                PenaltyMultiplier = 100,
                Sensitivity = 4,
                RegenerationRatePerMinute = 0.05,
                RegenerationLag = 1.Minutes(),
                StatisticSmoothingConstant = 1.Seconds(),
                StatisticTTL = 10.Minutes(),
                WeightUpdatePeriod = 10.Seconds(),
                WeightsDownSmoothingConstant = 1.Seconds(),
                WeightsRaiseSmoothingConstant = 1.Minutes(),
                WeightsTTL = 5.Minutes()
            };

            public StrategySettings StrategySettings = new StrategySettings
            {
                RequestStrategy = "Forking",
                MaxForkingParallelism = 3,
                MaxParallelParallelism = 3,
                MinForkingDelay = 2.Seconds(),
                SequentialDivisionFactor = 3
            };

            public int MaxReplicasPerRequest = 3;

            public double MaxReplicasUsageRatio = 1.25d;

            public double AdaptiveThrottlingCriticalRatio = 2.0d;

            public double AdaptiveThrottlingProbabilityCap = 0.8d;

            public double LocalDatacenterBoostModifier = 3.0d;

            public double LocalDatacenterBoostMinWeight = 0.75d;

            public bool UseLocalReplicaHealth = true;

            public bool UseReplicaBudgeting = true;

            public bool UseAdaptiveThrottling = true;

            public bool UseDatacenterWeightModifiers = true;

            public bool UseSnitchWeights = true;

            public bool UseSnitchForkingDelay = true;

            public int SnitchForkingDelaySigmas = 5;

            public int SnitchForkingDelayMinRequests = 25;

            public bool RetryEnabled = false;

            public int RetryAttemptsCount = 3;

            public TimeSpan RetryDelayMinimumValue = 1.Seconds();

            public TimeSpan RetryDelayMaximumValue = 5.Seconds();

            public bool ValidateHttpMethod = true;

            public bool PreserveOriginalUri = true;

            public bool LogNonAsciiRequestUrls = true;

            public List<int> AdditionalAcceptedCodes = new List<int>
            {
                1000,
                1001
            };

            public List<int> AdditionalRejectedCodes = new List<int>
            {
                2000,
                2001
            };

            public bool RewriteExistingExternalUrlHeader = true;
            public bool EncodeNonAsciiHeaders = false;
            public bool ArpCacheWarmupEnabled = true;
            public bool TcpKeepAliveEnabled = true;
            public TimeSpan TcpKeepAliveTime = 3.Seconds();
            public TimeSpan TcpKeepAliveInterval = 1.Seconds();

            public List<int> RetryCodesForNotIdempotentRequests = new List<int>();
        }

        #endregion

        #region RequestStreamingSettings

        [Serializable]
        public class RequestStreamingSettings
        {
            public bool Enabled = true;

            public DataSize MinBodySize = 4.Megabytes() + 1.Bytes();

            public DataRate MinSpeedForDefaultTimeout = 10.MegabytesPerSecond();
        }

        #endregion

        #region LeadershipModelSettings

        [Serializable]
        public class LeadershipModelSettings
        {
            public bool Use = false;

            public List<int> NotLeaderCodes = new List<int>
            {
                301,
                304
            };
        }

        #endregion

        #region KestrelSettings

        [Serializable]
        public class KestrelSettings
        {
            public bool UseBigGracePeriod = false;

            public DataSize MinResponseSizeForUseBigGracePeriod = 1.Megabytes();

            public TimeSpan BigGracePeriod = 1.Minutes();

            public DataRate SmallResponseSendRate = 1.KilobytesPerSecond();
        }

        #endregion

        #region PathPatternSettings

        [Serializable]
        public class PathPatternSettings
        {
            public List<PathSettingsRule> Rules = new List<PathSettingsRule>();
        }

        #endregion

        #region StickinessModelSettings

        [Flags]
        public enum StickinessMode
        {
            Disabled = 0,
            ByCookie = 1,
            ByHeader = 1 << 1,
            ByQueryParam = 1 << 2,
        }

        [Serializable]
        public class StickinessModelSettings
        {
            public StickinessMode Mode = StickinessMode.Disabled;

            public bool SetupCookie = false;

            public string CookieName = string.Empty;

            public TimeSpan CookieMaxAge = TimeSpan.Zero;

            public string CookieDomain = string.Empty;

            public string CookiePath = string.Empty;

            public string QueryParamName = string.Empty;

            public string HeaderName = string.Empty;

            public double MinAllowableWeight = 0.05;

            public double MinPercentageOfHealthyReplicas = 0.3;
        }

        #endregion
    }
}