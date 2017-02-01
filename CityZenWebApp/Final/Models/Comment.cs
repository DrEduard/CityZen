using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Final.Models
{
    [Table("Comments")]
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int PostId { get; set; }

        public string Username { get; set; }

        public string Text { get; set; }

        public DateTime? DateAdded { get; set; }

        public byte[] Photo { get; set; }

        public int Upvotes { get; set; }

        public bool IsImportant { get; set; }
    }
}