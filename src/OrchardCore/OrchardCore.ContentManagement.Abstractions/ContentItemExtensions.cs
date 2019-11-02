using System;
using System.Threading.Tasks;
using Castle.DynamicProxy;

namespace OrchardCore.ContentManagement
{
    public static class ContentItemExtensions
    {
        internal static ProxyGenerator _proxyGenerator = new ProxyGenerator();

        /// <summary>
        /// Gets a content item as a strongly typed content item proxy.
        /// </summary>
        public static TTypedContentItem To<TTypedContentItem>(this ContentItem contentItem) where TTypedContentItem : TypedContentItem
        {
            var o = (TTypedContentItem)Activator.CreateInstance(typeof(TTypedContentItem), new object[] { contentItem });
            var proxy = (TTypedContentItem)_proxyGenerator.CreateClassProxyWithTarget(o.GetType(),
                o, new object[] { contentItem }, new TypedContentItemInterceptor<TTypedContentItem>());

            return proxy;
        }

        /// <summary>
        /// Gets a content part by its type.
        /// </summary>
        /// <returns>The content part or <code>null</code> if it doesn't exist.</returns>
        public static TPart As<TPart>(this ContentItem contentItem) where TPart : ContentPart
        {
            return contentItem.Get<TPart>(typeof(TPart).Name);
        }

        /// <summary>
        /// Gets a content part by its type or create a new one.
        /// </summary>
        /// <typeparam name="TPart">The type of the content part.</typeparam>
        /// <returns>The content part instance or a new one if it doesn't exist.</returns>
        public static TPart GetOrCreate<TPart>(this ContentItem contentItem) where TPart : ContentPart, new()
        {
            return contentItem.GetOrCreate<TPart>(typeof(TPart).Name);
        }

        /// <summary>
        /// Adds a content part by its type.
        /// </summary>
        /// <typeparam name="part">The part to add to the <see cref="ContentItem"/>.</typeparam>
        /// <returns>The current <see cref="ContentItem"/> instance.</returns>
        public static ContentItem Weld<TPart>(this ContentItem contentItem, TPart part) where TPart : ContentPart
        {
            contentItem.Weld(typeof(TPart).Name, part);
            return contentItem;
        }

        /// <summary>
        /// Updates the content part with the specified type.
        /// </summary>
        /// <typeparam name="TPart">The type of the part to update.</typeparam>
        /// <returns>The current <see cref="ContentItem"/> instance.</returns>
        public static ContentItem Apply<TPart>(this ContentItem contentItem, TPart part) where TPart : ContentPart
        {
            contentItem.Apply(typeof(TPart).Name, part);
            return contentItem;
        }

        /// <summary>
        /// Modifies a new or existing content part by name.
        /// </summary>
        /// <typeparam name="name">The name of the content part to update.</typeparam>
        /// <typeparam name="action">An action to apply on the content part.</typeparam>
        /// <returns>The current <see cref="ContentPart"/> instance.</returns>
        public static ContentItem Alter<TPart>(this ContentItem contentItem, Action<TPart> action) where TPart : ContentPart, new()
        {
            var part = contentItem.GetOrCreate<TPart>();
            action(part);
            contentItem.Apply(part);

            return contentItem;
        }

        /// <summary>
        /// Modifies a new or existing content part by name.
        /// </summary>
        /// <typeparam name="name">The name of the content part to update.</typeparam>
        /// <typeparam name="action">An action to apply on the content part.</typeparam>
        /// <returns>The current <see cref="ContentPart"/> instance.</returns>
        public static async Task<ContentItem> AlterAsync<TPart>(this ContentItem contentItem, Func<TPart, Task> action) where TPart : ContentPart, new()
        {
            var part = contentItem.GetOrCreate<TPart>();
            await action(part);
            contentItem.Apply(part);

            return contentItem;
        }
    }
}
