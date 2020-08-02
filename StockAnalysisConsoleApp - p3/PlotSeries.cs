using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using ScottPlot;

namespace StockAnalysis
{
    public class PlotSeries
    {
        public static void PlotOHLC(List<StockHistoryDay> stockHistory, string folderName, string fileName)
        {
            List<ScottPlot.OHLC> valSeriesList = new List<ScottPlot.OHLC>();

            foreach (StockHistoryDay stockHistoryDay in stockHistory)
            {
                ScottPlot.OHLC ohlc = new ScottPlot.OHLC (
                    stockHistoryDay.Open,
                    stockHistoryDay.High,
                    stockHistoryDay.Low,
                    stockHistoryDay.Close,
                    stockHistoryDay.Date
                );

                valSeriesList.Add(ohlc);
            }

            ScottPlot.OHLC[] valSeries = valSeriesList.ToArray();

            var plt = new ScottPlot.Plot(1000, 700);

            plt.Title("MSFT", fontName: "Segoe UI Light", color: Color.Black);
            plt.Ticks(dateTimeX: true, fontName: "Cascadia Mono");

            // grids at every 7 days
            plt.Grid(xSpacing: 7, xSpacingDateTimeUnit: ScottPlot.Config.DateTimeUnit.Day);
            plt.SetCulture(shortDatePattern: "M\\/dd");

            // create folder if not exists
            System.IO.Directory.CreateDirectory(folderName);

            plt.PlotOHLC(valSeries);

            plt.SaveFig($"./{folderName}/{fileName}.png");
        }

        public static void PlotMonthlyChange(List<MonthlyClosedDifference> series, string propertyName, string folderName, string fileName)
        {
            // get and convert specific property of a list of objects to array of doubles
            PropertyInfo prop = typeof(MonthlyClosedDifference).GetProperty(propertyName);
            double[] valSeries = series.Select(x => Convert.ToDouble(prop.GetValue(x))).ToArray();

            // months to x values and labels
            double[] x = DataGen.Consecutive(series.Count());
            string[] labels = series.Select(x => x.Month).ToArray();

            var plt = new ScottPlot.Plot(1000, 700);

            plt.Title(propertyName, fontName: "Segoe UI Light", color: Color.Black);
            plt.Ticks(dateTimeX: false, fontName: "Cascadia Mono");
            plt.Grid(xSpacing: 1);

            // apply custom axis tick labels
            plt.XTicks(x, labels);

            // create folder if not exists
            System.IO.Directory.CreateDirectory(folderName);

            plt.PlotBar(x, valSeries, fillColor: Color.Orange);

            plt.SaveFig($"./{folderName}/{fileName}.png");
        }
    }
}