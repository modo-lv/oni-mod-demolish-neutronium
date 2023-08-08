using System;

// ReSharper disable once CheckNamespace
namespace DemolishNeutronium {
  internal static partial class Main {
    /// <summary>
    /// Wrapper for logging; automatically prefixes all messages with mod's name.
    /// </summary>
    public static class Log {
      public static void Info(String line) {
        Debug.Log($"[{ModName}] {line}");
      }
    }
  }
}