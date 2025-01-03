namespace dotnet.woodyswildguess.Constants;

public class OidcConstants
{
    public static class Authorization {
        public static string GrantTypeKey = "grant_type";
        public static string GrantType = "authorization_code";
        public static string CodeVerifierKey = "code_verifier";
        public static string CodeVerifier = "challenge";
        public static string RedirectUriKey = "redirect_uri";
        public static string CodeKey = "code";
        public static string ScopeKey = "scope";
        public static string StateKey = "state";
        public static string CodeChallengeKey = "code_challenge";
        public static string CodeChallengeMethodKey = "code_challenge_method";
        public static string ResponseTypeKey = "response_type";
        public static string ClientIdKey = "client_id";
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
