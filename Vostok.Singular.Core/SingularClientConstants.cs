using System;

namespace Vostok.Singular.Core
{
    internal static class SingularClientConstants
    {
        public const string ServicesConfigurationNamePrefix = "singular/global/services/";
        public const string EnvironmentsConfigurationNamePrefix = "singular/global/environments/";
        public const int ForkingStrategyParallelismLevel = 3;

        public static readonly TimeSpan ConnectionTimeout = TimeSpan.FromMilliseconds(250);
    }
}