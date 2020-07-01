using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExampleMVC.Models;
using System.IO;

namespace ExampleMVC.Controllers
{
    public class HomeController : Controller
    {
        INVENTORYMNGDBEntities CompanyDB = new INVENTORYMNGDBEntities();
        public ActionResult Index()
        {
            try
            {
                ViewBag.CompanyInfo_result = CompanyDB.spGetCompanyInformation(1).ToList();
            }
            catch { }

            return View();
        }

        public PartialViewResult _InsertUpdateCompany(int CID)
        {
            //CID == CompanyID

            spGetCompanyInformation_Result CompanyInfo_Result = new spGetCompanyInformation_Result();
            var CompanyInfo = CompanyDB.spGetCompanyInformation(CID).FirstOrDefault();

            if (CompanyInfo == null)
            { CompanyInfo_Result.companyID = 0; }
            else { CompanyInfo_Result = CompanyInfo; }

            //ViewBag.Country_result = INVENTORYMNGDB.spGetCountry().ToList();

            return PartialView("_InsertUpdateCompany", CompanyInfo_Result);
        }

        public byte[] ConvertToBytes(HttpPostedFileBase document)
        {
            byte[] imageBytes = null;
            BinaryReader reader = new BinaryReader(document.InputStream);
            imageBytes = reader.ReadBytes((int)document.ContentLength);
            return imageBytes;
        }

        [HttpPost]
        [ValidateInput(false)]
        //public JsonResult _InsertUpdateCompanyInfo(spGetCompanyInformation_Result fileData) // Installunobtructive
        public ActionResult _InsertUpdateCompanyInfo(spGetCompanyInformation_Result fileData)
        {
            var CreatedBy = "1";
            var companyID = 0;
            var result = "";

            if (fileData.companyID > 0)
            {
                if (Request.Files.Count > 0)
                {
                    byte[] returnimageBytes = null;
                    string filename = "";
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        filename = Path.GetFileName(Request.Files[i].FileName);                        
                        HttpPostedFileBase file = files[i];
                        if (filename != null && filename != "")
                        {
                            returnimageBytes = ConvertToBytes(file);
                        }
                        else
                        {
                            returnimageBytes = null;
                        }

                        companyID = (int)CompanyDB.spCompanyUpdate(fileData.companyID, fileData.countryID, fileData.companyName, 
                            fileData.companyAddress, fileData.companyContact, fileData.companyEmail, fileData.companyPhone, fileData.companyFax,
                            returnimageBytes, int.Parse(CreatedBy)).FirstOrDefault().Value;
                        result = "Updated";
                    }
                }
                else
                {
                    companyID = (int)CompanyDB.spCompanyUpdate(fileData.companyID, fileData.countryID, fileData.companyName,
                            fileData.companyAddress, fileData.companyContact, fileData.companyEmail, fileData.companyPhone, fileData.companyFax,
                            null, int.Parse(CreatedBy)).FirstOrDefault().Value;
                    result = "Updated";
                }

            }
            else
            {
                if (Request.Files.Count > 0)
                {
                    byte[] returnimageBytes = null;
                    string filename = "";
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        filename = Path.GetFileName(Request.Files[i].FileName);                        
                        HttpPostedFileBase file = files[i];
                        if (filename != null && filename != "")
                        {
                            returnimageBytes = ConvertToBytes(file);
                        }
                        else
                        {
                            returnimageBytes = null;
                        }

                        companyID = (int)CompanyDB.spCompanyInsert(fileData.countryID, fileData.companyName,
                            fileData.companyAddress, fileData.companyContact, fileData.companyEmail, fileData.companyPhone, fileData.companyFax,
                            returnimageBytes, int.Parse(CreatedBy)).FirstOrDefault().Value;
                        result = "Inserted";
                    }                    
                }
                else
                {
                    companyID = (int)CompanyDB.spCompanyInsert(fileData.countryID, fileData.companyName,
                        fileData.companyAddress, fileData.companyContact, fileData.companyEmail, fileData.companyPhone, fileData.companyFax,
                        null, int.Parse(CreatedBy)).FirstOrDefault().Value;
                    result = "Inserted";
                }
            }
            return RedirectToAction("Index", "Home");
            //return Json(new { companyID = companyID, Message = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}