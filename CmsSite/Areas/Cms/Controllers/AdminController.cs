﻿using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using CmsSite.Areas.Cms.Extensions;
using CmsSite.Models;

namespace CmsSite.Areas.Cms.Controllers
{

    //[Authorize(Roles = "Admin, Editor")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;        //standard i alle vores controllere
        private readonly IdentityService _identityService;


        public AdminController()
        {
            _context = new ApplicationDbContext();
            _identityService = IdentityService.Create(_context);
        }
        // GET: Cms/Admin
        public ActionResult Index()
        {
            return View();
        }


        //List Users

        public ActionResult Users()
        {
            //var users = _context.Users.ToList();
            var users = _identityService.GetUsers();
          
            return View("Users", users);
        }

        //Create User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(ApplicationUser user, string password)
        {
            if (string.IsNullOrEmpty(user.PhoneNumber))           
                _identityService.CreateUser(user.UserName, user.Email, password);            
            else          
                 _identityService.CreateUser(user.UserName, user.Email, password, user.PhoneNumber);                          
            return RedirectToAction("Users");
        }

        //Delete User

        public ActionResult DeleteUser(string id)
        {
           _identityService.DeleteUser(id);
            return RedirectToAction("Users");
        }

        //Edit User

        //update af de ting der skal være til brugeren
        
        //Update User Roles
        public bool UpdateUserRole(string userid, string rolename, bool add = true)
        {
            //toggle user on/off
            if (add)
            {
                _identityService.AddUserToRole(userid, rolename);
            }
            else
            {
                _identityService.RemoveUserFromRole(userid, rolename);
            }

            return true;
        }


        //Create Role

        public ActionResult CreateRole(string rolename)
        {
            if (!_identityService.RoleExists(rolename))
            {
                _identityService.CreateRole(rolename);

            }
            return Redirect("Users");
        }

        //Delete Role

        //Edit Role

    

        


        //List Pages
        public ActionResult Pages()
        {

            var pages = _context.CmsPages
                .Include(p => p.Parent)
                .Include(p => p.Template)
                .Where(p => p.ParentId != null).ToList();

            ViewBag.ParentId = new SelectList(_context.CmsPages.Where(p => p.ParentId != null), "PageId", "Name");
            ViewBag.TemplateId = new SelectList(_context.CmsTemplates, "TemplateId", "Name");
            return View(pages);
        }

        //Create Page

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePage(CmsPage page)
        {
            if (ModelState.IsValid)
            {
                var parent = _context.CmsPages.Find(page.ParentId);
                page.Parent = parent;
                //refactor names (properties) (Niceurl)
                page.Url = parent.Url + (parent.Url == "/" ? "" : "/") + AdminHelper.ToUrlName(page.Name);
                page.Alias = AdminHelper.ToUrlName(page.Name);

                page.Level = parent.Level + 1;

                _context.CmsPages.Add(page);
                _context.SaveChanges();

                return RedirectToAction("Pages");                               //refresh view og laver en ny liste
            }

            ViewBag.ParentId = new SelectList(_context.CmsPages.Where(p => p.ParentId != null), "PageId", "Name");
            ViewBag.TemplateId = new SelectList(_context.CmsTemplates, "TemplateId", "Name");
            //ViewBag.Icons = new SelectList(_context.CmsPages, "Iconname");
            return View(page);
        }

        //Edit Page
        [HttpGet]
    
        public ActionResult EditPage(int id)
        {
            var page = _context.CmsPages
                .Include(p => p.Properties)
                .Include(p => p.Template)
                .FirstOrDefault(p => p.PageId == id);
            return View(page);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditPage(CmsPage page, FormCollection form)
        {
            var oldProperties = _context.CmsProperties.Where(p => p.PageId == page.PageId).ToList();
            foreach (var prop in oldProperties)
            {
                for (int i = 0; i < form.Keys.Count; i++)                  //for at få indekseret vores properties index
                {
                    if (form.GetKey(i) != prop.Alias) continue;
                    //If key and prop equals 
                    prop.Value = form[i];
                    _context.Entry(prop).State = EntityState.Modified;

                }
            }
            _context.Entry(page).State = EntityState.Modified;
            _context.SaveChanges();

            return RedirectToAction("EditPage", new {id = page.PageId});
        }
        //Delete Page

        public ActionResult DeletePage(int id)
        {
            var page = _context.CmsPages.Find(id);
            if (page != null)
            {
                var properties = _context.CmsProperties.Where(p => p.PageId == page.PageId).ToList();
                _context.CmsProperties.RemoveRange(properties);

                // remove children
                //måske

                _context.CmsPages.Remove(page);
                _context.SaveChanges();
            }

            return RedirectToAction("Pages");
        }


        //Partial View for Templates List

        public ActionResult TemplatePartial()
        {

            var path = Server.MapPath("/Views/Page/Pages");                     //retunerer serverens fyskiske sti

            DirectoryInfo dirinfo = new DirectoryInfo(path);
            List<FileInfo> files = dirinfo.GetFiles().ToList();                     //for at få en liste over de fysiske filer så vi kan navigerer rundt i dem

            return PartialView("_TemplateTree", files);
        }



        //Templates Action
        public ActionResult Templates(string templateName)
        {
            if (string.IsNullOrEmpty(templateName))
            {
                return View("Templates", new MvcHtmlString(string.Empty));                      //modsat string er dette en string med metadata - altså en mere komplicerret form for en string
            }

            var fileContent = System.IO.File.ReadAllText(Server.MapPath("/Views/Page/Pages/" + templateName + ".cshtml"));
            return View("Templates", MvcHtmlString.Create(fileContent));
        }

        //Delete Template

        [HttpGet]
        public void DeleteTemplate(string templateName)
        {
            var path = Server.MapPath("/Views/Page/Pages/" + templateName + ".cshtml");


            var template = _context.CmsTemplates.FirstOrDefault(x => x.Alias.Equals(templateName));
            var pages = _context.CmsPages.Where(p => p.TemplateId == template.TemplateId).ToList();

            if (pages.Count == 0)
            {
                //delete template
                System.IO.File.Delete(path);
                _context.CmsTemplates.Remove(template);
                _context.SaveChanges();
            }

            else
            {
                //give notification

            }
           
        }


        //Save Template 
        [HttpPost]
        [ValidateInput(false)]
        public string SaveTemplate(string templateName, string html)
        {
            if (string.IsNullOrEmpty(templateName) || string.IsNullOrEmpty(html))
            {
                //fejl GØR NOGET

            }

            var path = Server.MapPath("/Views/Page/Pages/" + templateName + ".cshtml");

            System.IO.File.WriteAllText(path, html);

            //if cmstemplate not Exists in db

            var exists = _context.CmsTemplates.FirstOrDefault(x => x.Alias == templateName);
            if (exists == null)
            {
                _context.CmsTemplates.Add(
                   new CmsTemplate
                   {
                       Name = templateName,
                       Alias = templateName.ToLower()
                   });

                _context.SaveChanges();

            }

            return templateName;
        }

        //Create Template
        [HttpGet]
        public ActionResult CreateTemplate()
        {
            var html = System.IO.File.ReadAllText(Server.MapPath("/Views/Shared/HtmlNewTemplate.cshtml"));
            return View("CreateTemplate", new MvcHtmlString(html));
        }

        public ActionResult Template()
        {
            return View();
        }


        //Create property
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateProperty(CmsProperty property)
        {
            //set alias
            property.Alias = AdminHelper.ToUrlName(property.Name);
            if (ModelState.IsValid)
            {
                //save in db
                _context.CmsProperties.Add(property);
                _context.SaveChanges();
            }

            return RedirectToAction("EditPage", new {id = property.PageId});
        }



        //Delete property
        public ActionResult DeleteProperty( int id )
        {
            var prop = _context.CmsProperties.Find(id);
            _context.CmsProperties.Remove(prop);
            _context.SaveChanges();
            return RedirectToAction("EditPage", new { id = prop.PageId });
        }


    }
}