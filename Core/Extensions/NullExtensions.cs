namespace Modo.Core.Extensions;

public static class NullExtensions {
  public static T OrIfNull<T>(this T? maybe, Func<T> valueProvider) {
    return maybe ?? valueProvider();
  }
}