using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KCS.Models
{
    public class RoleMetadata    
    {
        [Required]
        [Remote("RoleNameExists", "Role", HttpMethod = "POST", AdditionalFields = "RoleId", ErrorMessage = "Role already inserted.")]
        public string RoleName { get; set; }
    }

    [MetadataType(typeof(RoleMetadata))]
    public partial class Role
    {
    }
}