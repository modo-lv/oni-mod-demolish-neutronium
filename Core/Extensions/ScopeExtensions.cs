namespace Modo.Core.Extensions;

public static class ScopeExtensions {
  public static T Also<T>(this T self, Action<T> action) {
    action(self);
    return self;
  }
}