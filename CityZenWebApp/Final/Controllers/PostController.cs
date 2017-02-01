using Final.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Final.Controllers
{
    public class PostController : Controller
    {
        /// <summary>
        /// GET Post by id
        /// </summary>
        
        [Authorize]
        public ActionResult Post(int postId)
        {
            Post post = null;
            try
            {
                using (var ctx = new ApplicationDbContext())
                {
                    post = ctx.Posts.FirstOrDefault(m => m.Id == postId);
                    post.Comments = ctx.Comments.Where(m => m.PostId == postId).OrderByDescending(m => m.IsImportant == true).ThenByDescending(m => m.DateAdded).ToList();  //.OrderByDescending(m => m.DateAdded).ToList();
                }
                return View(post);
            }
            catch(Exception e)
            {
                return View("Error");
            }
            
        }

        [Authorize]
        public JsonResult GetAddressesByCity(string city)
        {
            if (!string.IsNullOrEmpty(city))
            {
                List<string> streets = null;
                using(var ctx = new ApplicationDbContext())
                {
                    streets = ctx.Posts.Where(m => m.City == city).Select(m => m.Address).Distinct().ToList();
                }
                streets.Insert(0, "--Select Street--");
                return Json(streets,JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [Authorize]
        public ActionResult _Posts(string category = "", string city = "", string address = "")
        {
            List<Post> posts = null;
            using (var ctx = new ApplicationDbContext())
            {
                if(category != "" || city != "" || (address != "" && address != "--Select Street--"))
                {
                    if (category != "")
                    {
                        posts = ctx.Posts.Where(m => m.Category == category)?.OrderByDescending(m => m.DateAdded).ToList();//.Skip(0 * 10).Take(10).ToList();
                    }
                    if (city != "" && posts != null)
                    {
                        posts = posts.Where(m => m.City == city).ToList();
                    }
                    else if(city != "" && posts == null)
                    {
                        posts = ctx.Posts.Where(m => m.City == city)?.OrderByDescending(m => m.DateAdded).ToList();//.Skip(0 * 10).Take(10).ToList();
                    }
                    if (address != "" && address != "--Select Street--" && posts != null)
                    {
                        posts = posts.Where(m => m.Address == address).ToList();
                    }
                    else if(address != "" && address != "--Select Street--" && posts == null)
                    {
                        posts = ctx.Posts.Where(m => m.Address == address)?.OrderByDescending(m => m.DateAdded).ToList();//.Skip(0 * 10).Take(10).ToList();
                    }
                }else
                {
                    posts = ctx.Posts.OrderByDescending(m => m.DateAdded).ToList();//.Skip(0 * 10).Take(10).ToList();
                }
            }

            return PartialView("_Posts",posts);
        }
        // GET: All Post
        [Authorize]
        public ActionResult AllPosts(int page = 1)
        {
            var pageIndex = page - 1;
            PostsFiltersViewModel postsFiltersVM = new PostsFiltersViewModel();
            using (var ctx = new ApplicationDbContext())
            {
                postsFiltersVM.Posts = ctx.Posts.OrderByDescending(m => m.DateAdded).ToList();//.Skip(pageIndex * 10).Take(10).ToList();
            }

            return View(postsFiltersVM);
        }

        [Authorize]
        public ActionResult AddPost()
        {
            PostViewModel postVM = new PostViewModel();
            using(var ctx = new ApplicationDbContext())
            {
                postVM.Categories = ctx.Categories.ToList();
            }
            Category cat = new Category { Id= 100000000, Name = "Other"};

            postVM.Categories.Add(cat);
            return View(postVM);
        }

        [HttpPost]
        [Authorize]
        public ActionResult AddPost(PostViewModel postViewModel, string newCategory = "")
        {
            if (postViewModel.Post.Category == "--Select Category--")
                ModelState.AddModelError("Category", "You have to select a category.");
            if (postViewModel.Post.Category == "Other")
            {
                if (string.IsNullOrEmpty(newCategory))
                {
                    ModelState.AddModelError("Category", "You have to select a category.");
                }else
                {
                    Category cat = new Category { Name = newCategory };
                    using (var c = new ApplicationDbContext())
                    {
                        c.Categories.Add(cat);
                        c.SaveChanges();
                    }
                    postViewModel.Post.Category = newCategory;
                }
               
            }
                

            if (ModelState.IsValid)
            {
                try
                {
                    postViewModel.Post.DateAdded = DateTime.Now;
                    HttpPostedFileBase file = Request?.Files?[0];
                    if(file != null && file.ContentLength > 0)
                    {
                        byte[] imageBytes = ConvertToBytes(file);
                        postViewModel.Post.Photo = CompressImage(imageBytes);
                        postViewModel.Post.PhotoExtension = file.ContentType;
                    }
                    postViewModel.Post.UserName = User.Identity.GetUserName();
                    using (var ctx = new ApplicationDbContext())
                    {
                        ctx.Posts.Add(postViewModel.Post);
                        postViewModel.Categories = ctx.Categories.ToList();
                        ctx.SaveChanges();
                    }
                    return RedirectToAction("AllPosts","Post");
                }
                catch (Exception e)
                {
                    return View("Error");
                }
            }
            else
            {
                using (var ctx = new ApplicationDbContext())
                {
                    postViewModel.Categories = ctx.Categories.ToList();
                }
                return View(postViewModel);
            }
        }
        
        [HttpPost]
        public string UpvotePost(int postId)
        {
            try
            {
                var username = User.Identity.GetUserName();
                using (var ctx = new ApplicationDbContext())
                {
                    var userPostUpvote = ctx.UserPostUpvotes.FirstOrDefault(m => m.UserName == username && m.PostId == postId);
                    if (userPostUpvote == null)
                    {
                        var post = ctx.Posts.First(m => m.Id == postId);
                        post.Upvotes += 1;

                        UserPostUpvotes up = new UserPostUpvotes();
                        up.PostId = postId;
                        up.UserName = username;
                        ctx.UserPostUpvotes.Add(up);

                        ctx.SaveChanges();

                        return "ok";
                    }
                    else
                    {
                        return "You have already upvoted this post.";
                    }
                }
            }
            catch (Exception e)
            {
                string exStr = e.InnerException.Message;
                return "Something went wrong, please try again.";
            }
        }
        [HttpPost]
        public string Downvote(int postId)
        {
            try
            {
                var username = User.Identity.GetUserName();
                using (var ctx = new ApplicationDbContext())
                {
                    var userPostUpvote = ctx.UserPostUpvotes.FirstOrDefault(m => m.UserName == username && m.PostId == postId);
                    if (userPostUpvote != null)
                    {
                        ctx.UserPostUpvotes.Remove(userPostUpvote);
                        Post post = ctx.Posts.FirstOrDefault(m => m.Id == postId);
                        post.Upvotes -= 1;
                        ctx.SaveChanges();
                        return "ok";
                    }
                    else
                    {
                        return "You have already downvoted this post.";
                    }
                }
            }
            catch (Exception e)
            {
                string exStr = e.InnerException.Message;
                return "Something went wrong, please try again.";
            }
        }


        [HttpPost]
        [Authorize]
        public ActionResult AddComment(int postId, string text)
        {
            Comment comment = new Comment();
            List<Comment> comments = null;
            comment.PostId = postId;
            comment.Text = text;
            comment.DateAdded = DateTime.Now;
            comment.Username = User.Identity.GetUserName();
            if (User.IsInRole("Admin") || User.IsInRole("Authority"))
            {
                comment.IsImportant = true;
            }else
            {
                comment.IsImportant = false;
            }
            using (var ctx = new ApplicationDbContext())
            {
                ctx.Comments.Add(comment);
                ctx.SaveChanges();
                comments = ctx.Comments.Where(m => m.PostId == postId).OrderByDescending(m => m.IsImportant == true).ThenByDescending(m => m.DateAdded).ToList();  
            }
            return PartialView("_Comments", comments);
        }

        [Authorize]
        public ActionResult MarkPostResolved(int postId, string appOrRes)
        {
            Post post = null;
            using(var ctx = new ApplicationDbContext())
            {
                post = ctx.Posts.Where(m => m.Id == postId).First();
                if(appOrRes == "app")
                {
                    post.IsResolved = false;
                    post.IsApproved = true;
                }
                if(appOrRes == "res")
                {
                    post.IsResolved = true;
                    post.IsApproved = false;
                }
                post.DateResolved = DateTime.Now;
                post.ResolverUsername = User.Identity.GetUserName();
                ctx.SaveChanges();
            }
            return View("Post", new { postId = postId });
        }
        [Authorize]
        [HttpPost]
        public void DeletePost(int postId)
        {
            Post post = null;
            using(var c = new ApplicationDbContext())
            {
                post = c.Posts.FirstOrDefault(m => m.Id == postId);
                c.Posts.Remove(post);
               
                c.SaveChanges();
            }
        }
        //[Authorize]
        //public ActionResult MarkPostApproved(int postId)
        //{
        //    Post post = null;
        //    using (var ctx = new ApplicationDbContext())
        //    {
        //        post = ctx.Posts.Where(m => m.Id == postId).First();
        //        post.IsApproved = true;
        //        post.IsResolved = false;
        //        post.DateResolved = DateTime.Now;
        //        post.ResolverUsername = User.Identity.GetUserName();
        //        ctx.SaveChanges();
        //    }
        //    return View("Post", new { postId = postId });
        //}

        [Authorize]
        public ActionResult MyPosts()
        {
            MyPostsViewModel myPostsVM = new MyPostsViewModel();
            string userName = User.Identity.GetUserName();
            
            using(var ctx = new ApplicationDbContext())
            {
                myPostsVM.AddedPosts = ctx.Posts.Where(m => m.UserName == userName).ToList();
                //var upvotedPostsIdList
                List<int> upvotedPostsIdList = ctx.UserPostUpvotes.Where(m => m.UserName == userName).Select(m => m.PostId).ToList();
                foreach(var postId in upvotedPostsIdList)
                {
                    var post = ctx.Posts.Where(m => m.Id == postId).FirstOrDefault();
                    if (post != null)
                        myPostsVM.UpvotedPosts.Add(post);
                }
            }
            return View(myPostsVM);
        }

        [Authorize]
        public ActionResult Recent()
        {
            MyPostsViewModel myPostsVM = new MyPostsViewModel();
            string userName = User.Identity.GetUserName();

            using (var ctx = new ApplicationDbContext())
            {
                myPostsVM.AddedPosts = ctx.Posts.Where(m => m.IsResolved == true).OrderByDescending(m => m.DateAdded).Take(5).ToList();
                //var upvotedPostsIdList
                myPostsVM.UpvotedPosts = ctx.Posts.Where(m => m.IsApproved == true).OrderByDescending(m => m.DateAdded).Take(5).ToList();
            }
            return View(myPostsVM);
        }


        public byte[] CompressImage(byte[] inputBytes)
        {
            var jpegQuality = 70;
            Image image;
            byte[] outputBytes;
            using (var inputStream = new MemoryStream(inputBytes))
            {
                image = Image.FromStream(inputStream);
                var jpegEncoder = ImageCodecInfo.GetImageDecoders()
                  .First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                var encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, jpegQuality);

                using (var outputStream = new MemoryStream())
                {
                    image.Save(outputStream, jpegEncoder, encoderParameters);
                    outputBytes = outputStream.ToArray();
                }
            }
            return outputBytes;
        }

        public byte[] ConvertToBytes(HttpPostedFileBase Image)
        {
            byte[] imageBytes = null;
            BinaryReader reader = new BinaryReader(Image.InputStream);
            imageBytes = reader.ReadBytes((int)Image.ContentLength);
            return imageBytes;
        }

    }
}