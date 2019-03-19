
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCS.Common;
using KCS.Models;
using KCS.DTOs;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace KCS.Controllers
{
    public class MenuController : BaseController
    {
        KCSEntities KCS_dbContext = new KCSEntities();

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetMenuList()
        {
            #region Get list of menu and load that data in partial view.

            List<MenuDTO> Users = (from menu in KCS_dbContext.Menus
                                   from parentmenu in KCS_dbContext.Menus.Where(x=>x.MenuId==menu.ParentMenuId).DefaultIfEmpty()
                                   select new MenuDTO
                                   {
                                       MenuId = menu.MenuId,
                                       MenuText = menu.MenuText,
                                       MenuTargetPage = menu.MenuTargetPage,
                                       ParentMenuId = menu.ParentMenuId,
                                       ParentMenuName = parentmenu.MenuText,
                                       CreatedOn = menu.CreatedOn,
                                       ActiveStatus = menu.ActiveStatus
                                   }).OrderBy(x=>x.MenuText).ToList();

            return PartialView("_MenuList", Users);

            #endregion Get list of menu and load that data in partial view.
        }

        [HttpGet]
        public ActionResult Add()
        {
            #region Fill Parent Menu Dropdownlist.

            List<MenuDTO> Menus = (from menu in KCS_dbContext.Menus
                                   select new MenuDTO
                                   {
                                       MenuId = menu.MenuId,
                                       MenuText = menu.MenuText
                                   }).OrderBy(x=>x.MenuText).ToList();
            
            SelectList ParentMenuSelectList = new SelectList(Menus, "MenuId", "MenuText");

            ViewBag.ParentMenu = ParentMenuSelectList;

            #endregion Fill Parent Menu Dropdownlist.

            return View();
        }

        [HttpPost]
        public ActionResult Add(Menu menuModel)
        {
            try
            {
                #region Fill Parent Menu Dropdownlist.

                List<MenuDTO> Menus = (from menu in KCS_dbContext.Menus
                                       select new MenuDTO
                                       {
                                           MenuId = menu.MenuId,
                                           MenuText = menu.MenuText
                                       }).OrderBy(x => x.MenuText).ToList();

                SelectList ParentMenuSelectList = new SelectList(Menus, "MenuId", "MenuText");

                ViewBag.ParentMenu = ParentMenuSelectList;

                #endregion Fill Parent Menu Dropdownlist.

                if (ModelState.IsValid)
                {
                    try
                    {
                        #region If model is valid than insert record

                        Menu menu = new Menu();

                        menu.MenuText = menuModel.MenuText;
                        menu.MenuTargetPage= menuModel.MenuTargetPage;
                        menu.ParentMenuId = menuModel.ParentMenuId != null ? menuModel.ParentMenuId : (int?)null;
                        menu.ActiveStatus = menuModel.ActiveStatus;
                        menu.CreatedOn = DateTime.Now;

                        KCS_dbContext.Menus.Add(menu);
                        KCS_dbContext.SaveChanges();

                        #endregion If model is valid than insert record
                    }
                    catch (Exception ex)
                    {
                        #region If any error than add that errors to model and re-load the view with error message

                        ModelState.AddModelError("", ex.InnerException.InnerException.Message);

                        return View(menuModel);

                        #endregion If any error than add that errors to model and re-load the view with error message
                    }

                    #region When record save than redirect to List page and pass success message as TempData (A temporary session)

                    TempData["SuccessMessage"] = Constants.SuccessMessageInsert;

                    return RedirectToAction("Index", "Menu");

                    #endregion When record save than redirect to List page and pass success message as TempData (A temporary session)
                }
                else
                {
                    return View(menuModel);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException.InnerException.Message);
                return View(menuModel);
            }
        }

        #region It is a remote method call as validation method.

        public JsonResult MenuNameExists(int? MenuId, string MenuText)
        {
            #region To check whether entered name is already inserted or not.

            bool ReturnValue = false;

            if (MenuId == null)
            {
                #region Check only name at time of insert.

                ReturnValue = !KCS_dbContext.Menus.ToList().Exists(x => x.MenuText.Equals(MenuText, StringComparison.CurrentCultureIgnoreCase));

                #endregion Check only name at time of insert.
            }
            else
            {
                #region Check name and entered menu id at time of edit.

                ReturnValue = !KCS_dbContext.Menus.ToList().Exists(x => x.MenuText.Equals(MenuText, StringComparison.CurrentCultureIgnoreCase) && x.MenuId != MenuId);

                #endregion Check name and entered menu id at time of edit.
            }

            return Json(ReturnValue);

            #endregion To check whether entered name is already inserted or not.
        }

        #endregion It is a remote method call as validation method.

        [HttpGet]
        public ActionResult Edit(int id)
        {
            #region Fill Parent Menu Dropdownlist.

            List<MenuDTO> Menus = (from menu in KCS_dbContext.Menus
                                   select new MenuDTO
                                   {
                                       MenuId = menu.MenuId,
                                       MenuText = menu.MenuText
                                   }).OrderBy(x => x.MenuText).ToList();

            SelectList ParentMenuSelectList = new SelectList(Menus, "MenuId", "MenuText");

            ViewBag.ParentMenu = ParentMenuSelectList;

            #endregion Fill Parent Menu Dropdownlist.

            if (id > 0)
            {
                #region Get menu details by menu id and pass into view as model

                MenuDTO menuDTO = ((from menu in KCS_dbContext.Menus
                                    where menu.MenuId == id
                                    select new MenuDTO
                                    {
                                        MenuId = menu.MenuId,
                                        MenuText = menu.MenuText,
                                        MenuTargetPage = menu.MenuTargetPage,
                                        ParentMenuId = menu.ParentMenuId,
                                        ActiveStatus = menu.ActiveStatus
                                    })).FirstOrDefault();

                if (menuDTO != null)
                {
                    Menu menu = new Menu();

                    menu.MenuId = menuDTO.MenuId;
                    menu.MenuText= menuDTO.MenuText;
                    menu.MenuTargetPage = menuDTO.MenuTargetPage;
                    menu.ParentMenuId = menuDTO.ParentMenuId;
                    menu.ActiveStatus = menuDTO.ActiveStatus;

                    return View(menu);                    
                }

                #endregion Get menu details by menu id and pass into view as model

                return RedirectToAction("Index", "Menu");
            }

            return RedirectToAction("Index", "Menu");
        }

        [HttpPost]
        public ActionResult Edit(Menu menuModel)
        {
            try
            {
                #region Fill Parent Menu Dropdownlist.

                List<MenuDTO> Menus = (from menu in KCS_dbContext.Menus
                                       select new MenuDTO
                                       {
                                           MenuId = menu.MenuId,
                                           MenuText = menu.MenuText
                                       }).OrderBy(x => x.MenuText).ToList();

                SelectList ParentMenuSelectList = new SelectList(Menus, "MenuId", "MenuText");

                ViewBag.ParentMenu = ParentMenuSelectList;

                #endregion Fill Parent Menu Dropdownlist.

                if (ModelState.IsValid)
                {
                    try
                    {
                        #region Update menu details.

                        Menu menu = KCS_dbContext.Menus.FirstOrDefault(x => x.MenuId == menuModel.MenuId);

                        if (menu != null)
                        {
                            menu.MenuId = menuModel.MenuId;
                            menu.MenuText = menuModel.MenuText;
                            menu.MenuTargetPage = menuModel.MenuTargetPage;
                            menu.ParentMenuId = menuModel.ParentMenuId;
                            menu.ActiveStatus = menuModel.ActiveStatus;
                            menu.ModifiedOn = DateTime.Now;

                            KCS_dbContext.SaveChanges();
                        }

                        #endregion Update menu details.

                        #region When record updated than redirect to List page and pass success message as TempData (A temporary session)

                        TempData["SuccessMessage"] = Constants.SuccessMessageUpdate;

                        return RedirectToAction("Index", "Menu");

                        #endregion When record updated than redirect to List page and pass success message as TempData (A temporary session)
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", ex.InnerException.InnerException.Message);
                        return View(menuModel);
                    }
                }
                else
                {
                    return View(menuModel);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException.InnerException.Message);
                return View(menuModel);
            }
        }

        [HttpPost]
        public ActionResult Delete(int Id)
        {
            try
            {
                #region Delete Menu 

                Menu menu = KCS_dbContext.Menus.Find(Id);
                KCS_dbContext.Menus.Remove(menu);
                KCS_dbContext.SaveChanges();

                return Json(new { result = "success", success = Constants.SuccessMessageDelete }, JsonRequestBehavior.AllowGet);

                #endregion Delete Menu 
            }
            catch (DbUpdateException ex)
            {
                #region If refernce of menu id are there in child table than throw sql error. (Foreign key error)

                var sqlex = ex.InnerException.InnerException as SqlException;

                if (sqlex != null)
                {
                    switch (sqlex.Number)
                    {
                        case 547:
                            return Json(new { result = "error", success = Constants.ErrorMessageForeignKeyViolation }, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(new { result = "error", success = Constants.ErrorMessageDelete }, JsonRequestBehavior.AllowGet);

                #endregion If refernce of menu id are there in child table than throw sql error. (Foreign key error)

            }
            catch (Exception ex)
            {
                #region Throw any other exception

                return Json(new { result = "error", success = Constants.ErrorMessageDelete }, JsonRequestBehavior.AllowGet);

                #endregion Throw any other exception
            }
        }
    }
}