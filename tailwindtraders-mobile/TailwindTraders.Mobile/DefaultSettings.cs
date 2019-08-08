
namespace TailwindTraders.Mobile.Features.Settings
{
    public static class DefaultSettings
    {
        public static string Electrical = "Electrical";
        public static string Hardware = "Hardware";
        public static string Tiles = "Tiles";
        public static string Hinges = "Hinges";

        public const string ApiAuthorizationHeader = "Authorization";

        public static string AccessToken = string.Empty;

        public const string AppCenterAndroidSecret = "** ANDROID APP CENTER SECRET HERE **";

        public const string AppCenteriOSSecret = "dfa7ebd6-23a1-43e2-9fe0-c3129e7f16b9";

        public const bool UseFakeAPIs = false;

        public const bool UseFakeAuthentication = DebugMode;

        public const bool ForceAutomaticLogin = false;

        public const bool BreakNetworkRandomly = false;

        public const bool AndroidDebuggable = DebugMode;

        public const bool UseDebugLogging = DebugMode;

        public const bool EnableARDiagnostics = DebugMode;

        public const bool DebugMode =
#if DEBUG 
            true;
#else
            false;
#endif
        
        public static string RootApiUrl { get; set; } = "https://bemaproduct.azurewebsites.net/api";

        public static string RootProductsWebApiUrl { get; set; } = "https://bemaproduct.azurewebsites.net/api";
    }
}
