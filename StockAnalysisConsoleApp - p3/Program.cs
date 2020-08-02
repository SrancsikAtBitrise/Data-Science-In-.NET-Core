using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;

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
                    List<StockHistoryDay> stockHistory = await iexcloudService.GetStockHistory("pk_dc6464269c8b40dc99a9942bab84430c");


                    foreach (StockHistoryDay stockHistoryDay in stockHistory)
                    {
                        Console.WriteLine(
                            $"label:\t {stockHistoryDay.Label} \nopen:\t {stockHistoryDay.Open} \nvolume:\t {stockHistoryDay.Volume}\n"
                        );
                    }

                    PlotSeries.PlotOHLC(stockHistory, "charts", "OHLC");

                    // calculate mondthly change in closed figures
                    List<MonthlyClosedDifference> closedMonthlyChange = stockHistory
                                            .GroupBy(x => x.Date.ToString("MMMM"))
                                            .Select(g => new MonthlyClosedDifference
                                            {
                                                Month = g.Key,
                                                Difference = g.Last().Close - g.First().Close,
                                                DifferencePercent = (g.Last().Close - g.First().Close) / g.First().Close * 100
                                            })
                                            .ToList();

                    PlotSeries.PlotMonthlyChange(closedMonthlyChange, "Difference", "charts", "ClosedMonthlyDifference");
                    PlotSeries.PlotMonthlyChange(closedMonthlyChange, "DifferencePercent", "charts", "ClosedMonthlyDifferencePercent");

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
