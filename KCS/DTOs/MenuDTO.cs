using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KCS.DTOs
{
    public class MenuDTO
    {
        public MenuDTO()
        {

        }

        public int MenuId { get; set; }
        public string MenuText { get; set; }
        public string MenuIcon { get; set; }
        public string MenuTargetPage { get; set; }
        public Nullable<int> ParentMenuId { get; set; }
        public string ParentMenuName { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public bool ActiveStatus { get; set; }
    }
}