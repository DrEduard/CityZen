using Final.Models;
using Final.Models.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Final.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports
        [Authorize]
        public ActionResult MostFollowedReports()
        {
            List<Post> posts = null;
            //Dictionary<string, string> mostFollowedReports = new Dictionary<string, string>();
            List<MostFollowedReportsData> mf = new List<MostFollowedReportsData>();
            //mf.Add(new MostFollowedReportsData(5,4));
            //mf.Add(new MostFollowedReportsData(10, 8));
            //mf.Add(new MostFollowedReportsData(15, 19));
            //mf.Add(new MostFollowedReportsData(20, 3));
            //mf.Add(new MostFollowedReportsData(5, 40));

            using (var ctx = new ApplicationDbContext())
            {
                posts = ctx.Posts.OrderByDescending(m => m.Upvotes).Take(5).ToList();
                foreach (var post in posts)
                {
                    mf.Add(new MostFollowedReportsData(post.Upvotes, post.Title));
                }
            }

            return View(mf);
        }

        [Authorize]
        public ActionResult ActivityReport()
        {
            List<ActivityReportViewModel> avm = new List<ActivityReportViewModel>();


            using(var ctx = new ApplicationDbContext())
            {
                avm.Add(new ActivityReportViewModel(ctx.Posts.Where(m => m.IsApproved == false && m.IsResolved == false).Count(), "Not resolved", "Not resolved"));
                avm.Add(new ActivityReportViewModel(ctx.Posts.Where(m => m.IsApproved == true && m.IsResolved == false).Count(), "Approved", "Approved"));
                avm.Add(new ActivityReportViewModel(ctx.Posts.Where(m => m.IsResolved == true).Count(), "Resolved", "Resolved"));
            }
            return View(avm);
        }

        [Authorize]
        public ActionResult MostControversial()
        {
            MostControversialViewModel mvm = new MostControversialViewModel();

            List<Post> posts = null;

            using(var ctx = new ApplicationDbContext())
            {
                posts = ctx.Posts.Include("Comments").ToList();
                posts = posts.OrderByDescending(m => m.Comments.Count()).Take(5).ToList();
                foreach(var post in posts)
                {
                    mvm.top.Add(new Ob(post.Comments.Where(m => m.IsImportant == true).Count(), post.Title));
                    mvm.bot.Add(new Ob(post.Comments.Where(m => m.IsImportant == false).Count(), post.Title));
                }
            }
            return View(mvm);

        }
        public void ExportToXML(string type)
        {
            if(type == "mf")
            {
                List<Post> posts = null;
                List<MostFollowedReportsData> mf = new List<MostFollowedReportsData>();
                using (var ctx = new ApplicationDbContext())
                {
                    posts = ctx.Posts.OrderByDescending(m => m.Upvotes).Take(5).ToList();
                    foreach (var post in posts)
                    {
                        mf.Add(new MostFollowedReportsData(post.Upvotes, post.Title));
                    }
                }

                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename = MostFollowedPostsReport.xml");
                Response.ContentType = "text/xml";

                var serializer = new System.Xml.Serialization.XmlSerializer(mf.GetType());
                serializer.Serialize(Response.OutputStream, mf);
            }else if(type == "ar")
            {
                List<ActivityReportViewModel> avm = new List<ActivityReportViewModel>();
                using (var ctx = new ApplicationDbContext())
                {
                    avm.Add(new ActivityReportViewModel(ctx.Posts.Where(m => m.IsApproved == false && m.IsResolved == false).Count(), "Not resolved", "Not resolved"));
                    avm.Add(new ActivityReportViewModel(ctx.Posts.Where(m => m.IsApproved == true && m.IsResolved == false).Count(), "Approved", "Approved"));
                    avm.Add(new ActivityReportViewModel(ctx.Posts.Where(m => m.IsResolved == true).Count(), "Resolved", "Resolved"));
                }
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename = MostFollowedPostsReport.xml");
                Response.ContentType = "text/xml";

                var serializer = new System.Xml.Serialization.XmlSerializer(avm.GetType());
                serializer.Serialize(Response.OutputStream, avm);
            }
            else
            {
                MostControversialViewModel mvm = new MostControversialViewModel();

                List<Post> posts = null;

                using (var ctx = new ApplicationDbContext())
                {
                    posts = ctx.Posts.Include("Comments").ToList();
                    posts = posts.OrderByDescending(m => m.Comments.Count()).Take(5).ToList();
                    foreach (var post in posts)
                    {
                        mvm.top.Add(new Ob(post.Comments.Count(), post.Title));
                        mvm.bot.Add(new Ob(post.Comments.Where(m => m.IsImportant == false).Count(), post.Title));
                    }
                }
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename = MostFollowedPostsReport.xml");
                Response.ContentType = "text/xml";

                var serializer = new System.Xml.Serialization.XmlSerializer(mvm.top.GetType());
                serializer.Serialize(Response.OutputStream, mvm.top);
            }
            
        }

        //public ActionResult PostsResolvedPerMonth()
        //{

        //    List<Post> posts = null;

        //    List<PRPMReport> pr = new List<PRPMReport>();

        //    using(var c = new ApplicationDbContext())
        //    {
        //        posts = c.Posts.ToList();


        //    }

        //}

    }
}