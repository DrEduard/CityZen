using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Final.Models
{
    public class PostViewModel
    {
        public Post Post { get; set; }

        public List<Category> Categories { get; set; }
    }
}