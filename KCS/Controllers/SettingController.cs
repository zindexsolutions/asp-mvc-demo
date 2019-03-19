using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KCS.DTOs;
using KCS.Models;

namespace KCS.Controllers
{
    public class SettingController : BaseController
    {
        KCSEntities KCS_dbContext = new KCSEntities();

        // GET: Setting
        public ActionResult Index()
        {
            try
            {
                #region Get settings record (Currently only one record LogoImage)

                var Settings = (from setting in KCS_dbContext.Settings
                                select new SettingDTO
                                {
                                    SettingId = setting.SettingId,
                                    Name = setting.Name,
                                    Image = setting.Image
                                }).FirstOrDefault();

                #endregion Get settings record (Currently only one record LogoImage)

                if (Settings != null)
                {
                    #region Convert Dto object into model 

                    Setting setting = new Setting();
                    setting.SettingId = Settings.SettingId;
                    setting.Name = Settings.Name;
                    setting.Image = Settings.Image;

                    #endregion Convert Dto object into model 

                    return View(setting);
                }
            }
            catch
            {
                
            }
            return RedirectToAction("ErrorPage", "Home");

        }

        public ActionResult Save(Setting settingModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    #region Update settings if model is valid 

                    Setting setting = KCS_dbContext.Settings.FirstOrDefault(x => x.SettingId == settingModel.SettingId);

                    if (setting != null)
                    {
                        setting.SettingId = settingModel.SettingId;
                        setting.Name = settingModel.Name;
                        
                        var LogoImagePath = Request.Files["LogoImagePath"];

                        #region when image is uploded in file upload

                        if (LogoImagePath != null && LogoImagePath.ContentLength > 0)
                        {
                            settingModel.Image = new byte[LogoImagePath.ContentLength];
                            LogoImagePath.InputStream.Read(settingModel.Image, 0, LogoImagePath.ContentLength);
                            setting.Image = settingModel.Image;
                        }

                        #endregion when image is uploded in file upload

                        else
                        {
                            #region If image is added earlier and remove now than pass null in image

                            if (settingModel.IsImageDeleted == true)
                            {
                                setting.Image = null;
                            }

                            #endregion If image is added earlier and remove now than pass null in image
                        }

                        KCS_dbContext.SaveChanges();

                        #endregion Update settings if model is valid 

                        #region Add Success message as tempdata and reload the current action.

                        TempData["SuccessMessage"] = Constants.SuccessMessageUpdate;

                        return RedirectToAction("Index", "Setting");

                        #endregion Add Success message as tempdata and reload the current action.
                    }

                    #region If any errors are there than add error in modelstate and reload the view.

                    ModelState.AddModelError("", "Record not found, try again.");

                    return View(settingModel);

                    #endregion If any errors are there than add error in modelstate and reload the view.
                }
                else
                {
                    return View(settingModel);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.InnerException.InnerException.Message);
                return View(settingModel);
            }
        }
    }
}