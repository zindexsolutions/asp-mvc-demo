using KCS.DTOs;
using KCS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCS.Common;

namespace KCS.Controllers
{
    public class LoginController : Controller
    {
        KCSEntities KCS_dbContext = new KCSEntities();
        // GET: Login
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Index(User model)
        {
            //Exclude validation from Name and StartDate field
            ModelState.Remove("Name");
            ModelState.Remove("StartDate");
            if (ModelState.IsValid)
            {
                UserDTO ReturnValue = CheckLogin(model.Username, model.Password);

                if (ReturnValue == null)
                {
                    #region When username or password not found then add errors to model

                    ModelState.Clear();
                    ModelState.AddModelError("", "Username or password not found, try again...!");

                    #endregion When username or password not found then add errors to model

                    ViewBag.IsError = true;

                    return View(model);
                }
                else if (ReturnValue.ActiveStatus == false)
                {
                    #region When user status is inactivated then don't allow user to login

                    ModelState.Clear();
                    ModelState.AddModelError("", "Sorry this user is inactivated.");

                    #endregion When user status is inactivated then don't allow user to login

                    ViewBag.IsError = true;

                    return View(model);
                }
                else
                {
                    Session[Constants.LoginSessionName] = ReturnValue;

                    #region Get User Rights and store all Rights in session so we use tham in whole project.

                    Session[Constants.RightsSessionName] = GetUserRights(ReturnValue.RoleId);
                    Session[Constants.ParentMenuSessionName] = GetParentMenu();

                    #endregion Get User Rights and store all Rights in session so we use tham in whole project.

                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.IsError = true;
                return View(model);
            }
        }

        #region  Get default parent menu 

        public List<MenuRoleMapperDTO> GetParentMenu()
        {
            #region Get all menu (who don't have any parent element) -- They are either parent or any menu or main menu

            var ParentMenu = (from menu in KCS_dbContext.Menus
                                join parentmenu in KCS_dbContext.Menus on menu.MenuId equals parentmenu.ParentMenuId
                                where menu.ParentMenuId == null
                                select new MenuRoleMapperDTO
                                {
                                    MenuId = menu.MenuId,
                                    MenuText = menu.MenuText,
                                    MenuIcon = menu.MenuIcon,
                                    MenuTargetPage = menu.MenuTargetPage,
                                    IsInsert = true,
                                    IsUpdate = true,
                                    IsDelete = true,
                                    IsView = true,
                                }).Distinct().ToList();

            #endregion Get all menu (who don't have any parent element) -- They are either parent or any menu or main menu

            if (ParentMenu != null)
            {
                return ParentMenu;
            }
            return null;
        }

        #endregion Get default parent menu

        #region Get User Rights 

        public List<MenuRoleMapperDTO> GetUserRights(int RoleId)
        {
            if(RoleId==1)
            {
                #region Get all menus and allow all things for Super Administrator Role

                List<MenuDTO> ParentMenu = (from menu in KCS_dbContext.Menus
                                            where menu.ParentMenuId!=null
                                             select new MenuDTO {
                                                 MenuId=menu.MenuId,
                                                 ParentMenuId=menu.ParentMenuId
                                             }).ToList();

                List<Nullable<int>> ParentMenuIDs = ParentMenu.Select(t => t.ParentMenuId).Distinct().ToList();

                #region Allow Insert/Update/Delete/View for Super Administror only.

                var SuperAdminRights = (from menu in KCS_dbContext.Menus                                        
                                        select new MenuRoleMapperDTO
                                        {
                                          MenuId = menu.MenuId,
                                          MenuText = menu.MenuText,
                                          MenuIcon = menu.MenuIcon,
                                          ParentMenuId = menu.ParentMenuId,
                                          MenuTargetPage = menu.MenuTargetPage,
                                          IsInsert = true,
                                          IsUpdate = true,
                                          IsDelete = true,
                                          IsView = true,
                                        }).OrderBy(x => x.MenuText).Where(x=> !ParentMenuIDs.Contains(x.MenuId)).ToList();

                #endregion Allow Insert/Update/Delete/View for Super Administror only.

                #endregion Get all menus and allow all things for Super Administrator Role

                if (SuperAdminRights != null)
                {
                    return SuperAdminRights;
                }
                return null;
            }

            #region Get all rights for the current logged in user whose role is not super administrator

            var UserRights = (from menuRoleMapper in KCS_dbContext.MenuRoleMappers
                              join menu in KCS_dbContext.Menus on menuRoleMapper.MenuId equals menu.MenuId
                              where menuRoleMapper.RoleId == RoleId
                              select new MenuRoleMapperDTO
                              {
                                  MenuId = menu.MenuId,
                                  MenuText = menu.MenuText,
                                  MenuIcon = menu.MenuIcon,
                                  ParentMenuId = menu.ParentMenuId,
                                  MenuTargetPage = menu.MenuTargetPage,
                                  IsInsert = menuRoleMapper.IsInsert,
                                  IsUpdate = menuRoleMapper.IsUpdate,
                                  IsDelete = menuRoleMapper.IsDelete,
                                  IsView = menuRoleMapper.IsView,
                              }).OrderBy(x=>x.MenuText).ToList();

            #endregion Get all rights for the current logged in user whose role is not super administrator

            if (UserRights!=null)
            {
                return UserRights;
            }
            return null;
        }

        #endregion Get User Rights 

        public UserDTO CheckLogin(string UserName, string Password)
        {
            #region Encrypt the user entered password.

            Password = Encryptor.Encrypt(Password);

            #endregion Encrypt the user entered password.

            var UserRecord = (from user in KCS_dbContext.Users
                              join role in KCS_dbContext.Roles on user.RoleId equals role.RoleId
                              where user.Username == UserName && user.Password == Password
                              select new UserDTO
                              {
                                  UserId = user.UserId,
                                  RoleId = role.RoleId,
                                  RoleName = role.RoleName,
                                  Username = user.Username,
                                  Name = user.Name,
                                  Phone = user.Phone,
                                  ProfilePicture = user.ProfilePicture,
                                  StartDate = user.StartDate,
                                  EndDate = user.EndDate,
                                  ActiveStatus = user.ActiveStatus,
                                  IsDefault=user.IsDefault
                              }).FirstOrDefault();
            
            if (UserRecord == null)
            {
                return null;
            }
            else
            {
                return UserRecord;
            }
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            #region Clear all session and logout current user

            Session.Clear();

            return RedirectToAction("Index", "Login");

            #endregion Clear all session and logout current user
        }
    }
}