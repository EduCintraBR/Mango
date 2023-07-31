using Mango.Services.EmailAPI.Messaging;

namespace Mango.Services.EmailAPI.Extension
{
    public static class ApplicationBuilderExtensions
    {
        private static IAzureServiceBusConsumer ServiceBusConsumer { get; set; }

        public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder builder)
        {
            ServiceBusConsumer = builder.ApplicationServices.GetService<IAzureServiceBusConsumer>();
            var hostApplicationLifetime = builder.ApplicationServices.GetService<IHostApplicationLifetime>();

            hostApplicationLifetime.ApplicationStarted.Register(OnStart);
            hostApplicationLifetime.ApplicationStopping.Register(OnStop);

            return builder;
        }

        private static void OnStop()
        {
            ServiceBusConsumer.Stop();
        }

        private static void OnStart()
        {
            ServiceBusConsumer.Start();
        }
    }
}
