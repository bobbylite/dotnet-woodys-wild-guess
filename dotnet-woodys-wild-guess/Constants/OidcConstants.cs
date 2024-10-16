namespace dotnet.woodyswildguess.Constants;

public class OidcConstants
{
    public static class Authorization {
        public static string GrantTypeKey = "grant_type";
        public static string GrantType = "authorization_code";
        public static string CodeVerifierKey = "challenge";
        public static string CodeVerifier = "challenge";
        public static string RedirectUriKey = "redirect_uri";
        public static string CodeKey = "code";
    }

    public static class Twitter {
        public static class Api {
            public static string Token = "token";
            public static string Version = "2";
            public static string Tweets = "tweets";
            public static class Authorization {
                public static string Protocol = "oauth2";
            }
        }
    }
}
