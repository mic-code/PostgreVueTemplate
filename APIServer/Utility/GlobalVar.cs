namespace Utility;

public static class GlobalVar
{
    public static bool IsDev;

    //Server
    public static string ServiceName;
    public static string Endpoint;
    public static string Key;

    //JWT
    public static string TokenKey;
    public static string Issuer;

    //DB
    public static string DBConn;
    public static string DBName;
    public static string DBDebug;

    //MongoSink
    public static string SinkConnString;
    public static string SinkCollection;

    //S3FileService
    public static string S3AccessKey;
    public static string S3SecretKey;
    public static string S3BucketName;
    public static string S3EndpointString;
    public static string S3CdnEndpoint;
    public static bool S3Accelerate;

    //SES
    public static string SESAccessKey;
    public static string SESSecretKey;
    public static string SESEndpointString;

    //AI
    public static string AIEndpoint;
    public static string AIKey;

    //Discord
    public static string DiscordWebhook;

    public static string LogCollectionName = "LogCollection";

    public static string WorkerAPIKey = "kfuwehagoianekfkeusrhguiahenhbndkhfbliawuhgoihewt";

    static ConfigurationManager config;

    public static void Init(ConfigurationManager config)
    {
        GlobalVar.config = config;

        //Server
        ServiceName = ReadVar("Server:Name", "SERVICE_NAME");
        Endpoint = ReadVar("Server:Endpoint", "SERVER_ENDPOINT");
        Key = ReadVar("Server:Key", "SERVER_KEY");

        //JWT
        TokenKey = ReadVar("Jwt:Key", "JWT_KEY");
        Issuer = ReadVar("Jwt:Issuer", "JWT_ISSUER");

        //DB
        DBConn = ReadVar("DB:ConnectionString", "DB_CONN");
        DBName = ReadVar("DB:DatabaseName", "DB_NAME");
        DBDebug = ReadEnv("DB_DEBUG");

        //MongoSink
        SinkConnString = ReadVar("MongoSink:ConnectionString", "MONGO_SINK_CONN");
        SinkCollection = ReadVar("MongoSink:CollectionName", "MONGO_SINK_COLLECTION");

        //S3FileService
        S3AccessKey = ReadVar("AWSS3:accessKey", "S3_ACCESSKEY");
        S3SecretKey = ReadVar("AWSS3:secretKey", "S3_SECRETKEY");
        S3BucketName = ReadVar("AWSS3:bucketName", "S3_BUCKET");
        S3EndpointString = ReadVar("AWSS3:endpoint", "S3_ENDPOINT");
        S3CdnEndpoint = ReadVar("AWSS3:cdnEndpoint", "S3_CDN_ENDPOINT");
        S3Accelerate = ReadBool("AWSS3:accelerate", "S3_ACCELERATE");

        //SES
        SESAccessKey = ReadVar("AWSSES:accessKeyId");
        SESSecretKey = ReadVar("AWSSES:accessKeySecret");
        SESEndpointString = ReadVar("AWSSES:endpoint");

        //AI
        AIEndpoint = ReadVar("AI:Endpoint", "AI_ENDPOINT");
        AIKey = ReadVar("AI:Key", "AI_KEY");

        //Discord
        DiscordWebhook = ReadVar("Discord:Webhook", "DISCORD_WEBHOOK");
    }

    static string ReadEnv(string enviromentName)
    {
        return Environment.GetEnvironmentVariable(enviromentName);
    }

    static string ReadVar(string settingName)
    {
        return config[settingName];
    }

    static string ReadVar(string settingName, string enviromentName)
    {
        var settingString = config[settingName];
        var envString = Environment.GetEnvironmentVariable(enviromentName);
        if (envString != null)
            settingString = envString;

        return settingString;
    }

    static bool ReadBool(string settingName, string enviromentName)
    {
        var settingString = config[settingName];
        var envString = Environment.GetEnvironmentVariable(enviromentName);
        if (envString != null)
            settingString = envString;

        return settingString != null && settingString.ToUpper() == "TRUE";
    }
}
