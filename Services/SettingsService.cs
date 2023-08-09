using DemolishNeutronium.Models;
using Newtonsoft.Json;
using PeterHan.PLib.Options;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

// ReSharper disable once CheckNamespace
namespace DemolishNeutronium {
    /// <summary>
    /// Functionality for working with mod's configuration.
    /// </summary>
    public static class SettingsService {
      /// <summary>
      /// Load mod settings. If the config file does not exist, it will be created with default values.
      /// </summary>
      public static Settings Load() {
        LogService.Info("Loading settings...");
        var result = POptions.ReadSettings<Settings>();

        if (result == null) {
          result = new Settings();
          POptions.WriteSettings(result);
        }
        LogService.Info($"Settings loaded: {JsonConvert.SerializeObject(result)}");
        return result;
      }
    }
}