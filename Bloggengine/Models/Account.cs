namespace Bloggengine.Models
{
    using System.IO;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Account
    {
       
        public Account()
        {
            Like_conn = new HashSet<Likes>();
        }
        [Key]
        [ScaffoldColumn(false)]
        public int Id { get; set; }

        
        
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(20, ErrorMessage = "Use max 20 and min 8 characters", MinimumLength = 4)]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Only Alphabets and Numbers allowed.")]
        public string UserName { get; set; }

        
        [Display(Name ="First Name")]
        [Required(ErrorMessage ="First name is required")]
        [StringLength(20, ErrorMessage = "Use max 10 and min 4 characters", MinimumLength = 3)]
        public string F_Name { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last name is required")]
        [StringLength(20, ErrorMessage = "Use max 10 and min 4 characters", MinimumLength = 3)]
        public string L_Name { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "First name is required")]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }


        [Required(ErrorMessage ="Password required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [RegularExpression(@"^.*(?=.*[!@#$%^&*\(\)_\-+=]).*$", ErrorMessage = "password has to be at least 7 characters long and contain a special character (~@#$&*()-_+=) ")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [NotMapped]
        public string Confirm_Password { get; set; }

        public virtual ICollection<Likes> Like_conn { get; set; }
    }
}
