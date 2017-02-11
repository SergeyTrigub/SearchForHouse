using Autofac;
using Autofac.Integration.Mvc;
using Owin;
using SH.Api.Infrastructure;
using SH.Contracts;
using SH.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace SH.Web
{
    public partial class Startup
    {
        public void ConfigureAutofacMvc(IAppBuilder app)
        {
            var builder = new ContainerBuilder();

            // STANDARD MVC SETUP:

            builder.RegisterModule<WebApiServiceModule>();

            // Register your MVC controllers.
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            builder.RegisterType<CountryService>().As<ICountryService>().InstancePerRequest();

            // Run other optional steps, like registering model binders,
            // web abstractions, etc., then set the dependency resolver
            // to be Autofac.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            // OWIN MVC SETUP:

            // Register the Autofac middleware FIRST, then the Autofac MVC middleware.
            app.UseAutofacMiddleware(container);
            app.UseAutofacMvc();
        }

    }
}