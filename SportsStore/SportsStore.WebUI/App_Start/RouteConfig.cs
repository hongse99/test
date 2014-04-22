using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SportsStore.WebUI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(null,
                "", //  비어있는  url 하고만 매치된다. (예:/)
                new { controller = "Product", action="List", category=(string)null, page=1 }
             );

            routes.MapRoute(
                null, // 라우트명은 지정하지 않아도 된다.
                "Page{page}", // /Page2, /Page123 에는 매치되지만 /PageXYZ 에는 매치되지 않는다.
                new { Controller = "Product", action = "List", category = (string)null },
                new { page=@"\d+" } // 제약조건: 페이지는 숫자이어야 한다.
            );

            routes.MapRoute(
                null, 
                "{category}", // /Football  이나 /AnythingNoSlash 와 매치 된다.
                new { Controller = "Product", action = "List", page=1 }
                
            );

            routes.MapRoute(
                null, // 라우트명은 지정하지 않아도 된다.
                "{category}/Page{page}", // /Football/Page567 과 매치 
                new { Controller = "Product", action = "List" }, //기본값 설정
                new { page = @"\d+" } // 제약조건: 페이지는 숫자이어야 한다.
            );

            routes.MapRoute(null, "{controller}/{action}");
           
        }
    }
}