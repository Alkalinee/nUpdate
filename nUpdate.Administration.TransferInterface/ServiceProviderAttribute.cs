﻿using System;

namespace nUpdate.Administration.TransferInterface
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
    public class ServiceProviderAttribute : Attribute
    {
        /// <summary>
        ///     Gets the type of the services provider.
        /// </summary>
        public Type ServiceType { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceProviderAttribute"/> class.
        /// </summary>
        /// <param name="serviceType">The type of the transfer services provider.</param>
        /// <exception cref="System.ArgumentNullException">srviceType is null.</exception>
        /// <exception cref="System.ArgumentException">Implementation of IServiceProvider is missing.;serviceType</exception>
        public ServiceProviderAttribute(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");
            if (!typeof(IServiceProvider).IsAssignableFrom(serviceType))
                throw new ArgumentException("Implementation of IServiceProvider is missing.", "serviceType");
            ServiceType = serviceType;
        }
    }
}
