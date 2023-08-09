using System;

// ReSharper disable once CheckNamespace
namespace DemolishNeutronium {
  /// <summary>
  /// Wrapper for logging; automatically prefixes all messages with mod's name.
  /// </summary>
  public static class LogService {
    public static void Info(String line) {
      Debug.Log($"[{Main.ModName}] {line}");
    }
  }
}