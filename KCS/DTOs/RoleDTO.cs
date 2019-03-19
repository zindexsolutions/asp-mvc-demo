using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KCS.DTOs
{
    public class RoleDTO
    {
        public RoleDTO()
        {

        }

        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public bool ActiveStatus { get; set; }
    }
}