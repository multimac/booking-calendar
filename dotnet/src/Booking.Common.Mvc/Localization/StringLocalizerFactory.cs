using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Resources;
using Booking.Common.Mvc.Logging;
using Booking.Common.Mvc.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Booking.Common.Mvc.Localization
{
    /// <summary>
    /// An <see cref="IStringLocalizerFactory"/> that creates instances of <see cref="ResourceManagerStringLocalizer"/>.
    /// Based off <see cref="ResourceManagerStringLocalizerFactory">, but provides support for loading
    /// resources in external assemblies.  
    /// </summary>
    public class StringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IResourceNamesCache _resourceNamesCache = new ResourceNamesCache();
        private readonly ConcurrentDictionary<string, ResourceManagerStringLocalizer> _localizerCache =
            new ConcurrentDictionary<string, ResourceManagerStringLocalizer>();

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ILogger<StringLocalizerFactory> _logger;
        private readonly StringLocalizerFactoryOptions _options;

        /// <summary>
        /// Creates a new <see cref="ResourceManagerStringLocalizer"/>.
        /// </summary>
        /// <param name="hostingEnvironment">The <see cref="IHostingEnvironment"/>.</param>
        /// <param name="localizationOptions">The <see cref="IOptions{LocalizationOptions}"/>.</param>
        public StringLocalizerFactory(
            IHostingEnvironment hostingEnvironment,
            ILogger<StringLocalizerFactory> logger,
            IOptions<StringLocalizerFactoryOptions> localizationOptions)
        {
            if (hostingEnvironment == null)
            {
                throw new ArgumentNullException(nameof(hostingEnvironment));
            }
            
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (localizationOptions == null)
            {
                throw new ArgumentNullException(nameof(localizationOptions));
            }

            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            _options = localizationOptions.Value;
        }

        /// <summary>
        /// Gets the resource prefix used to look up the resource.
        /// </summary>
        /// <param name="typeInfo">The type of the resource to be looked up.</param>
        /// <returns>The prefix for resource lookup.</returns>
        protected virtual string GetResourcePrefix(TypeInfo typeInfo)
        {
            if (typeInfo == null)
            {
                throw new ArgumentNullException(nameof(typeInfo));
            }

            var assemblyName = typeInfo.Assembly.GetName().Name;

            return GetResourcePrefix(typeInfo, assemblyName, GetRelativePath(assemblyName));
        }

        /// <summary>
        /// Gets the resource prefix used to look up the resource.
        /// </summary>
        /// <param name="typeInfo">The type of the resource to be looked up.</param>
        /// <param name="baseNamespace">The base namespace of the application.</param>
        /// <param name="resourcesRelativePath">The folder containing all resources.</param>
        /// <returns>The prefix for resource lookup.</returns>
        /// <remarks>
        /// For the type "Sample.Controllers.Home" if there's a resourceRelativePath return
        /// "Sample.Resourcepath.Controllers.Home" if there isn't one then it would return "Sample.Controllers.Home".
        /// </remarks>
        protected virtual string GetResourcePrefix(TypeInfo typeInfo, string baseNamespace, string resourcesRelativePath)
        {
            if (typeInfo == null)
            {
                throw new ArgumentNullException(nameof(typeInfo));
            }

            if (string.IsNullOrEmpty(baseNamespace))
            {
                throw new ArgumentNullException(nameof(baseNamespace));
            }

            return string.IsNullOrEmpty(resourcesRelativePath)
                ? typeInfo.FullName
                : baseNamespace + "." + resourcesRelativePath + TrimPrefix(typeInfo.FullName, baseNamespace + ".");
        }

        /// <summary>
        /// Gets the resource prefix used to look up the resource.
        /// </summary>
        /// <param name="baseResourceName">The name of the resource to be looked up</param>
        /// <param name="baseNamespace">The base namespace of the application.</param>
        /// <returns>The prefix for resource lookup.</returns>
        protected virtual string GetResourcePrefix(string baseResourceName, string baseNamespace)
        {
            if (string.IsNullOrEmpty(baseResourceName))
            {
                throw new ArgumentNullException(nameof(baseResourceName));
            }

            var locationPath = baseNamespace + GetRelativePath(baseNamespace);
            baseResourceName = locationPath + TrimPrefix(baseResourceName, baseNamespace + ".");

            return baseResourceName;
        }

        /// <summary>
        /// Creates a <see cref="ResourceManagerStringLocalizer"/> using the <see cref="Assembly"/> and
        /// <see cref="Type.FullName"/> of the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="resourceSource">The <see cref="Type"/>.</param>
        /// <returns>The <see cref="ResourceManagerStringLocalizer"/>.</returns>
        public IStringLocalizer Create(Type resourceSource)
        {
            if (resourceSource == null)
            {
                throw new ArgumentNullException(nameof(resourceSource));
            }

            var typeInfo = resourceSource.GetTypeInfo();
            var assembly = typeInfo.Assembly;

            // Re-root the base name if a resources path is set
            var baseName = GetResourcePrefix(typeInfo);

            return _localizerCache.GetOrAdd(baseName, _ =>
                new ResourceManagerStringLocalizer(
                    new ResourceManager(baseName, assembly),
                    assembly,
                    baseName,
                    _resourceNamesCache)
            );
        }

        /// <summary>
        /// Creates a <see cref="ResourceManagerStringLocalizer"/>.
        /// </summary>
        /// <param name="baseName">The base name of the resource to load strings from.</param>
        /// <param name="location">The location to load resources from.</param>
        /// <returns>The <see cref="ResourceManagerStringLocalizer"/>.</returns>
        public IStringLocalizer Create(string baseName, string location)
        {
            if (baseName == null)
            {
                throw new ArgumentNullException(nameof(baseName));
            }

            location = location ?? _hostingEnvironment.ApplicationName;

            baseName = GetResourcePrefix(baseName, location);

            return _localizerCache.GetOrAdd($"B={baseName},L={location}", _ =>
            {
                var assembly = Assembly.Load(new AssemblyName(location));
                return new ResourceManagerStringLocalizer(
                    new ResourceManager(baseName, assembly),
                    assembly,
                    baseName,
                    _resourceNamesCache);
            });
        }

        private string GetRelativePath(string assembly)
        {
            string path;
            if (_options.ResourcePaths == null || !_options.ResourcePaths.TryGetValue(assembly, out path))
            {
                _logger.KeyNotFound(assembly, _options.DefaultPath);
                path = _options.DefaultPath ?? string.Empty;
            }

            if (!string.IsNullOrEmpty(path))
            {
                path = path
                    .Replace(Path.AltDirectorySeparatorChar, '.')
                    .Replace(Path.DirectorySeparatorChar, '.') + ".";
            }

            return path;
        }
        private static string TrimPrefix(string name, string prefix)
        {
            if (name.StartsWith(prefix, StringComparison.Ordinal))
            {
                return name.Substring(prefix.Length);
            }

            return name;
        }
    }
}