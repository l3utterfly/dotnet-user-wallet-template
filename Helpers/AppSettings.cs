namespace WebApi.Helpers;

public class AppSettings
{
    /// <summary>
    /// This is used to disable the exception thrown if a setting is being accessed when it has not been set
    /// </summary>
    /// <remarks>This is primarily used so we can use reflection to set the class properties for the first time</remarks>
    public static bool DisableNullCheck = false;

    public string Secret { get; set; }

    #region SendGrid settings

    private string _sendgridapikey;
    public string SendGridAPIKey
    {
        get
        {
            if (!DisableNullCheck && string.IsNullOrEmpty(_sendgridapikey)) throw new MissingFieldException("Attempted to use app settings value: SendGridAPIKey when it is not supplied. Please supply the app settings value via your appsettings.json.");

            return _sendgridapikey;
        }
        set { _sendgridapikey = value; }
    }

    private string _FromEmailAddress;
    public string FromEmailAddress
    {
        get
        {
            if (!DisableNullCheck && string.IsNullOrEmpty(_FromEmailAddress)) throw new MissingFieldException("Attempted to use app settings value: FromEmailAddress when it is not supplied. Please supply the app settings value via your appsettings.json.");

            return _FromEmailAddress;
        }
        set { _FromEmailAddress = value; }
    }

    #endregion
}