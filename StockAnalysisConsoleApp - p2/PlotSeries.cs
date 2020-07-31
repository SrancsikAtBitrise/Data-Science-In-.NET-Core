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
        public static void PlotStockHistory(List<StockHistoryDay> stockHistory, string seriesName, string folderName)
        {

            var dateLabels = stockHistory.Select(x => x.Date.ToString());
            DateTime startDate  = stockHistory.Select(x => x.Date).Min();

            PropertyInfo prop = typeof(StockHistoryDay).GetProperty(seriesName);
            double[] valSeries = stockHistory.Select(x => Convert.ToDouble(prop.GetValue(x))).ToArray();

            var plt = new ScottPlot.Plot(1000, 700);

            plt.Ticks(dateTimeX: true, fontName: "Cascadia Mono");
            plt.Title(seriesName, fontName: "Segoe UI Light", color: Color.Black);

            // TODO missing weekends should be skipped
            plt.SetCulture(shortDatePattern: "M\\/dd");

            // grids at every 5 days
            plt.Grid(xSpacing: 5, xSpacingDateTimeUnit: ScottPlot.Config.DateTimeUnit.Day);

            plt.PlotSignal(valSeries, xOffset: startDate.ToOADate(), color: Color.LightSeaGreen, lineWidth: 3);
            System.IO.Directory.CreateDirectory(folderName);

            plt.SaveFig($"./{folderName}/{seriesName}.png");
        }

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
    }
}