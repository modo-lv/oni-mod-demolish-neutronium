namespace Modo.Core.Extensions;

public static class LogExtensions {
  public static String goInstance(this KMonoBehaviour component) =>
    $"{component.gameObject} [{component.gameObject.GetInstanceID()}]";
}