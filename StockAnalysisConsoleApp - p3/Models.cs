using System;
using ScottPlot;

namespace StockAnalysis
{
    ///<summary>
    /// Object representing a day in stock history
    ///</summary>
    public class StockHistoryDay
    {
        public DateTime Date {get; set; }
        public string Label {get; set; }
        public double Open {get; set; }
        public double Close {get; set; }
        public double High {get; set; }
        public double Low {get; set; }
        public int Volume {get; set; }
        public double ChangeOverTime {get; set; }
    }

    ///<summary>
    /// Object representing monthly differences in close figures
    ///</summary>
    public class MonthlyClosedDifference
    {
        public string Month {get; set; }
        public double Difference {get; set; }
        public double DifferencePercent {get; set; }
    }
}