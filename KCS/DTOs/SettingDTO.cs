using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KCS.DTOs
{
    public class SettingDTO
    {
        public SettingDTO()
        {

        }

        public int SettingId { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }
    
    }
}