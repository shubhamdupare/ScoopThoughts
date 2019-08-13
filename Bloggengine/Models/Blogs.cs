namespace Bloggengine.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Web.Mvc;

    public partial class Blogs
    {
        public Blogs()
        {
            Like_conn = new HashSet<Likes>();
        }

        [Key]
        [ScaffoldColumn(false)]
        public int Blog_Id { get; set; }

        [Display(Name = "Title")]
        [Required(ErrorMessage = "Blog title is required")]
        [StringLength(100, ErrorMessage = "Use max 100 and min 50 characters", MinimumLength = 50)]
        public string Title { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Author Name")]
        public string Author { get; set; }

        [Display(Name = "Upload Image")]
        public string Images { get; set; }

        [Display(Name = "Body")]
        [Required(ErrorMessage = "Content is required")]
        [StringLength(5000, ErrorMessage = "Enter atleast 500-5000 characters ", MinimumLength = 500)]
        [AllowHtml]
        public string Content { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Posted { get; set; }

        public virtual ICollection<Likes> Like_conn { get; set; }

        public virtual Account Account { get; set; }
    }

    public partial class Likes
    {
        public Likes()
        {
            Accounts = new HashSet<Account>();
        }

        [Key]
        public int Like_id { get; set; }

        public int Blog_Id { get; set; }

        public int User_id { get; set; }

        public bool IsLiked { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }

        public virtual Blogs Blogs { get; set; }
    }

}
