
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCS.Common;
using KCS.Models;
using KCS.DTOs;
using System.Data.SqlClient;
using System.Data.Entity.Infrastructure;

namespace KCS.Controllers
{
    public class RoleController : BaseController
    {
        
        KCSEntities KCS_dbContext = new KCSEntities();

        public RoleController()
        {
            
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetRoleList()
        {
            #region Get list of role and load that data in partial view.

            List<RoleDTO> Users = (from role in KCS_dbContext.Roles
                                   select new RoleDTO
                                   {
                                       RoleId = role.RoleId,
                                       RoleName = role.RoleName,
                                       RoleDescription = role.RoleDescription, 
                                       CreatedOn=role.CreatedOn,
                                       ActiveStatus = role.ActiveStatus
                                   }).OrderBy(x => x.RoleName).ToList();

            return PartialView("_RoleList", Users);

            #endregion Get list of role and load that data in partial view.
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(Role roleModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        #region If model is valid than insert record

                        Role role = new Role();

                        role.RoleName = roleModel.RoleName;
                        role.RoleDescription = roleModel.RoleDescription;
                        role.ActiveStatus = roleModel.ActiveStatus;
                        role.CreatedOn = DateTime.Now;

                        KCS_dbContext.Roles.Add(role);
                        KCS_dbContext.SaveChanges();

                        #endregion If model is valid than insert record
                    }
                    catch (Exception ex)
                    {
                        #region If any error than add that errors to model and re-load the view with error message

                        ModelState.AddModelError("", ex.InnerException.InnerException.Message);
                        return View(roleModel);

                        #endregion If any error than add that errors to model and re-load the view with error message
                    }

                    #region When record save than redirect to List page and pass success message as TempData (A temporary session)

                    TempData["SuccessMessage"] = Constants.SuccessMessageInsert;

                    return RedirectToAction("Index", "Role");

                    #endregion When record save than redirect to List page and pass success message as TempData (A temporary session)
                }
                else
                {
                    return View(roleModel);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException.InnerException.Message);
                return View(roleModel);
            }
        }

        #region It is a remote method call as validation method.

        public JsonResult RoleNameExists(int? RoleId, string RoleName)
        {
            #region To check whether entered name is already inserted or not.

            bool ReturnValue = false;

            if (RoleId == null)
            {
                #region Check only name at time of insert.

                ReturnValue = !KCS_dbContext.Roles.ToList().Exists(x => x.RoleName.Equals(RoleName, StringComparison.CurrentCultureIgnoreCase));

                #endregion Check name and entered menu id at time of edit.
            }
            else
            {
                #region Check name and entered menu id at time of edit.

                ReturnValue = !KCS_dbContext.Roles.ToList().Exists(x => x.RoleName.Equals(RoleName, StringComparison.CurrentCultureIgnoreCase) && x.RoleId != RoleId);

                #endregion Check name and entered menu id at time of edit.
            }

            return Json(ReturnValue);

            #endregion To check whether entered name is already inserted or not.
        }

        #endregion It is a remote method call as validation method.

        [HttpGet]
        public ActionResult Edit(int id)
        {
            if(id==1)
            {
                return RedirectToAction("ErrorPage", "Home");
            }

            if (id > 0)
            {
                #region Get role details by menu id and pass into view as model

                RoleDTO roleDTO = ((from role in KCS_dbContext.Roles
                                    where role.RoleId == id
                                    select new RoleDTO
                                    {
                                        RoleId = role.RoleId,
                                        RoleName = role.RoleName,
                                        RoleDescription = role.RoleDescription,                                        
                                        ActiveStatus = role.ActiveStatus
                                    })).FirstOrDefault();

                if (roleDTO != null)
                {
                    Role role = new Role();

                    role.RoleId = roleDTO.RoleId;
                    role.RoleName = roleDTO.RoleName;
                    role.RoleDescription = roleDTO.RoleDescription;                    
                    role.ActiveStatus = roleDTO.ActiveStatus;

                    return View(role);
                }
                #endregion Get role details by menu id and pass into view as model

                return RedirectToAction("Index", "Role");
            }

            return RedirectToAction("Index", "Role");
        }

        [HttpPost]
        public ActionResult Edit(Role roleModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        #region Update role details.

                        Role role = KCS_dbContext.Roles.FirstOrDefault(x => x.RoleId == roleModel.RoleId);

                        if (role != null)
                        {
                            role.RoleId = roleModel.RoleId;
                            role.RoleName = roleModel.RoleName;
                            role.RoleDescription = roleModel.RoleDescription;
                            role.ActiveStatus = roleModel.ActiveStatus;
                            role.ModifiedOn = DateTime.Now;

                            KCS_dbContext.SaveChanges();
                        }

                        #endregion Update role details.

                        #region When record updated than redirect to List page and pass success message as TempData (A temporary session)

                        TempData["SuccessMessage"] = Constants.SuccessMessageUpdate;

                        return RedirectToAction("Index", "Role");

                        #endregion When record updated than redirect to List page and pass success message as TempData (A temporary session)
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", ex.InnerException.InnerException.Message);
                        return View(roleModel);
                    }
                }
                else
                {
                    return View(roleModel);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException.InnerException.Message);
                return View(roleModel);
            }
        }

        [HttpPost]
        public ActionResult Delete(int Id)
        {
            try
            {
                #region Delete Role

                Role role = KCS_dbContext.Roles.Find(Id);
                KCS_dbContext.Roles.Remove(role);
                KCS_dbContext.SaveChanges();

                #endregion Delete Role 

                return Json(new { result = "success", success = Constants.SuccessMessageDelete }, JsonRequestBehavior.AllowGet);
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