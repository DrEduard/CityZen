using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Final.Models
{
    public class MostFollowedReportsData
    {
        public int y { get; set; }

        public string label { get; set; }

        public MostFollowedReportsData()
        {

        }

        public MostFollowedReportsData(int y, string label)
        {
            this.y = y;
            this.label = label;
        }
    }
}