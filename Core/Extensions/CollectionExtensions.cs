using System.Collections.Generic;

namespace Modo.Core.Extensions;

public static class CollectionExtensions {
  public static void ForEach<T>(this T[] array, Action<T> action) {
    foreach (var item in array) {
      action(item);
    }
  }
  
  public static Boolean IsIn<T>(this T item, ICollection<T> collection) {
    return collection.Contains(item);
  }
}