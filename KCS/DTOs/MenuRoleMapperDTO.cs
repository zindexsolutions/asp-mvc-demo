using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KCS.DTOs
{
    public class MenuRoleMapperDTO
    {
        public MenuRoleMapperDTO()
        {

        }

        public int MenuId { get; set; }
        public string MenuText { get; set; }

        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public Nullable<int> ParentMenuId { get; set; }
        public string ParentMenuName { get; set; }

        public string MenuIcon { get; set; }
        public string MenuTargetPage { get; set; }

        public Nullable<bool> IsInsert { get; set; }
        public Nullable<bool> IsUpdate { get; set; }
        public Nullable<bool> IsDelete { get; set; }
        public Nullable<bool> IsView { get; set; }

        public System.DateTime CreatedOn { get; set; }
        
    }
}