using KCS.DTOs;
using KCS.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace KCS.Controllers
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TestFilter : ActionFilterAttribute
    {

    }

    public class BaseController : Controller
    {
        KCSEntities KCS_dbContext = new KCSEntities();

        public BaseController()
        {
            #region Set date culture for whole project.

            CultureInfo newCulture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            newCulture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            newCulture.DateTimeFormat.DateSeparator = "/";
            Thread.CurrentThread.CurrentCulture = newCulture;

            #endregion Set date culture for whole project.
        }

        public void Generate_BreadCrum(string ControllerName, string ActionName, int LastMenuId)
        {
            try
            {
                #region Generate Breadcrumb - For generating breadcrum we start with action and reach to the top level till parent menu is null

                List<Crumb> crumbs = new List<Crumb>();

                #region When dashboard is there than we are not add that action in breadcrum

                if (ControllerName != "Home" || ActionName != "Index")
                {
                    ActionName = (ActionName == "Index") ? "View" : ActionName;
                    crumbs.Add(new Crumb() { Text = ActionName });
                }

                #endregion When dashboard is there than we are not add that action in breadcrum

                var MenuList = (from menus in KCS_dbContext.Menus
                                select new MenuDTO {
                                    MenuId = menus.MenuId,
                                    MenuText = menus.MenuText,
                                    MenuTargetPage = menus.MenuTargetPage,
                                    MenuIcon = menus.MenuIcon,
                                    ParentMenuId = menus.ParentMenuId,
                                }).ToList();

                if (MenuList != null)
                {
                    Nullable<int> CurrentMenuId = LastMenuId;

                    #region Loop through leaf menu to top parent menu

                    while (CurrentMenuId != null)
                    {
                        foreach (MenuDTO menuDto in MenuList.Where(x => x.MenuId == CurrentMenuId))
                        {
                            crumbs.Add(new Crumb() { Text = menuDto.MenuText, Link = menuDto.MenuTargetPage, Icon = menuDto.MenuIcon });

                            CurrentMenuId = menuDto.ParentMenuId;
                        }
                    }

                    #endregion Loop through leaf menu to top parent menu
                }

                #region Add Home (Dashboard) Link - Default in whole project

                crumbs.Add(new Crumb() { Text = "Home", Link = "Home", Icon = "fa fa-home" });

                #endregion Add Home (Dashboard) Link - Default in whole project

                crumbs.Reverse();

                ViewBag.Breadcrumb = crumbs;

                #endregion Generate Breadcrumb - For generating breadcrum we start with action and reach to the top level till parent menu is null
            }
            catch
            {

            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            #region Get Current request Action and controller name.
            
            //Check Session is created or not
            //ActionDescriptor actionDescriptor = filterContext.ActionDescriptor;
            string ActionName = filterContext.ActionDescriptor.ActionName;
            string ControllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            #endregion Get Current request Action and controller name.

            UserDTO userDTO = null;

            #region When session  is not created than redirect to login page.

            if (Session[Constants.LoginSessionName] != null)
            {
                #region Store user session details in Viewbag so we can use tham in whole project.

                ViewBag.UserInformation = Session[Constants.LoginSessionName];

                #endregion Store user session details in Viewbag so we can use tham in whole project.

                userDTO = (UserDTO)Session[Constants.LoginSessionName];

                if (Session[Constants.RightsSessionName] != null)
                {
                    #region Store user rights details in Viewbag so we can use tham in whole project. (For SuperAdministrator allow full access)

                    ViewBag.UserRights = Session[Constants.RightsSessionName];

                    #endregion Store user rights details in Viewbag so we can use tham in whole project. (For SuperAdministrator allow full access)
                }

                if (Session[Constants.ParentMenuSessionName] != null)
                {
                    ViewBag.ParentMenuRights = Session[Constants.ParentMenuSessionName];
                }

                var allowAnonymous = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);

                #region Get Project Logo Image Form Settings table.

                Get_Logo();

                #endregion Get Project Logo Image Form Settings table.

                #region All action which is added with anonymous attribute than we dont check for rights (Dashboard, Logout etc..)

                if (!allowAnonymous)
                {
                    #region Current Page Rights 

                    try
                    {
                        MenuRoleMapperDTO CurrentPageRights=null;

                        if (userDTO.RoleId == 1)
                        {
                            CurrentPageRights = (from menus in KCS_dbContext.Menus
                                                 where menus.MenuTargetPage == ControllerName
                                                 select new MenuRoleMapperDTO()
                                                 {
                                                     MenuId = menus.MenuId,
                                                     MenuText = menus.MenuText,
                                                     IsInsert = true,
                                                     IsUpdate = true,
                                                     IsDelete = true,
                                                     IsView = true
                                                 }).FirstOrDefault();
                        }
                        else
                        {
                            CurrentPageRights = (from menuRoleMapper in KCS_dbContext.MenuRoleMappers
                                                 join menus in KCS_dbContext.Menus on menuRoleMapper.MenuId equals menus.MenuId
                                                 where menus.MenuTargetPage == ControllerName && menuRoleMapper.RoleId == userDTO.RoleId
                                                 select new MenuRoleMapperDTO()
                                                 {
                                                     MenuId = menus.MenuId,
                                                     MenuText = menus.MenuText,
                                                     IsInsert = menuRoleMapper.IsInsert,
                                                     IsUpdate = menuRoleMapper.IsUpdate,
                                                     IsDelete = menuRoleMapper.IsDelete,
                                                     IsView = menuRoleMapper.IsView
                                                 }).FirstOrDefault();
                        }

                        if (CurrentPageRights == null)
                        {
                            throw new Exception();
                        }
                        else
                        {
                            ViewBag.CurrentPageRights = CurrentPageRights;

                            Generate_BreadCrum(ControllerName, ActionName, CurrentPageRights.MenuId);

                            if (ActionName == "Add")
                            {
                                if (CurrentPageRights.IsInsert == true)
                                {
                                    base.OnActionExecuting(filterContext);
                                }
                                else
                                {
                                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                                    {
                                        {"controller", "Home"},
                                        {"action", "ErrorPage"}
                                    });
                                }
                            }
                            else if (ActionName == "Edit")
                            {
                                if (CurrentPageRights.IsUpdate == true)
                                {
                                    base.OnActionExecuting(filterContext);
                                }
                                else
                                {
                                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                                    {
                                        {"controller", "Home"},
                                        {"action", "ErrorPage"}
                                    }
                                    );
                                }
                            }
                            else if (ActionName == "Delete")
                            {
                                if (CurrentPageRights.IsDelete == true)
                                {
                                    base.OnActionExecuting(filterContext);
                                }
                                else
                                {
                                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                                    {
                                        {"controller", "Home"},
                                        {"action", "ErrorPage"}
                                    }
                                    );
                                }
                            }
                            else if (ActionName == "Index")
                            {
                                if (CurrentPageRights.IsView == true)
                                {
                                    base.OnActionExecuting(filterContext);
                                }
                                else
                                {
                                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                                    {
                                        {"controller", "Home"},
                                        {"action", "ErrorPage"}
                                    }
                                    );
                                }
                            }
                            else
                            {
                                base.OnActionExecuting(filterContext);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        filterContext.Result = new RedirectResult("~/Home/Index");
                    }

                    #endregion Current Page Rights     
                }
                else
                {
                    base.OnActionExecuting(filterContext);
                }

                #endregion All action which is added with anonymous attribute than we dont check for rights (Dashboard, Logout etc..)
            }
            else
            {

                filterContext.Result = new RedirectResult("~/Login/Index");
            }

            #endregion When session  is not created than redirect to login page.
        }

        public void Get_Logo()
        {
            Setting setting = KCS_dbContext.Settings.Where(x => x.SettingId == 1).FirstOrDefault(); 

            if(setting!=null)
            {
                ViewBag.Setting = setting;
            }
        }
    }
}