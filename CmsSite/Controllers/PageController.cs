﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsSite.Models;

namespace CmsSite.Controllers
{
    public class PageController : Controller
    {
        private readonly CmsApi _cmsApi;

        public PageController()
        {
           _cmsApi = new CmsApi(ApplicationDbContext.Create());
        }

      

        public ActionResult Index(string route)
        {
            var currentPage = _cmsApi.GetCurrentPage(route);

            if (route != null)
            {
                var routes = route.Split(new[] {"/"}, StringSplitOptions.None);
                if (routes[0] == "Account")
                {
                    var controller = new AccountController();
                    controller.ActionInvoker.InvokeAction(ControllerContext, routes[1]);
                }

                if (routes[0]== "Manage")
                {
                    var controller = new ManageController();
                    controller.ActionInvoker.InvokeAction(ControllerContext, routes[1]);
                }

            }

            return route != null ? View("Pages/" + currentPage.Template, currentPage) : View("Pages/Index",currentPage);
        }
    }
}