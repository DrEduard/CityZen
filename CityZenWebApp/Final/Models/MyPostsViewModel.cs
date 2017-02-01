using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Final.Models
{
    public class MyPostsViewModel
    {
        public List<Post> UpvotedPosts { get; set; }

        public List<Post> AddedPosts { get; set; }

        public MyPostsViewModel()
        {
            UpvotedPosts = new List<Post>();
            AddedPosts = new List<Post>();
        }
    }
}