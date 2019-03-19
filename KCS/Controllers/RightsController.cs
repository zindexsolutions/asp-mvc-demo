using KCS.DTOs;
using KCS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KCS.Controllers
{
    public class RightsController : BaseController
    {
        KCSEntities KCS_dbContext = new KCSEntities();

        [HttpGet]
        public ActionResult Index()
        {
            #region Get role except Super Administrator.

            List<RoleDTO> Roles = (from role in KCS_dbContext.Roles
                                   where role.RoleId!=1
                                   select new RoleDTO
                                   {
                                       RoleId = role.RoleId,
                                       RoleName = role.RoleName
                                   }).OrderBy(x=>x.RoleName).ToList();
            
            SelectList RoleSelectList = new SelectList(Roles, "RoleId", "RoleName");

            ViewBag.Role = RoleSelectList;

            #endregion Get role except Super Administrator.

            return View();
        }

        public ActionResult GetRightsList(int RoleId)
        {
            #region Get User Rights (if already added than we get rights for all menu if rights not set than only menu name)

            List<MenuRoleMapperDTO> UserRights = (from menu in KCS_dbContext.Menus
                                                  join menuRoleMapper in KCS_dbContext.MenuRoleMappers.Where(x => x.RoleId==RoleId) on menu.MenuId equals menuRoleMapper.MenuId into menuRoleMapperAvaialbleRecords
                                                  join parentMenu in KCS_dbContext.Menus on menu.ParentMenuId equals parentMenu.MenuId into parentMenu                                                  
                                                  from rights in menuRoleMapperAvaialbleRecords.DefaultIfEmpty()
                                                  from parentMenus in parentMenu.DefaultIfEmpty()
                                                  select new MenuRoleMapperDTO
                                                  {
                                                      MenuId = menu.MenuId,
                                                      MenuText = menu.MenuText,
                                                      ParentMenuId = menu.ParentMenuId,
                                                      ParentMenuName = parentMenus.MenuText,
                                                      IsInsert = rights.IsInsert,
                                                      IsUpdate = rights.IsUpdate,
                                                      IsDelete = rights.IsDelete,
                                                      IsView = rights.IsView,
                                                  }).OrderBy(x=>x.MenuText).ToList();

            #endregion Get User Rights (if already added than we get rights for all menu if rights not set than only menu name)

            return PartialView("_ManageRights", UserRights);
        }

        [HttpPost]
        public ActionResult SaveRights(List<MenuRoleMapper> MenuRoleMappers, int RoleId)
        {
            #region Start Database transaction.

            using (DbContextTransaction transaction = KCS_dbContext.Database.BeginTransaction())
            {
                try
                {
                    #region Delete all records from menurolemapper for the passed RoleId

                    KCS_dbContext.MenuRoleMappers.RemoveRange(KCS_dbContext.MenuRoleMappers.Where(x => x.RoleId == RoleId));

                    KCS_dbContext.SaveChanges();

                    #endregion Delete all records from menurolemapper for the passed RoleId

                    #region Update CreatedOn datetime for all the records 

                    MenuRoleMappers.ForEach((menuRoleMapper) =>
                    {
                        menuRoleMapper.CreatedOn = DateTime.Now;
                    });

                    #endregion Update CreatedOn datetime for all the records 

                    IEnumerable<MenuRoleMapper> Rights = MenuRoleMappers;

                    #region Insert multiple records by Add Range

                    KCS_dbContext.MenuRoleMappers.AddRange(Rights);

                    KCS_dbContext.SaveChanges();

                    #endregion Insert multiple records by Add Range

                    #region If all things are done than commit the transacation.

                    transaction.Commit();

                    #endregion If all things are done than commit the transacation.

                    #region Once rights have been updated than regenerate the rights session varaible

                    Session[Constants.RightsSessionName] = GetUserRights(ViewBag.UserInformation.RoleId);

                    #endregion Once rights have been updated than regenerate the rights session varaible
                }
                catch (Exception ex)
                {
                    #region If any errors occurred than rollback the transaction.

                    transaction.Rollback();

                    #endregion If any errors occurred than rollback the transaction.

                    return Json(new { result = "error", success = Constants.ErrorMessageRights }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { result = "success", success = Constants.SuccessMessageRights }, JsonRequestBehavior.AllowGet);

            #endregion Start Database transaction.
        }

        #region Get User Rights 

        public List<MenuRoleMapperDTO> GetUserRights(int RoleId)
        {
            if (RoleId == 1)
            {
                #region Get all menus and allow all things for Super Administrator Role

                List<MenuDTO> ParentMenu = (from menu in KCS_dbContext.Menus
                                            where menu.ParentMenuId != null
                                            select new MenuDTO
                                            {
                                                MenuId = menu.MenuId,
                                                ParentMenuId = menu.ParentMenuId
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
                                        }).OrderBy(x => x.MenuText).Where(x => !ParentMenuIDs.Contains(x.ParentMenuId)).ToList();

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
                              }).OrderBy(x => x.MenuText).ToList();

            #endregion Get all rights for the current logged in user whose role is not super administrator

            if (UserRights != null)
            {
                return UserRights;
            }
            return null;
        }

        #endregion Get User Rights 
    }
}
