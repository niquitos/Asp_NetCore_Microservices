namespace Play.Frontend.Settings
{
	public class ServiceSettingsBase
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public override string ToString() => $"https://{Host}:{Port}";
    }
}