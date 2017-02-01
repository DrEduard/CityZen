using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Final.Models.Reports
{
    public class PRPMReport
    {

        public DateTime x { get; set; }

        public int y { get; set; }

        public PRPMReport(DateTime X, int Y)
        {
            x = X;
            y = Y;
        }
    }
}