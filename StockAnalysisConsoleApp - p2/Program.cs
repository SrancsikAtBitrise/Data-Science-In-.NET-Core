using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace StockAnalysis
{
    class Program
    {
        // System.CommandLine.DragonFruit generates option from XML comment
        /// <param name="startDate"> An option whose argument is parsed as an string </param>
        /// <param name="endDate"> An option whose argument is parsed as an string </param>
        /// <param name="pHistory"> An option whose argument is parsed as an int </param>
        /// <param name="confidence"> An option whose argument is parsed as an int </param>
        // TODO Add dynamic defaults, e.g. last complete 12 weeks
        static async Task<int> Main()
        {

            var builder = new HostBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHttpClient<IEXCloudService>();
                }).UseConsoleLifetime();

            var host = builder.Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {

                    var iexcloudService = services.GetRequiredService<IEXCloudService>();
                    var stockHistory = await iexcloudService.GetStockHistory("pk_dc6464269c8b40dc99a9942bab84430c");


                    foreach (StockHistoryDay stockHistoryDay in stockHistory)
                    {
                        Console.WriteLine(
                            $"label:\t {stockHistoryDay.Label} \nopen:\t {stockHistoryDay.Open} \nvolume:\t {stockHistoryDay.Volume}\n"
                        );
                    }

                    // Iterate over properties and draw charts
                    Type type = typeof(StockHistoryDay);
                    PropertyInfo[] properties = type.GetProperties();

                    foreach (PropertyInfo property in properties) {
                        if (property.PropertyType == typeof(Double))
                        {
                            PlotSeries.PlotStockHistory(stockHistory, property.Name, "charts");
                        }
                    }

                    PlotSeries.PlotOHLC(stockHistory, "charts", "OHLC");

                }

                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();

                    logger.LogError(ex, "An error occurred.");

                    Console.WriteLine("ERROR");
                    Console.WriteLine(ex.Message);
                }
            }

            Console.WriteLine("Hit Enter to exit.");
            Console.ReadLine();
            return 0;
        }
    }
}
