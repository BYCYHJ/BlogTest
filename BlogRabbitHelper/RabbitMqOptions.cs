namespace BlogRabbitHelper
{
    /// <summary>
    /// RabbitMq设置类
    /// </summary>
    public class RabbitMqOptions
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string ExchangerName {  get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }
}
