using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Final.Models.Reports
{
    public class ActivityReportViewModel
    {
        public int y { get; set; }

        public string legendText { get; set; }

        public string label { get; set; }

        public ActivityReportViewModel()
        {

        }

        public ActivityReportViewModel(int y, string lt, string l)
        {
            this.y = y;
            this.legendText = lt;
            this.label = l;
        }
    }
}