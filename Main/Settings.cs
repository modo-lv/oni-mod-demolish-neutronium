using System;
using PeterHan.PLib.Options;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

// ReSharper disable once CheckNamespace
namespace DemolishNeutronium {
  internal static partial class Main {
    [ConfigFile(SharedConfigLocation: true, IndentOutput: true)]
    public class Settings {
      
      /// <summary>
      /// Load mod settings. If the config file does not exist, it will be created.
      /// </summary>
      public static Settings Load() {
        Log.Info("Loading settings...");
        if (POptions.ReadSettings<Settings>() is Settings cfg)
          return cfg;

        var result = new Settings();
        POptions.WriteSettings(result);
        return result;
      }
      
      [Option(
        title: "Enable dust (if available)",
        tooltip: "If another mod (e.g. Rocketry Expanded) has added Neutronium Dust resource, " +
                 "digging Neutronium will excavate it.",
        category: "Neutronium Dust"
      )]
      public Boolean DustEnabled { get; set; } = true;

      [Option(
        title: "Dust amount (kg) per 1t of Neutronium:",
        tooltip: "Amount of dust excavated from Neutronium tiles.\n" +
                 "Default is 0.01kg (100g for every 10t of Neutronium).\n",
        category: "Neutronium Dust",
        Format = "F3"
      )]
      [Limit(min: 0.001, max: 1000)]
      public Single DustAmount { get; set; } = 0.01f;

      [Option(
        title: "Dust amount multiplier",
        tooltip: "Doubles (by default) the drop amount to compensate for the game halving it\n" +
                 "(so that the dust amount setting is applied exactly).\n" +
                 "Reduce this if you're using other mods that increase drop rates.",
        category: "Neutronium Dust"
      )]
      [Limit(min: 1, max: 2)]
      public Single DustMultiplier { get; set; } = 2;
    }
  }
}