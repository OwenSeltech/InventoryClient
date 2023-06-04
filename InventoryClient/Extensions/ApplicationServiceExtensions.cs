namespace InventoryClient.Extensions
{
    public static class ApplicationServiceExtensions
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
		{
			var apiSettings = config.GetSection("ApiSettings");
			var baseUrl = apiSettings.GetValue<string>("BaseUrl");

			services.AddHttpClient("MyApiClient", client =>
			{
				client.BaseAddress = new Uri(baseUrl);
			});
			return services;
		}
	}
}
