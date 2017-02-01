using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Final.Models
{
    [Table("Posts")]
    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public byte[] Photo { get; set; }

        public string PhotoExtension { get; set; }
        [Required]
        [Display(Name = "Category")]
        public string Category { get; set; }

        [Required]
        [Display(Name = "Street address")]
        public string Address { get; set; }
        [Required]
        [Display(Name = "City")]
        public string City { get; set; }

        public DateTime? DateAdded { get; set; }

        public DateTime? DateResolved { get; set; }

        public List<Comment> Comments { get; set; }

        public int Upvotes { get; set; }

        public bool IsApproved { get; set; }

        public bool IsResolved { get; set; }
        /// <summary>
        /// the one who resolved the post
        /// </summary>
        public string ResolverUsername { get; set; }
        /// <summary>
        /// username of the person who added the post
        /// </summary>
        public string UserName { get; set; }

        public ApplicationUser User { get; set; }


    }
}