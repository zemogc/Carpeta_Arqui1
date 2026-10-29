using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NorthWind.ConsoleApp.Services;

// Inyección de dependencias: se registran varias implementaciones de IUserActionWriter
// y los servicios reciben todas con IEnumerable<IUserActionWriter>.
var Builder = Host.CreateApplicationBuilder();
Builder.Services.AddConsoleWriter();
Builder.Services.AddDebugWriter();
Builder.Services.AddFileWriter();
Builder.Services.AddSingleton<AppLogger>();
Builder.Services.AddSingleton<ProductService>();
using var AppHost = Builder.Build();

AppLogger Logger = AppHost.Services.GetRequiredService<AppLogger>();
Logger.WriteLog("Application started.");

ProductService service = AppHost.Services.GetRequiredService<ProductService>();
service.Add("Demo", "Azúcar refinada");
