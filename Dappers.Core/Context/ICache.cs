using System;
using System.Collections;
using System.Text;

namespace Dappers.Context
{
    public interface ICache
    {
        /// <summary>
        /// Gets the number of items in the SpringCache.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets a collection of all cache item keys.
        /// </summary>
        ICollection Keys { get; }

        bool ContainsKey(object key);

        /// <summary>
        /// Retrieves an item from the SpringCache.
        /// </summary>
        /// <param name="key">
        /// Item key.
        /// </param>
        /// <returns>
        /// Item for the specified <paramref name="key"/>, or <c>null</c>.
        /// </returns>
        object Get(object key);

        /// <summary>
        /// Removes an item from the SpringCache.
        /// </summary>
        /// <param name="key">
        /// Item key.
        /// </param>
        void Remove(object key);

        /// <summary>
        /// Removes collection of items from the SpringCache.
        /// </summary>
        /// <param name="keys">
        /// Collection of keys to remove.
        /// </param>
        void RemoveAll(ICollection keys);

        /// <summary>
        /// Removes all items from the SpringCache.
        /// </summary>
        void Clear();

        /// <summary>
        /// Inserts an item into the SpringCache.
        /// </summary>
        /// <remarks>
        /// Items inserted using this method have no expiration time
        /// and default cache priority.
        /// </remarks>
        /// <param name="key">
        /// Item key.
        /// </param>
        /// <param name="value">
        /// Item value.
        /// </param>
        void Insert(object key, object value);

        /// <summary>
        /// Inserts an item into the SpringCache.
        /// </summary>
        /// <remarks>
        /// Items inserted using this method have default cache priority.
        /// </remarks>
        /// <param name="key">
        /// Item key.
        /// </param>
        /// <param name="value">
        /// Item value.
        /// </param>
        /// <param name="timeToLive">
        /// Item's time-to-live.
        /// </param>
        void Insert(object key, object value, TimeSpan timeToLive);
    }
}
