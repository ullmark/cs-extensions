using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.Core;
using EPiServer;
using EPiServer.Filters;
using EPiServer.Configuration;

namespace Development
{
    public static class EPiServerExtensions
    {

        /// <summary>
        /// Gets the property from the page and parses it to the 
        /// provided <typeparamref name="T"/> and returns it.
        /// Returns <typeparamref name="T"/>s default value if property 
        /// not set or not existing.
        /// </summary>
        /// <example>
        /// <code>
        /// string body = CurrentPage.GetPropertyValue<string>("MainBody");
        /// </code>
        /// </example>
        /// <exception cref="T:System.InvalidCastException">
        /// When trying to parse a property to an invalid type.
        /// </exception>
        /// <typeparam name="T">Target Type</typeparam>
        /// <param name="page">The <see cref="T:EPiServer.Core.PageData"/> that contains the properties</param>
        /// <param name="propertyname">name of the property</param>
        /// <returns></returns>
        public static T GetPropertyValue<T>(this PageData page, string propertyname)
        {
            return page.GetPropertyValue<T>(propertyname, default(T));
        }

        /// <summary>
        /// Gets the property from the page and parses it to the 
        /// provided <typeparamref name="T"/> and returns it.
        /// Returns <paramref name="fallback"/> if property isn't set
        /// or not existing.
        /// </summary>
        /// <example>
        /// <code>
        /// int count = CurrentPage.GetPropertyValue<int>("MaxCount", 50);
        /// PageReference pf = CurrentPage.GetPropertyValue<PageReference>("MenuRoot", PageReference.StartPage);
        /// </code>
        /// </example>
        /// <exception cref="T:System.InvalidCastException">
        /// When trying to parse a property to an invalid type.
        /// </exception>
        /// <typeparam name="T"></typeparam>
        /// <param name="page"></param>
        /// <param name="propertyname"></param>
        /// <param name="fallback"></param>
        /// <returns></returns>
        public static T GetPropertyValue<T>(this PageData page, string propertyname, T fallback)
        {
            if (page.IsPropertySet(propertyname))
            {
                return (T)page.Property[propertyname].Value;
            }
            return fallback;
        }

        /// <summary>
        /// Gets the <see cref="T:EPiServer.Core.PageData"/> that the property in 
        /// <paramref name="propertyname"/> points at.
        /// </summary>
        /// <example>
        /// <code>
        /// PageData newsPage = CurrentPage.GetPropertyPage("NewsPage");
        /// </code>
        /// </example>
        /// <exception cref="T:System.InvalidCastException">
        /// Cast if the property specified in <paramref name="propertyname"/> is of other type than 
        /// <see cref="T:EPiServer.Core.PropertyPageReference"/>
        /// </exception>
        /// <exception cref="T:EPiServer.Core.AccessDeniedException">
        /// Cast if the user has insufficiant rights for the page
        /// </exception>
        /// <param name="page"></param>
        /// <param name="propertyname"></param>
        /// <returns></returns>
        public static PageData GetPropertyPage(this PageData page, string propertyname)
        {
            PageReference reference = page.GetPropertyValue<PageReference>(propertyname);
            if (!PageReference.IsNullOrEmpty(reference))
            {
                return DataFactory.Instance.GetPage(reference);
            }
            return null;
        }

        /// <summary>
        /// Gets the <see cref="T:EPiServer.Core.PageData"/> that the property in 
        /// <paramref name="propertyname"/> points at.
        /// </summary>
        /// <remarks>
        /// Returns null if anything went wrong
        /// </remarks>
        /// <param name="page"></param>
        /// <param name="propertyname"></param>
        /// <returns></returns>
        public static PageData GetPropertyPageSafe(this PageData page, string propertyname)
        {
            try
            {
                return page.GetPropertyPage(propertyname);
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Gets a <see cref="T:EPiServer.Core.PageDataCollection"/> containing 
        /// the children of the page
        /// </summary>
        /// <example>
        /// <code>
        /// PageDataCollection children = CurrentPage.GetChildren();
        /// </code>
        /// </example>
        /// <param name="page"></param>
        /// <returns></returns>
        public static PageDataCollection GetChildren(this PageData page)
        {
            return page.PageLink.GetChildren();
        }

        /// <summary>
        /// Gets a <see cref="T:EPiServer.Core.PageDataCollection"/> containing
        /// the children of the <paramref name="page"/>. And applies the <paramref name="filters"/>
        /// </summary>
        /// <param name="page">The page which children should be returned</param>
        /// <param name="filters">The filters that should be applied</param>
        /// <returns></returns>
        public static PageDataCollection GetChildren(this PageData page, params IPageFilter[] filters)
        {
            return page.PageLink.GetChildren(filters);
        }

        /// <summary>
        /// Gets a <see cref="T:EPiServer.Core.PageDataCollection"/> containing
        /// the children of the <paramref name="page"/>.
        /// </summary>
        /// <param name="page">The page which children should be returned</param>
        /// <returns></returns>
        public static PageDataCollection GetChildren(this PageReference page)
        {
            return DataFactory.Instance.GetChildren(page);
        }

        /// <summary>
        /// Gets a <see cref="T:EPiServer.Core.PageDataCollection"/> containing
        /// the children of the <paramref name="page"/>. And applies the <paramref name="filters"/>
        /// </summary>
        /// <param name="page">The page which children should be returned</param>
        /// <param name="filters">The filters that should be applied</param>
        /// <returns></returns>
        public static PageDataCollection GetChildren(this PageReference page, params IPageFilter[] filters)
        {
            PageDataCollection pages = page.GetChildren();
            foreach (IPageFilter filter in filters)
            {
                filter.Filter(pages);
            }
            return pages;
        }

        /// <summary>
        /// Gets a <see cref="T:EPiServer.Core.PageDataCollection"/> containing 
        /// the siblings of <paramref name="page"/>
        /// </summary>
        /// <param name="page">The page which siblings should be returned</param>
        /// <returns>A collection of siblings</returns>
        public static PageDataCollection GetSiblings(this PageData page)
        {
            return page.GetSiblings(true);
        }

        /// <summary>
        /// Gets a <see cref="T:EPiServer.Core.PageDataCollection"/> containing 
        /// the siblings of <paramref name="page"/>
        /// </summary>
        /// <param name="page">The page which siblings should be returned</param>
        /// <param name="excludeSelf">Whether <paramref name="page"/> should be removed from the collection</param>
        /// <returns>A collection of siblings</returns>
        public static PageDataCollection GetSiblings(this PageData page, bool excludeSelf)
        {
            return page.GetSiblings(excludeSelf, null);
        }

        /// <summary>
        /// Gets a <see cref="T:EPiServer.Core.PageDataCollection"/> containing the siblings
        /// of <paramref name="page"/> after applying the <paramref name="filters"/>
        /// </summary>
        /// <param name="page">The page which siblings should be returned</param>
        /// <param name="filters">The filters that should be applied</param>
        /// <returns>A collection of siblings</returns>
        public static PageDataCollection GetSiblings(this PageData page, params IPageFilter[] filters)
        {
            return page.GetSiblings(true, filters);
        }

        /// <summary>
        /// Gets a <see cref="T:EPiServer.Core.PageDataCollection"/> containing 
        /// the siblings of <paramref name="page"/> after applying the <paramref name="filters"/>
        /// </summary>
        /// <param name="page">The page which siblings should be returned</param>
        /// <param name="excludeSelf">Whether <paramref name="page"/> should be excluded.</param>
        /// <param name="filters">The filters that should be applied</param>
        /// <returns></returns>
        public static PageDataCollection GetSiblings(this PageData page, bool excludeSelf, params IPageFilter[] filters)
        {
            PageDataCollection siblings = page.ParentLink.GetChildren();
            if (excludeSelf)
            {
                siblings.Remove(page);
            }
            if (filters != null)
            {
                foreach (IPageFilter filter in filters)
                {
                    filter.Filter(siblings);
                }
            }
            return siblings;
        }

        /// <summary>
        /// Returns a <see cref="T:System.Boolean"/> whether the property with the name 
        /// <paramref name="propertyname"/> is set on <paramref name="page"/>
        /// </summary>
        /// <param name="page"></param>
        /// <param name="propertyname"></param>
        /// <returns></returns>
        public static bool IsPropertySet(this PageData page, string propertyname)
        {
            return page.Property[propertyname] != null && page.Property[propertyname].Value != null;
        }

        /// <summary>
        /// Returns a <see cref="T:System.Boolean"/> whether the 
        /// <paramref name="page"/> has any children
        /// </summary>
        /// <param name="page">the page to check for children</param>
        /// <returns></returns>
        public static bool HasChildren(this PageReference page)
        {
            return (page.GetChildren().Count > 0);
        }

        /// <summary>
        /// Returns a <see cref="T:System.Boolean"/> whether the 
        /// <paramref name="page"/> has any children
        /// </summary>
        /// <param name="page">the page to check for children</param>
        /// <returns></returns>
        public static bool HasChildren(this PageData page)
        {
            return (page.GetChildren().Count > 0);
        }

        /// <summary>
        /// Returns a <see cref="T:System.Boolean"/> whether the 
        /// <paramref name="page"/> has any children of which status
        /// </summary>
        /// <param name="page"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static bool HasChildren(this PageData page, PagePublishedStatus status)
        {
            return (page.GetChildren(new FilterPublished(status)).Count > 0);
        }

        /// <summary>
        /// Returns the root relative <see cref="T:System.String"/> url of the 
        /// page that the property named <paramref name="propertyname"/> has.
        /// </summary>
        /// <exception cref="System.InvalidCastException">
        /// Cast if you send in the name of a property not of type <see cref="T:EPiServer.Core.PropertyPageReference"/>
        /// </exception>
        /// <remarks>
        /// Returns empty string if page doesn't exist or accessdenied
        /// </remarks>
        /// <param name="page"></param>
        /// <param name="propertyname"></param>
        /// <returns></returns>
        public static string GetPropertyLinkUrl(this PageData page, string propertyname)
        {
            PageReference pf = page.GetPropertyValue<PageReference>(propertyname);
            if (!PageReference.IsNullOrEmpty(pf))
            {
                PageData pd = pf.GetPageSafe();
                if (pd != null)
                {
                    return pd.LinkURL;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Checks if this PageReference is a child of the <paramref name="parent"/>
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static bool IsChildOf(this PageReference child, PageReference parent)
        {
            PageData page = DataFactory.Instance.GetPage(child);
            return page.IsChildOf(parent);
        }

        /// <summary>
        /// Checks if this page is a child of the provided parent reference
        /// </summary>
        /// <param name="child"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static bool IsChildOf(this PageData child, PageReference parent)
        {
            if (child == null)
            {
                throw new ArgumentNullException("child", "Cannot be null");
            }
            while (child.ParentLink != PageReference.RootPage)
            {
                if (child.ParentLink == parent)
                {
                    return true;
                }
                child = DataFactory.Instance.GetPage(child.ParentLink);
            }
            return false;
        }

        /// <summary>
        /// Checks whether this pagereference is parent
        /// of the <paramref name="child"/>
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public static bool IsParentOf(this PageReference parent, PageReference child)
        {
            return child.IsChildOf(parent);
        }

        /// <summary>
        /// Checks whether this pagedata is parent of 
        /// <paramref name="child"/>
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        public static bool IsParentOf(this PageData parent, PageReference child)
        {
            return child.IsChildOf(parent.PageLink);
        }

        /// <summary>
        /// Gets a pagedatacollection for this collections referenses
        /// </summary>
        /// <param name="pageReferences"></param>
        /// <returns></returns>
        public static PageDataCollection GetPages(this PageReferenceCollection pageReferences)
        {
            PageDataCollection pages = new PageDataCollection();
            foreach (PageReference r in pageReferences)
            {
                pages.Add(r.GetPage());
            }
            return pages;
        }

        /// <summary>
        /// Gets the data for this reference
        /// </summary>
        /// <example>
        /// PageData page = PageReference.StartPage.GetPage();
        /// </example>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static PageData GetPage(this PageReference reference, ILanguageSelector language)
        {
            if (language != null)
                return DataFactory.Instance.GetPage(reference, language);
            return DataFactory.Instance.GetPage(reference);
        }

        /// <summary>
        /// Gets the page for the provided reference
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static PageData GetPage(this PageReference reference)
        {
            return reference.GetPage(null);
        }

        /// <summary>
        /// Gets the Url to this page
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static string LinkUrl(this PageReference reference)
        {
            return reference.LinkUrl(null);
        }

        /// <summary>
        /// Gets the url to this page
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public static string LinkUrl(this PageReference reference, ILanguageSelector language)
        {
            PageData page = reference.GetPageSafe(language);
            if (page != null)
                return page.LinkURL;
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static PageData GetPageSafe(this PageReference reference)
        {
            return reference.GetPageSafe(null);
        }

        /// <summary>
        /// Gets the page safely
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public static PageData GetPageSafe(this PageReference reference, ILanguageSelector language)
        {
            try
            {
                return reference.GetPage(language);
            }
            catch { }
            return null;
        }

        /// <summary>
        /// Gets the PageData of this page's parent
        /// </summary>
        /// <example>
        /// PageData page = PageReference.StartPage.GetParent();
        /// </example>
        /// <param name="page"></param>
        /// <returns></returns>
        public static PageData GetParent(this PageData page)
        {
            return page.ParentLink.GetPage();
        }

        /// <summary>
        /// Builds an external friendly url to this page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static string ExternalUrl(this PageReference page)
        {
            return page.GetPage().ExternalUrl();
        }

        /// <summary>
        /// Builds an external friendly url to this page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static string ExternalUrl(this PageData page)
        {
            StringBuilder url = new StringBuilder();
            url.Append(Settings.Instance.SiteUrl.Scheme);
            url.Append("://");
            url.Append(Settings.Instance.SiteUrl.Host);
            if (Settings.Instance.SiteUrl.Port != 80)
                url.AppendFormat(":{0}", Settings.Instance.SiteUrl.Port);
            url.Append(page.LinkURL);

            UrlBuilder builder = new UrlBuilder(url.ToString());
            EPiServer.Global.UrlRewriteProvider.ConvertToExternal(builder, page.PageLink, Encoding.UTF8);
            return builder.ToString();
        }
    }
}
