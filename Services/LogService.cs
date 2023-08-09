using System;
using static Debug;

// ReSharper disable once CheckNamespace
namespace DemolishNeutronium {
  /// <summary>
  /// Wrapper for logging; automatically prefixes all messages with mod's name.
  /// </summary>
  public static class LogService {
    public static void Info(String line) {
      Log($"[{Main.ModName}] {line}");
    }

    /// <summary>
    /// Log a message only if running a debug build.
    /// </summary>
    public static void Debug(String line) {
#if DEBUG
      Info(line);
#endif
    }
  }
}