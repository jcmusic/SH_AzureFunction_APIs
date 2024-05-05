using SH.DAL;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SafeHarborFunctionApp.Services;
using SH.BLL;
using SH.Models.Models;

[assembly: FunctionsStartup(typeof(SafeHarborFunctionApp.Startup))]

namespace SafeHarborFunctionApp;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        // DB Context
        builder.Services.AddDbContext<CustomerDbContext>(
            DbContextOptions => DbContextOptions.UseSqlite("CustomersDb",
                b => b.MigrationsAssembly("SH.DAL.Sqlite")));

        builder.Services.AddHttpClient();
        //builder.Services.AddSingleton<IImageService>((s) => new ImageService());
        builder.Services.AddScoped<IImageService, ImageService>();
        builder.Services.AddScoped<ICustomerLogic, CustomerLogic>();
        builder.Services.AddScoped<ICustomerRepo, SH.DAL.Sqlite.CustomerRepo>();

        //builder.Services.AddSingleton<ILoggerProvider, MyLoggerProvider>();
    }

}