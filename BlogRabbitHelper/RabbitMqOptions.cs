namespace BlogRabbitHelper
{
    /// <summary>
    /// RabbitMq设置类
    /// </summary>
    public class RabbitMqOptions
    {
        private string HostName { get; set; }
        private int Port { get; set; }
        private string ExchangerName {  get; set; }
        private string? UserName { get; set; }
        private string? Password { get; set; }
    }
}
