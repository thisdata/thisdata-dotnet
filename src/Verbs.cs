namespace ThisData
{
    /// <summary>
    /// Verbs are what we use to track different types of events
    /// 
    /// Docs: http://help.thisdata.com/v1.0/docs/verbs
    /// </summary>
    public class Verbs
    {
        // Authentication
        public const string LOG_IN = "log-in";
        public const string LOG_IN_DENIED = "log-in-denied";
        public const string LOG_IN_CHALLENGE = "log-in-challenge";
        public const string LOG_OUT = "log-out";
        public const string AUTH_CHALLENGE = "authentication-challenge";
        public const string AUTH_CHALLENGE_PASS = "authentication-challenge-pass";
        public const string AUTH_CHALLENGE_FAIL = "authentication-challenge-fail";

        // Account Management
        public const string EMAIL_UPDATE = "email-update";
        public const string PASSWORD_UPDATE = "password-update";
        public const string TWO_FACTOR_DISABLE = "two-factor-disable";

        // Password Reset
        public const string PASSWORD_RESET_REQUEST = "password-reset-request";
        public const string PASSWORD_RESET = "password-reset";
        public const string PASSWORD_RESET_FAIL = "password-reset-fail";
    }

    /// <summary>
    /// Deprecated. Use Verbs instead
    /// </summary>
    public class AuditMessageVerbs : Verbs
    {
    }
}