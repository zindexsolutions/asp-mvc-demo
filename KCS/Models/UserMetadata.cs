using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KCS.Models
{
    public class UserMetadata
    {
        [Required]
        [Remote("UserNameExists", "User", HttpMethod = "POST", AdditionalFields = "UserId", ErrorMessage = "Username already registered.")]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Name { get; set; }
        
        [Required(ErrorMessage ="Please select role.")]
        public int RoleId { get; set; }

        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Please enter a valid phone number.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Please select start date.")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> EndDate { get; set; }

    }

    [MetadataType(typeof(UserMetadata))]
    public partial class User
    {
        public bool IsImageDeleted { get; set; }
    }
}