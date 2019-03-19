using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KCS.Models
{
    public class SettingMetadata
    {
        [Required]
        public string Name { get; set; }
    }

    [MetadataType(typeof(SettingMetadata))]
    public partial class Setting
    {
        public bool IsImageDeleted { get; set; }
    }
}