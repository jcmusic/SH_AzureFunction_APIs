using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SafeHarborFunctionApp.Services;
using SH.BLL;
using SH.Models.Models;

[assembly: FunctionsStartup(typeof(SafeHarborFunctionApp.Startup))]

namespace SafeHarborFunctionApp;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddHttpClient();
        //builder.Services.AddSingleton<IImageService>((s) => new ImageService());
        builder.Services.AddScoped<IImageService, ImageService>();
        builder.Services.AddScoped<ICustomerLogic, CustomerLogic>();
        builder.Services.AddScoped<ICustomerRepo, SH.DAL.Sqlite.CustomerRepo>();

        //builder.Services.AddSingleton<ILoggerProvider, MyLoggerProvider>();
    }

}