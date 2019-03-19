using KCS.Common;
using KCS.DTOs;
using KCS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KCS.Controllers
{
    public class UserController : BaseController
    {
        KCSEntities KCS_dbContext = new KCSEntities();

        // GET: User
        [HttpGet]
        public ActionResult Index()
        {
            //ViewBag.Role = Roles.Count > 0 ? Roles : null;

            return View();
        }

        public ActionResult GetUserList()
        {
            #region Get list of users and load that data in partial view.

            List<UserDTO> Users = (from user in KCS_dbContext.Users
                                   join role in KCS_dbContext.Roles on user.RoleId equals role.RoleId
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
                                       IsDefault = user.IsDefault

                                   }).OrderBy(x => x.Name).ToList();

            #endregion Get list of users and load that data in partial view.

            return PartialView("_UserList", Users);
        }

        [HttpGet]
        public ActionResult Add()
        {
            #region Fill Role Dropdownlist

            List<RoleDTO> Roles = new List<RoleDTO>();

            if (ViewBag.UserInformation.RoleId==1)
            {
                #region If role of logged in user is super administrator than allow tham to add new super administrator

                Roles = (from role in KCS_dbContext.Roles
                        select new RoleDTO
                        {
                            RoleId = role.RoleId,
                            RoleName = role.RoleName
                        }).OrderBy(x => x.RoleName).ToList();

                #endregion If role of logged in user is super administrator than allow tham to add new super administrator
            }
            else
            {
                #region If role of logged in user is csuper administrator than don't allow tham to add new super administrator

                Roles = (from role in KCS_dbContext.Roles
                         where role.RoleId!=1
                         select new RoleDTO
                         {
                             RoleId = role.RoleId,
                             RoleName = role.RoleName
                         }).OrderBy(x => x.RoleName).ToList();

                #endregion If role of logged in user is not super administrator than don't allow tham to add new super administrator
            }

            SelectList RoleSelectList = new SelectList(Roles, "RoleId", "RoleName");

            ViewBag.Role = RoleSelectList;

            #endregion Fill Role Dropdownlist

            return View();
        }
        
        [HttpPost]
        public ActionResult Add(User userModel)
        {
            try
            {
                #region Fill dropdown list -- same as above rights pattern used.

                List<RoleDTO> Roles = new List<RoleDTO>();

                if (ViewBag.UserInformation.RoleId == 1)
                {
                    Roles = (from role in KCS_dbContext.Roles
                             select new RoleDTO
                             {
                                 RoleId = role.RoleId,
                                 RoleName = role.RoleName
                             }).OrderBy(x => x.RoleName).ToList();
                }
                else
                {
                    Roles = (from role in KCS_dbContext.Roles
                             where role.RoleId != 1
                             select new RoleDTO
                             {
                                 RoleId = role.RoleId,
                                 RoleName = role.RoleName
                             }).OrderBy(x => x.RoleName).ToList();
                }

                SelectList RoleSelectList = new SelectList(Roles, "RoleId", "RoleName");

                ViewBag.Role = RoleSelectList;

                #endregion Fill dropdown list -- same as above rights pattern used.

                #region Check varaious date validation 

                KeyValuePair<bool, string> IsDateValid = Check_Date_Validation(Convert.ToDateTime(userModel.StartDate), userModel.EndDate, userModel.ActiveStatus);

                if (IsDateValid.Key == false)
                {
                    ModelState.AddModelError("", IsDateValid.Value);
                }

                #endregion Check varaious date validation 

                if (ModelState.IsValid)
                {
                    #region Insert New User

                    User user = new User();

                    user.Username = userModel.Username;

                    #region Encrypt user password.

                    user.Password = Encryptor.Encrypt(userModel.Password);

                    #endregion Encrypt user password.

                    user.Name = userModel.Name;
                    user.Phone = userModel.Phone;
                    user.RoleId = userModel.RoleId;
                    user.StartDate = userModel.StartDate != null ? Convert.ToDateTime(userModel.StartDate) : (DateTime?)null;
                    user.EndDate = userModel.EndDate != null ? Convert.ToDateTime(userModel.EndDate) : (DateTime?)null;

                    #region If User profile picture is added than add tham as binary data.

                    var ProfilePicturePath = Request.Files["ProfilePicturePath"];
                    if (ProfilePicturePath != null && ProfilePicturePath.ContentLength > 0)
                    {
                        userModel.ProfilePicture = new byte[ProfilePicturePath.ContentLength];
                        ProfilePicturePath.InputStream.Read(userModel.ProfilePicture, 0, ProfilePicturePath.ContentLength);
                        user.ProfilePicture = userModel.ProfilePicture;
                    }

                    #endregion If User profile picture is added than add tham as binary data.
                    else
                    {
                        user.ProfilePicture = null;
                    }

                    user.ActiveStatus = userModel.ActiveStatus;
                    user.CreatedOn = DateTime.Now;
                    
                    KCS_dbContext.Users.Add(user);
                    KCS_dbContext.SaveChanges();

                    #endregion Insert New User

                    #region When record save than redirect to List page and pass success message as TempData (A temporary session)

                    TempData["SuccessMessage"] = Constants.SuccessMessageInsert;

                    return RedirectToAction("Index", "User");

                    #endregion When record save than redirect to List page and pass success message as TempData (A temporary session)
                }
                ModelState.AddModelError("", "Something went wrong, try again.");
                return View(userModel);
                
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException.InnerException.Message);
                return View(userModel);
            }
        }

        #region Set Data at time of edit record

        [HttpGet]        
        public ActionResult Edit(int id)
        {
            if(id > 0)
            {
                #region Get User record for update.

                UserDTO userDTO = ((from user in KCS_dbContext.Users
                                where user.UserId == id
                                    select new UserDTO
                                    {
                                      UserId = user.UserId,
                                      RoleId = user.RoleId,
                                      Username = user.Username,
                                      Password = user.Password,
                                      Name = user.Name,
                                      Phone = user.Phone,
                                      ProfilePicture = user.ProfilePicture,
                                      StartDate = user.StartDate,
                                      EndDate = user.EndDate,                                      
                                      ActiveStatus = user.ActiveStatus
                                })).FirstOrDefault();

                #endregion Get User record for update.

                if (userDTO != null)
                {
                    #region Decrypt the password

                    userDTO.Password=Encryptor.Decrypt(userDTO.Password);

                    #endregion Decrypt the password

                    #region Fill dropdownlist - (Same as Index method pattern)

                    List<RoleDTO> Roles = new List<RoleDTO>();

                    if (ViewBag.UserInformation.RoleId == 1)
                    {
                        Roles = (from role in KCS_dbContext.Roles
                                 select new RoleDTO
                                 {
                                     RoleId = role.RoleId,
                                     RoleName = role.RoleName
                                 }).OrderBy(x => x.RoleName).ToList();
                    }
                    else
                    {
                        Roles = (from role in KCS_dbContext.Roles
                                 where role.RoleId != 1
                                 select new RoleDTO
                                 {
                                     RoleId = role.RoleId,
                                     RoleName = role.RoleName
                                 }).OrderBy(x => x.RoleName).ToList();
                    }

                    SelectList RoleSelectList = new SelectList(Roles, "RoleId", "RoleName", userDTO.RoleId);

                    ViewBag.Role = RoleSelectList;

                    #endregion Fill dropdownlist - (Same as Index method pattern)

                    #region Convert UserDTO record into User model.

                    User user = new User();

                    user.UserId = userDTO.UserId;
                    user.RoleId = userDTO.RoleId;
                    user.Username = userDTO.Username;
                    user.Password = userDTO.Password;
                    user.Name = userDTO.Name;
                    user.Phone = userDTO.Phone;
                    user.ProfilePicture = userDTO.ProfilePicture;
                    user.StartDate = userDTO.StartDate;
                    user.EndDate = userDTO.EndDate;
                    user.ActiveStatus = userDTO.ActiveStatus;

                    #endregion Convert UserDTO record into User model.

                    return View(user);
                }

                return RedirectToAction("Index", "User");

            }

            return RedirectToAction("Index", "User");

        }

        #endregion Set Data at time of edit record

        #region When username is already inserted than raised valiation by REMOTE method

        public JsonResult UserNameExists(int? UserId, string UserName)
        {
            bool ReturnValue = false;

            if (UserId == null)
            {
                #region Check only username at time of insert 

                ReturnValue = !KCS_dbContext.Users.ToList().Exists(x => x.Username.Equals(UserName, StringComparison.CurrentCultureIgnoreCase));

                #endregion Check only username at time of insert 
            }
            else
            {
                #region Check username with current user id at time of update

                ReturnValue = !KCS_dbContext.Users.ToList().Exists(x => x.Username.Equals(UserName, StringComparison.CurrentCultureIgnoreCase) && x.UserId!=UserId);

                #endregion Check username with current user id at time of update
            }

            return Json(ReturnValue);
        }

        #endregion When username is already inserted than raised valiation by REMOTE method

        [HttpPost]
        public ActionResult Edit(User userModel)
        {
            try
            {
                #region Fill Role Dropdownlist -- (Same as Index methods pattern)

                List<RoleDTO> Roles = new List<RoleDTO>();

                if (ViewBag.UserInformation.RoleId == 1)
                {
                    Roles = (from role in KCS_dbContext.Roles
                             select new RoleDTO
                             {
                                 RoleId = role.RoleId,
                                 RoleName = role.RoleName
                             }).OrderBy(x => x.RoleName).ToList();
                }
                else
                {
                    Roles = (from role in KCS_dbContext.Roles
                             where role.RoleId != 1
                             select new RoleDTO
                             {
                                 RoleId = role.RoleId,
                                 RoleName = role.RoleName
                             }).OrderBy(x => x.RoleName).ToList();
                }

                SelectList RoleSelectList = new SelectList(Roles, "RoleId", "RoleName");

                ViewBag.Role = RoleSelectList;

                #endregion Fill Role Dropdownlist -- (Same as Index methods pattern)

                #region Add Date validation

                KeyValuePair<bool, string> IsDateValid = Check_Date_Validation(Convert.ToDateTime(userModel.StartDate), userModel.EndDate, userModel.ActiveStatus);

                if (IsDateValid.Key==false)
                {
                    ModelState.AddModelError("", IsDateValid.Value);
                }

                #endregion Add Date validation

                if (ModelState.IsValid)
                {
                    #region Get user record by userId for updating entity.

                    User user = KCS_dbContext.Users.FirstOrDefault(x => x.UserId == userModel.UserId);

                    #endregion Get user record by userId for updating entity.

                    if (user != null)
                    {
                        #region Update user entity by POST data

                        user.UserId = userModel.UserId;
                        user.Username = userModel.Username;
                        user.Password = Encryptor.Encrypt(userModel.Password);
                        user.Name = userModel.Name;
                        user.Phone = userModel.Phone;
                        user.RoleId = userModel.RoleId;
                        user.StartDate = userModel.StartDate != null ? Convert.ToDateTime(userModel.StartDate) : (DateTime?)null;
                        user.EndDate = userModel.EndDate != null ? Convert.ToDateTime(userModel.EndDate) : (DateTime?)null;

                        #region When profile picture is uploaded in file upload than convert tham in byte[]

                        var ProfilePicturePath = Request.Files["ProfilePicturePath"];
                        if (ProfilePicturePath != null && ProfilePicturePath.ContentLength > 0)
                        {
                            userModel.ProfilePicture = new byte[ProfilePicturePath.ContentLength];
                            ProfilePicturePath.InputStream.Read(userModel.ProfilePicture, 0, ProfilePicturePath.ContentLength);
                            user.ProfilePicture = userModel.ProfilePicture;
                        }

                        #endregion When profile picture is uploaded in file upload than convert tham in byte[]

                        else
                        {
                            #region If user removed their profile picture which is added earlier than add null in profile picture field.

                            if (userModel.IsImageDeleted == true)
                            {
                                user.ProfilePicture = null;
                            }

                            #endregion If user removed their profile picture which is added earlier than add null in profile picture field.
                        }

                        user.ActiveStatus = userModel.ActiveStatus;
                        user.ModifiedOn = DateTime.Now;

                        KCS_dbContext.SaveChanges();

                        #endregion Update user entity by POST data

                        TempData["SuccessMessage"] = Constants.SuccessMessageUpdate;

                        return RedirectToAction("Index", "User");
                    }

                    return View(userModel);
                }
                else
                {
                    return View(userModel);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException.InnerException.Message);
                return View(userModel);
            }
        }

        #region Check date validation at time of insert and update (Server side validation)

        public KeyValuePair<bool, string> Check_Date_Validation(DateTime StartDate,  Nullable<DateTime> EndDate, bool ActiveStatus)
        {
            KeyValuePair<bool, string> ReturnValue = new KeyValuePair<bool, string>();

            #region Validation 1 : When User status is inactive than user must have to add end date.

            if (ActiveStatus==false && EndDate==null)
            {
                ReturnValue = new KeyValuePair<bool, string>(false, "You must have to enter end date while active status is inactive.");
                
                return ReturnValue;
            }

            #endregion Validation 1 : When User status is inactive than user must have to add end date.

            #region Validation 2 : When User status is active than end date must be null.

            else if (ActiveStatus == true && EndDate != null)
            {
                ReturnValue = new KeyValuePair<bool, string>(false, "You can not set end date till user active status is inactive.");

                return ReturnValue;
            }

            #endregion Validation 2 : When User status is active than end date must be null.

            #region Validation 3 : When User status is inactive than end date must be >= start date.

            else if (EndDate != null && (EndDate<StartDate))
            {
                ReturnValue = new KeyValuePair<bool, string>(false, "End date must be >= start date.");

                return ReturnValue;
            }

            #endregion Validation 3 : When User status is inactive than end date must be >= start date.
            else
            {
                #region Return success and true if all conditions are valid.

                ReturnValue = new KeyValuePair<bool, string>(true, "Success.");

                return ReturnValue;

                #endregion Return success and true if all conditions are valid.
            }
        }

        #endregion Check date validation at time of insert and update  (Server side validation)

        [HttpPost]
        public ActionResult Delete(int Id)
        {
            try
            {
                #region Delete User

                User user = KCS_dbContext.Users.Find(Id);
                KCS_dbContext.Users.Remove(user);
                KCS_dbContext.SaveChanges();

                return Json(new { result = "success", success = Constants.SuccessMessageDelete }, JsonRequestBehavior.AllowGet);

                #endregion Delete User
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
                return Json(new { result = "error", success = Constants.ErrorMessageDelete }, JsonRequestBehavior.AllowGet);
            }            
        }

        #region Set Data at time of edit record

        [HttpGet]
        public ActionResult UserProfile(int id)
        {
            if (id > 0)
            {
                #region If UserId is other than logged in user id than redirect to custom error page

                if(id!=ViewBag.UserInformation.UserId)
                {
                    return RedirectToAction("ErrorPage", "Home");

                }
                #endregion If UserId is other than logged in user id than redirect to custom error page

                #region Get User record for update.

                UserDTO userDTO = ((from user in KCS_dbContext.Users
                                    where user.UserId == id
                                    select new UserDTO
                                    {
                                        UserId = user.UserId,
                                        RoleId = user.RoleId,
                                        Username = user.Username,
                                        Password = user.Password,
                                        Name = user.Name,
                                        Phone = user.Phone,
                                        ProfilePicture = user.ProfilePicture,
                                        StartDate = user.StartDate,
                                        EndDate = user.EndDate,
                                        ActiveStatus = user.ActiveStatus
                                    })).FirstOrDefault();

                #endregion Get User record for update.

                if (userDTO != null)
                {
                    #region Decrypt the password

                    userDTO.Password = Encryptor.Decrypt(userDTO.Password);

                    #endregion Decrypt the password

                    #region Fill dropdownlist - (Same as Index method pattern)

                    List<RoleDTO> Roles = new List<RoleDTO>();

                    if (ViewBag.UserInformation.RoleId == 1)
                    {
                        Roles = (from role in KCS_dbContext.Roles
                                 select new RoleDTO
                                 {
                                     RoleId = role.RoleId,
                                     RoleName = role.RoleName
                                 }).OrderBy(x => x.RoleName).ToList();
                    }
                    else
                    {
                        Roles = (from role in KCS_dbContext.Roles
                                 where role.RoleId != 1
                                 select new RoleDTO
                                 {
                                     RoleId = role.RoleId,
                                     RoleName = role.RoleName
                                 }).OrderBy(x => x.RoleName).ToList();
                    }

                    SelectList RoleSelectList = new SelectList(Roles, "RoleId", "RoleName", userDTO.RoleId);

                    ViewBag.Role = RoleSelectList;

                    #endregion Fill dropdownlist - (Same as Index method pattern)

                    #region Convert UserDTO record into User model.

                    User user = new User();

                    user.UserId = userDTO.UserId;
                    user.RoleId = userDTO.RoleId;
                    user.Username = userDTO.Username;
                    user.Password = userDTO.Password;
                    user.Name = userDTO.Name;
                    user.Phone = userDTO.Phone;
                    user.ProfilePicture = userDTO.ProfilePicture;
                    user.StartDate = userDTO.StartDate;
                    user.EndDate = userDTO.EndDate;
                    user.ActiveStatus = userDTO.ActiveStatus;

                    #endregion Convert UserDTO record into User model.

                    return View(user);
                }

                return RedirectToAction("ErrorPage", "Home");

            }

            return RedirectToAction("ErrorPage", "Home");

        }

        #endregion Set Data at time of edit record


        [HttpPost]
        public ActionResult UserProfile(User userModel)
        {
            try
            {
                #region Fill Role Dropdownlist -- (Same as Index methods pattern)

                List<RoleDTO> Roles = new List<RoleDTO>();

                if (ViewBag.UserInformation.RoleId == 1)
                {
                    Roles = (from role in KCS_dbContext.Roles
                             select new RoleDTO
                             {
                                 RoleId = role.RoleId,
                                 RoleName = role.RoleName
                             }).OrderBy(x => x.RoleName).ToList();
                }
                else
                {
                    Roles = (from role in KCS_dbContext.Roles
                             where role.RoleId != 1
                             select new RoleDTO
                             {
                                 RoleId = role.RoleId,
                                 RoleName = role.RoleName
                             }).OrderBy(x => x.RoleName).ToList();
                }

                SelectList RoleSelectList = new SelectList(Roles, "RoleId", "RoleName");

                ViewBag.Role = RoleSelectList;

                #endregion Fill Role Dropdownlist -- (Same as Index methods pattern)

                #region Add Date validation

                KeyValuePair<bool, string> IsDateValid = Check_Date_Validation(Convert.ToDateTime(userModel.StartDate), userModel.EndDate, userModel.ActiveStatus);

                if (IsDateValid.Key == false)
                {
                    ModelState.AddModelError("", IsDateValid.Value);
                }

                #endregion Add Date validation

                if (ModelState.IsValid)
                {
                    #region Get user record by userId for updating entity.

                    User user = KCS_dbContext.Users.FirstOrDefault(x => x.UserId == userModel.UserId);

                    #endregion Get user record by userId for updating entity.

                    if (user != null)
                    {
                        #region Update user entity by POST data

                        user.UserId = userModel.UserId;
                        user.Username = userModel.Username;
                        user.Password = Encryptor.Encrypt(userModel.Password);
                        user.Name = userModel.Name;
                        user.Phone = userModel.Phone;
                        user.RoleId = userModel.RoleId;
                        user.StartDate = userModel.StartDate != null ? Convert.ToDateTime(userModel.StartDate) : (DateTime?)null;
                        user.EndDate = userModel.EndDate != null ? Convert.ToDateTime(userModel.EndDate) : (DateTime?)null;

                        #region When profile picture is uploaded in file upload than convert tham in byte[]

                        var ProfilePicturePath = Request.Files["ProfilePicturePath"];
                        if (ProfilePicturePath != null && ProfilePicturePath.ContentLength > 0)
                        {
                            userModel.ProfilePicture = new byte[ProfilePicturePath.ContentLength];
                            ProfilePicturePath.InputStream.Read(userModel.ProfilePicture, 0, ProfilePicturePath.ContentLength);
                            user.ProfilePicture = userModel.ProfilePicture;
                        }

                        #endregion When profile picture is uploaded in file upload than convert tham in byte[]

                        else
                        {
                            #region If user removed their profile picture which is added earlier than add null in profile picture field.

                            if (userModel.IsImageDeleted == true)
                            {
                                user.ProfilePicture = null;
                            }

                            #endregion If user removed their profile picture which is added earlier than add null in profile picture field.
                        }

                        user.ActiveStatus = userModel.ActiveStatus;
                        user.ModifiedOn = DateTime.Now;

                        KCS_dbContext.SaveChanges();

                        #endregion Update user entity by POST data

                        TempData["SuccessMessage"] = Constants.SuccessMessageUpdate;

                        return RedirectToAction("Index", "Home");
                    }

                    return View(userModel);
                }
                else
                {
                    return View(userModel);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException.InnerException.Message);
                return View(userModel);
            }
        }

    }
}