﻿using Autofac;
using Autofac.Core;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SH.Api.Infrastructure
{
    public class NLogModule : LogModule<Logger>
    {
        protected override Logger CreateLoggerFor(Type type)
        {
            return LogManager.GetLogger(type.FullName);
        }
    }

    public abstract class LogModule<TLogger> : Module
    {
        protected abstract TLogger CreateLoggerFor(Type type);

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
            return type.GetProperties().Any(property => property.CanWrite && property.PropertyType == typeof(TLogger));
        }

        private bool HasConstructorDependencyOnLogger(Type type)
        {
            return type.GetConstructors()
                       .SelectMany(constructor => constructor.GetParameters()
                                                             .Where(parameter => parameter.ParameterType == typeof(TLogger)))
                       .Any();
        }

        private void InjectLoggerViaProperty(object sender, ActivatedEventArgs<object> @event)
        {
            var type = @event.Instance.GetType();
            var propertyInfo = type.GetProperties().First(x => x.CanWrite && x.PropertyType == typeof(TLogger));
            propertyInfo.SetValue(@event.Instance, CreateLoggerFor(type), null);
        }

        private void InjectLoggerViaConstructor(object sender, PreparingEventArgs @event)
        {
            var type = @event.Component.Activator.LimitType;
            @event.Parameters = @event.Parameters.Union(new[]
            {
                new ResolvedParameter((parameter, context) => parameter.ParameterType == typeof(TLogger), (p, i) => CreateLoggerFor(type))
            });
        }
    }
}