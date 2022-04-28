namespace Play.Common.Settings
{
    public class MongoDbSettings
    {
        public string Host { get; set; }
        public string Port { get; set; }

        public string ConnectionString => $"mongodb://{Host}:{Port}";
    }
}