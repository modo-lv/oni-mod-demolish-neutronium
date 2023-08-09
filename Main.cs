using System;
using DemolishNeutronium.Models;

namespace DemolishNeutronium {
  internal static class Main {
    /// <summary>
    /// Name of the mod for use in logging et al.
    /// </summary>
    public const String ModName = "Demolish Neutronium";
    
    /// <summary>
    /// Mod settings. 
    /// </summary>
    public static Lazy<Settings> Config = new Lazy<Settings>(SettingsService.Load);
  }
}