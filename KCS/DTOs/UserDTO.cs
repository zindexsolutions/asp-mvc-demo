using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KCS.DTOs
{
    public class UserDTO
    {
        public UserDTO()
        {
            IsDefault = false;
        }

        public int UserId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public byte[] ProfilePicture { get; set; }
        
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public bool ActiveStatus { get; set; }
        public bool IsDefault { get; set; }
    }
}