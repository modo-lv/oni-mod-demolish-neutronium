using HarmonyLib;
using static Debug;

// ReSharper disable once CheckNamespace
namespace DemolishNeutronium;

/// <summary>
/// Wrapper for logging; automatically prefixes all messages with mod's name.
/// </summary>
public class LogService(String modName) {
  /// <summary>Controls whether to log debug messages.</summary>
  public Boolean IsDebug =
#if DEBUG
    true;
#else
    false;
#endif

  public void Info(Object line) {
    Log($"[{modName}] {line}");
  }

  public void Warn(Object line) {
    LogWarning($"[{modName}] {line}");
  }

  /// <summary>Log a message only if running a debug build, or if the debug logging is enabled.</summary>
  public void Debug(Object line) {
    if (this.IsDebug)
      this.Info(line);
  }
}