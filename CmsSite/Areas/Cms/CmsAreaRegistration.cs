using System.Web.Mvc;

namespace CmsSite.Areas.Cms
{
    public class CmsAreaRegistration : AreaRegistration 
    {
        public override string AreaName => "Cms"; // der kan sættes @ foran så bliver den mere eksplicit ?!?

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Cms_default",
                "Cms/{controller}/{action}/{templateName}",
                new { action = "Index", templateName = UrlParameter.Optional }
            );
        }
    }
}