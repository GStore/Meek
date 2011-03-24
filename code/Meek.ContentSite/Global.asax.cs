﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Meek.Storage;
using Ninject;

namespace Meek.ContentSite
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Error()
        {
            var error = Server.GetLastError();
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);


            var kernel = new StandardKernel();
            kernel.Bind<Repository>().To<InMemoryRepository>().InSingletonScope();
            DependencyResolver.SetResolver(new NinjectResolver(kernel));

            //Do this to setup the test data before Initialize() for automated tests using in memory repo
            SetupRepository(kernel.Get<Repository>());

            BootStrapper.Initialize();
            //Configuration.Configuration.Initialize(repository, new BasicAuthorization(x => x.User.IsInRole("Content Admin")), "Missing");
        }

        protected void SetupRepository(Repository repository)
        {
            repository.Save("A/Junk/Route", new MeekContent("Junk", "route table padding", false));
            repository.Save("some/existing/content", new MeekContent("An existing title", "Existing HTML content", false));
            repository.Save("content/for/edit", new MeekContent("An existing title", "Existing HTML content", false));
            repository.Save("content/for/delete", new MeekContent("An existing title", "Existing HTML content to delete", false));
            repository.Save("A/Partial/Page", new MeekContent(null, "Existing partial content", true));
            repository.Save("Partial/For/Edit", new MeekContent(null, "Existing partial content to edit", true));
            repository.Save("Another/Junk/Route", new MeekContent(null, "route table padding", true));
        }

    }
}