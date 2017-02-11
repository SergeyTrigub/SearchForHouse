using Autofac;
using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;

namespace SH.Api.Infrastructure
{
	public class WebApiServiceModule : Module
    {
        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            var type = registration.Activator.LimitType;
            if (HasPropertyDependencyOnLogger(type))
            {
                registration.Activated += InjectLoggerViaProperty;
            }

            if (HasConstructorDependencyOnLogger(type))
            {
                registration.Preparing += InjectLoggerViaConstructor;
            }
        }

        private bool HasPropertyDependencyOnLogger(Type type)
        {
            return type.GetProperties().Any(property => property.CanWrite && property.PropertyType == typeof(IPrincipal));
        }

        private bool HasConstructorDependencyOnLogger(Type type)
        {
            return type.GetConstructors()
                       .SelectMany(constructor => constructor.GetParameters()
                                                             .Where(parameter => parameter.ParameterType == typeof(IPrincipal)))
                       .Any();
        }

        private void InjectLoggerViaProperty(object sender, ActivatedEventArgs<object> @event)
        {
            var type = @event.Instance.GetType();
            var propertyInfo = type.GetProperties().First(x => x.CanWrite && x.PropertyType == typeof(IPrincipal));
            propertyInfo.SetValue(@event.Instance, Thread.CurrentPrincipal, null);
        }

        private void InjectLoggerViaConstructor(object sender, PreparingEventArgs @event)
        {
            var type = @event.Component.Activator.LimitType;
            @event.Parameters = @event.Parameters.Union(new[]
            {
                new ResolvedParameter((parameter, context) => parameter.ParameterType == typeof(IPrincipal), (p, i) => Thread.CurrentPrincipal)
            });
        }
    }
}