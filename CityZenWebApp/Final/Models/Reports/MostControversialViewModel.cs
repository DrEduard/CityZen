using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Final.Models.Reports
{
    public class MostControversialViewModel
    {
        public List<Ob> top { get; set; }
        
        public List<Ob> bot { get; set; }


        public MostControversialViewModel()
        {
            top = new List<Ob>();
            bot = new List<Ob>();
        }
    }

    public class Ob
    {
        public int y { get; set; }

        public string label { get; set; }

        public Ob(int y, string l)
        {
            this.y = y;
            this.label = l;
        }
    }
}