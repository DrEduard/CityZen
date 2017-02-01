using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Final.Models
{
    public class PostsFiltersViewModel
    {
        public List<Post> Posts { get; set; }

        public List<Category> Categories { get; set; }

        public List<string> Cities { get; set; }

        public List<string> Streets { get; set; }

        public PostsFiltersViewModel()
        {
            using(var ctx = new ApplicationDbContext())
            {
                //Posts = ctx.Posts.ToList();
                var x = ctx.Categories.ToList();
                Categories = x.GroupBy(m => m.Name).Select(m => m.First()).ToList();
                Cities = ctx.Posts.Select(m => m.City).Distinct().ToList();
                Streets = ctx.Posts.Select(m => m.Address).Distinct().ToList();
            }
        }
    }
}