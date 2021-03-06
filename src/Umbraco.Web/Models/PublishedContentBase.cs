﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Umbraco.Core;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.PropertyEditors.ValueConverters;

namespace Umbraco.Web.Models
{
    /// <summary>
    /// Provide an abstract base class for <c>IPublishedContent</c> implementations.
    /// </summary>
    /// <remarks>This base class does which (a) consistently resolves and caches the Url, (b) provides an implementation
    /// for this[alias], and (c) provides basic content set management.</remarks>
    [DebuggerDisplay("Content Id: {Id}, Name: {Name}")]
    public abstract class PublishedContentBase : IPublishedContent
    {
        protected PublishedContentBase(IUmbracoContextAccessor umbracoContextAccessor)
        {
            UmbracoContextAccessor = umbracoContextAccessor ?? throw new ArgumentNullException(nameof(umbracoContextAccessor));
        }

        protected IUmbracoContextAccessor UmbracoContextAccessor { get; }

        #region ContentType

        public abstract PublishedContentType ContentType { get; }

        #endregion

        #region PublishedElement

        /// <inheritdoc />
        public abstract Guid Key { get; }

        #endregion

        #region PublishedContent

        /// <inheritdoc />
        public abstract int Id { get; }

        /// <inheritdoc />
        public abstract string Name { get; }

        /// <inheritdoc />
        public abstract string UrlSegment { get; }

        /// <inheritdoc />
        public abstract int SortOrder { get; }

        /// <inheritdoc />
        public abstract int Level { get; }

        /// <inheritdoc />
        public abstract string Path { get; }

        /// <inheritdoc />
        public abstract int? TemplateId { get; }

        /// <inheritdoc />
        public abstract int CreatorId { get; }

        /// <inheritdoc />
        public abstract string CreatorName { get; }

        /// <inheritdoc />
        public abstract DateTime CreateDate { get; }

        /// <inheritdoc />
        public abstract int WriterId { get; }

        /// <inheritdoc />
        public abstract string WriterName { get; }

        /// <inheritdoc />
        public abstract DateTime UpdateDate { get; }

        /// <inheritdoc />
        public virtual string Url => GetUrl();

        /// <inheritdoc />
        /// <remarks>
        /// The url of documents are computed by the document url providers. The url of medias are computed by the media url providers
        /// </remarks>
        public virtual string GetUrl(string culture = null) // TODO: consider .GetCulture("fr-FR").Url
        {
            var umbracoContext = UmbracoContextAccessor.UmbracoContext;

            if (umbracoContext == null)
                throw new InvalidOperationException("Cannot compute Url for a content item when UmbracoContext is null.");
            if (umbracoContext.UrlProvider == null)
                throw new InvalidOperationException("Cannot compute Url for a content item when UmbracoContext.UrlProvider is null.");

            switch (ItemType)
            {
                case PublishedItemType.Content:
                    return umbracoContext.UrlProvider.GetUrl(this, culture);
                case PublishedItemType.Media:
                    return umbracoContext.UrlProvider.GetMediaUrl(this, Constants.Conventions.Media.File, culture);
                default:
                    throw new NotSupportedException();
            }
        }

        /// <inheritdoc />
        public abstract PublishedCultureInfo GetCulture(string culture = null);

        /// <inheritdoc />
        public abstract IReadOnlyDictionary<string, PublishedCultureInfo> Cultures { get; }

        /// <inheritdoc />
        public abstract PublishedItemType ItemType { get; }

        /// <inheritdoc />
        public abstract bool IsDraft(string culture = null);

        /// <inheritdoc />
        public abstract bool IsPublished(string culture = null);

        #endregion

        #region Tree

        /// <inheritdoc />
        public abstract IPublishedContent Parent { get; }

        /// <inheritdoc />
        public abstract IEnumerable<IPublishedContent> Children { get; }

        #endregion

        #region Properties

        /// <inheritdoc cref="IPublishedElement.Properties"/>
        public abstract IEnumerable<IPublishedProperty> Properties { get; }

        /// <inheritdoc cref="IPublishedElement.GetProperty(string)"/>
        public abstract IPublishedProperty GetProperty(string alias);

        #endregion
    }
}
