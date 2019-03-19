using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KCS.Models
{
    public class MenuMetadata
    {
        [Required]
        [Remote("MenuNameExists", "Menu", HttpMethod = "POST", AdditionalFields = "MenuId", ErrorMessage = "Menu already inserted.")]
        [Display(Name = "Menu Text")]
        public string MenuText { get; set; }

        [Required]
        [Display(Name ="Target Page")]
        public string MenuTargetPage { get; set; }

        [Display(Name = "Parent Menu")]
        public Nullable<int> ParentMenuId { get; set; }
    }

    [MetadataType(typeof(MenuMetadata))]
    public partial class Menu
    {
    }
}