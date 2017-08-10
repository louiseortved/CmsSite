using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsSite.Models;
using CmsSite.ViewModels;
using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Services.Protocols;
using Microsoft.Owin.Security;


public class CmsApi
{
    private static ApplicationDbContext _context;
        //poorman dependency injection - vi bruger den fattige måde da kunden ikke har råd til at vi køber denne metode - deraf poorman :)

    public CmsApi(ApplicationDbContext context)
    {
        _context = context;
    }

    //Return As MvcHtmlstring
    public static MvcHtmlString GetProperty(PageViewModel page, string alias)
    {
        return new MvcHtmlString(GetPropertyByAlias(page.PageId, alias));
       
    }

    private static string GetPropertyByAlias(int id, string alias)
    {
        return _context.CmsProperties.FirstOrDefault(p => p.Alias == alias && p.PageId == id)?.Value;
    }

    //Return IEnumerable <dynamic> GetPages

    public static IEnumerable<dynamic> GetPages()
    {
        var pageList = new List<dynamic>();
        if (!_context.CmsPages.Any())
        {
            return pageList;
        }

        pageList.AddRange(_context.CmsPages
                .Include(p => p.Children)
                .Include(x => x.Parent)
                .Include(x => x.Template)
                .Where(p => p.Level > 0)
                .ToList()
        );
        return pageList;
    }


    //Return RootPage

    public static dynamic GetRootPage()
    {
        var root = Pages()     
            .FirstOrDefault(p => p.Url == "/");
        return (dynamic) root;
    }

    //Return PageById

    public static dynamic GetPageById(int id)
    {
        var page = Pages()
            .FirstOrDefault(p => p.PageId == id);
        return GetViewModel(page);
    }

    //return GetPageByAlias
    public static dynamic GetPageByAlias(string alias)
    {
        var page = Pages()
            .FirstOrDefault(p => p.Alias == alias);
        return GetViewModel(page);
    }

    //GetPageList lav en liste af vores GetPages (søgefunktion)

    //GetPagesByParentId

    //Return Dynamic CurrentPage
    public dynamic GetCurrentPage(string url)
    {
        if (url == null)
        {
            var currentPage = Pages()
                .FirstOrDefault(page => page.Url == "/");
            var viewModel = GetViewModel(currentPage);
            return (dynamic) viewModel;
        }

        else
        {
            var currentPage = Pages()
                .FirstOrDefault(page => page.Url == "/" + url);
            var viewModel = GetViewModel(currentPage);
            return (dynamic) viewModel;
        }
    }



    //  Private GetPages

    private static IEnumerable<dynamic> Pages()
    {
        var pageList = new List<dynamic>();
        if ( !  _context.CmsPages.Any())
        
        
        {
            return pageList;
        }
        pageList.AddRange(_context.CmsPages
             .Include(p => p.Children)
             .Include( p => p.Properties)
            .Include(x => x.Parent)
            .Include(x => x.Template)
            );

        return pageList;
    }

    //create vm from cmspage
    private static PageViewModel GetViewModel(CmsPage currentPage)
    {
        var vm = new PageViewModel
        {
            PageId = currentPage.PageId,
            Alias = currentPage.Alias,
            Name = currentPage.Name,
            IconName = currentPage.IconName,
            Template = currentPage.Template.Name,
            ShowInMenu = currentPage.ShowInMenu,
            CreateDate = currentPage.CreateDate,
            Level = currentPage.Level,
            Order = currentPage.Order,
            Parent = currentPage.Parent,
            //Properties = currentPage.Properties,
            //Children,
            Url = currentPage.Url

        };
        return vm;
    }



    public static IEnumerable<dynamic> GetMenuPages()
    {
        var pagelist = new List<dynamic>();


        if (_context.CmsPages.Any(p => p.ShowInMenu))
        {
            pagelist.AddRange(_context.CmsPages
                .Where(x => x.Level == 2)
                .Include(p => p.Parent)
                .Include(p => p.Children)
                .Where(p => p.ShowInMenu));
        }
        return pagelist;
    }



    public static IEnumerable<dynamic> GetChildPages(int id)
    {
        var childpagelist = new List<dynamic>();
      

        if (_context.CmsPages.Any(p => p.ShowInMenu))
        {
            childpagelist.AddRange(_context.CmsPages
                .Where(x => x.Level >= 2)
                .Where( x => x.ParentId == id)             
                .ToList());
              
        }

        return childpagelist; ;




        //public static IEnumerable<dynamic> GetChildPages(/*int level*/)
        //{
        //    var childpagelist = new List<dynamic>();

        //    if(_context.CmsPages.Any(p => p.ShowInMenu))
        //    {
        //        childpagelist.AddRange(_context.CmsPages
        //            .Include(c => c.Children)
        //            .Where(p => p.ShowInMenu));
        //    }

        //    return childpagelist;;

        //if (_context.CmsPages.Where(p => p.ShowInMenu))
        //{
        //    pagelist.Add(_context.CmsPages.Find();
        //}

        //return pagelist;

        //CHILDREN PROPERTY PAGES LEVEL > 1

    }



    //public static GetPages(int id)
    //{

    //}



}