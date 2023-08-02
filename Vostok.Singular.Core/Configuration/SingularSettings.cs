using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using Vostok.Clusterclient.Core.Model;
using Vostok.Configuration.Primitives;
using Vostok.Singular.Core.PathPatterns.BlackList.Settings;
using Vostok.Singular.Core.PathPatterns.Idempotency.IdempotencyControlRules.Settings;

namespace Vostok.Singular.Core.Configuration
{
    [Serializable]
    internal class SingularSettings
    {
        public WsDefaultsSettings WsDefaults = new WsDefaultsSettings();

        public WsClientSettings WsClient = new WsClientSettings();

        public WsServerSettings WsServer = new WsServerSettings();

        public TcpClientSettings TcpClient = new TcpClientSettings();

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

        public RequestRejectSettings RequestReject = new RequestRejectSettings();

        public RequestRedirectSettings RequestRedirect = new RequestRedirectSettings();

        public Dictionary<string, SingularSettings> SettingsAliases = new Dictionary<string, SingularSettings>();

        public LeadershipModelSettings LeadershipModel = new LeadershipModelSettings();

        public StickinessModelSettings StickinessModel = new StickinessModelSettings();

        public KestrelSettings Kestrel = new KestrelSettings();

        public int TcpPort;

        public TlsClientSettings TlsClient = new TlsClientSettings();

        #region TcpSettings

        [Serializable]
        public class TcpClientSettings
        {
            public DataSize? RequestProxyBufferSize;

            public DataSize? ResponseProxyBufferSize;

            public bool? ReuseAddress;

            public TimeSpan? TcpKeepAliveTime;

            public TimeSpan? TcpKeepAliveInterval;

            public double LocalDatacenterBoostModifier = 3.0d;

            public double LocalDatacenterBoostMinWeight = 0.75d;

            public double LocalHealthUpMultiplier = 1.5d;

            public double LocalHealthDownMultiplier = 0.6d;

            public double LocalHealthMinimumValue = 0.05d;

            public TimeSpan MinTimeoutForHealthTuning = TimeSpan.FromSeconds(1);

            public bool UseLocalReplicaHealth = true;

            public bool UseDatacenterWeightModifiers = true;

            public int MaxReplicasUsedPerConnection = 3;

            public int RetryAttemptsCount = 3;

            public TimeSpan RetryDelay = TimeSpan.FromMilliseconds(100);

            public TimeSpan ConnectionTimeBudget = TimeSpan.FromMilliseconds(100);

            public TimeSpan RequestTimeBudget = TimeSpan.FromSeconds(30);

            public HostingTopologyTransformSettings HostingTopologyTransform = new HostingTopologyTransformSettings();
        }

        #endregion

        #region WebsocketSettings

        [Serializable]
        public class WsDefaultsSettings
        {
            public TimeSpan ConnectionTimeBudget = TimeSpan.FromSeconds(30);
        }

        [Serializable]
        public class WsDisconnectSettings
        {
            public List<WsDisconnectPattern> Conditions = new List<WsDisconnectPattern>();
        }

        [Flags]
        public enum DisconnectTarget
        {
            StrongStickiness = 0
        }

        [Serializable]
        public class WsDisconnectPattern
        {
            public DisconnectTarget Target = DisconnectTarget.StrongStickiness;
            public WebSocketCloseStatus CloseStatus = (WebSocketCloseStatus)4000;
            public string CloseDescription = "close by singular";
        }

        [Serializable]
        public class WsClientSettings
        {
            public TimeSpan KeepAliveInterval = TimeSpan.FromMinutes(2);

            public int MaxReplicasPerRequest = 3;

            public bool TryReadFullMessage = false;

            public DataSize BufferSize = 16.Kilobytes();

            public double BufferSizeIncreaseMultiplier = 2d;

            public DataSize MaxBufferSize = 16.Kilobytes();

            public double LocalDatacenterBoostModifier = 3.0d;

            public double LocalDatacenterBoostMinWeight = 0.75d;

            public TimeSpan LocalHealthRegenerationDuration = TimeSpan.FromMinutes(2);

            public double LocalHealthUpMultiplier = 1.5d;

            public double LocalHealthDownMultiplier = 0.6d;

            public double LocalHealthMinimumValue = 0.05d;

            public TimeSpan MinTimeoutForHealthTuning = TimeSpan.FromSeconds(1);

            public bool UseLocalReplicaHealth = true;

            public bool UseDatacenterWeightModifiers = true;

            public int RetryAttemptsCount = 3;

            public TimeSpan RetryDelay = TimeSpan.FromMilliseconds(100);

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

            public HeadersTransformationSettings HeadersTransformation = new HeadersTransformationSettings();

            public HostingTopologyTransformSettings HostingTopologyTransform = new HostingTopologyTransformSettings();

            public WsDisconnectSettings DisconnectSettings = new WsDisconnectSettings();
        }

        [Serializable]
        public class WsServerSettings
        {
            public TimeSpan KeepAliveInterval = TimeSpan.FromMinutes(2);
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

            public TimeSpan TimeBudget = TimeSpan.FromSeconds(30);

            public RequestPriority Priority = RequestPriority.Ordinary;

            public List<RequestProtocol> ProtocolsBlackList = new List<RequestProtocol>();
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

            public HeadersTransformationSettings HeadersTransformation = new HeadersTransformationSettings();
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

            public TimeSpan ConnectionTimeout = TimeSpan.FromMilliseconds(750);

            public LocalWeightSettings LocalWeightSettings = new LocalWeightSettings()
            {
                InitialWeight = 1.0d,
                MinWeight = 0.001d,
                PenaltyMultiplier = 100,
                Sensitivity = 4,
                RegenerationRatePerMinute = 0.05,
                RegenerationLag = TimeSpan.FromMinutes(1),
                StatisticSmoothingConstant = TimeSpan.FromSeconds(1),
                StatisticTTL = TimeSpan.FromMinutes(10),
                WeightUpdatePeriod = TimeSpan.FromSeconds(10),
                WeightsDownSmoothingConstant = TimeSpan.FromSeconds(1),
                WeightsRaiseSmoothingConstant = TimeSpan.FromMinutes(1),
                WeightsTTL = TimeSpan.FromMinutes(5),
            };

            public StrategySettings StrategySettings = new StrategySettings
            {
                RequestStrategy = "Forking",
                MaxForkingParallelism = 3,
                MaxParallelParallelism = 3,
                MinForkingDelay = TimeSpan.FromSeconds(2),
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

            public TimeSpan RetryDelayMinimumValue = TimeSpan.FromSeconds(1);

            public TimeSpan RetryDelayMaximumValue = TimeSpan.FromSeconds(5);

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
            public TimeSpan TcpKeepAliveTime = TimeSpan.FromSeconds(3);
            public TimeSpan TcpKeepAliveInterval = TimeSpan.FromSeconds(1);

            public List<int> RetryCodesForNotIdempotentRequests = new List<int>();

            public LoggingOptions Logging = new LoggingOptions();

            public bool RewriteHostHeaderWithTargetHostValue = false;

            public HostingTopologyTransformSettings HostingTopologyTransform = new HostingTopologyTransformSettings();
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

            public TimeSpan BigGracePeriod = TimeSpan.FromMinutes(1);

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

        #region MatchRuleSettings

        [Flags]
        public enum MatchTarget
        {
            Url = 0,
            Header = 1,
            Query = 2,
            Method = 3
        }

        #endregion

        #region RequestRejectSettings

        [Serializable]
        public class RequestRejectSettings
        {
            public List<RejectPattern> Rules = new List<RejectPattern>();
        }

        [Serializable]
        public class RejectPattern
        {
            public MatchPattern RequestMatchPattern;
            public int RejectCode;
        }

        #endregion

        #region RequestRedirectSettings

        [Serializable]
        public class RequestRedirectSettings
        {
            public List<RedirectPattern> Rules = new List<RedirectPattern>();
        }

        [Serializable]
        public class RedirectPattern
        {
            public MatchPattern RequestMatchPattern;
            public string DestinationService;
            public string DestinationZone;
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

        #region TlsSettings

        [Serializable]
        public class TlsClientSettings
        {
            public bool EnableCertificateValidation;
            public List<string> CertificateThumbprintsWhitelist = new List<string>();
            public List<string> CertificateThumbprintsBlacklist = new List<string>();
            // handshake timeout?
        }

        #endregion

        [Serializable]
        public class LoggingOptions
        {
            public bool LogRequestDetails { get; set; } = false;

            public bool LogResultDetails { get; set; } = false;

            public bool LogReplicaRequests { get; set; } = true;

            public bool LogReplicaResults { get; set; } = true;
        }
    }
}