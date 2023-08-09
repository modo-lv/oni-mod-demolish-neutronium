using System.Collections.Generic;

namespace DemolishNeutronium.Extensions {
  public static class DictionaryExtensions {
    /// <summary>
    /// Syntactic sugar wrapping around <see cref="T:System.Collections.Generic.IDictionary`2" /> accessor
    /// allowing to provide a fallback value in case the supplied key is not found in the
    /// dictionary.
    /// </summary>
    /// <typeparam name="TKey">Dictionary key type.</typeparam>
    /// <typeparam name="TValue">Dictionary value type.</typeparam>
    /// <param name="dictionary">Dictionary to get value from.</param>
    /// <param name="key">Key for finding the value.</param>
    /// <param name="fallback">Value to return if the key is not found.</param>
    /// <returns>Found value or <paramref name="fallback" />.</returns>
    public static TValue Get<TKey, TValue>(
      this IDictionary<TKey, TValue> dictionary,
      TKey key,
      TValue fallback = default) {
      return dictionary.TryGetValue(key, out var value) ? value : fallback;
    }
  }
}