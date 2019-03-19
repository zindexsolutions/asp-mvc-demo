using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KCS.Models
{
    public static class Constants
    {

        public static string ProjectName = "KCS";
        public static string CurrentVersion = "<b>Version</b> 1.0";
        public static string FooterText = "<strong>Copyright © " + DateTime.Now.Year + " KCS.</strong> All rights reserved";
        public static string LoginSessionName = "UserInformation";
        public static string RightsSessionName = "UserRights";
        public static string ParentMenuSessionName = "ParentMenu";

        public static string SuccessMessageInsert = "Record inserted successfully.";
        public static string SuccessMessageUpdate = "Record updated successfully.";
        public static string SuccessMessageDelete = "Record deleted successfully.";

        public static string ErrorMessageInsert = "Record not inserted, please try again.";
        public static string ErrorMessageUpdate = "Record not updated, please try again.";
        public static string ErrorMessageDelete = "Record not deleted, please try again.";

        public static string ErrorMessageForeignKeyViolation = "A record you want to remove is referenced in some another table. Please remove that reference first.";

        public static string SuccessMessageRights = "Rights for the selected user have been submited.";
        public static string ErrorMessageRights = "Some error occured, rights information not saved.";

    }
}