using System;
using PeterHan.PLib.Options;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

// ReSharper disable once CheckNamespace
namespace DemolishNeutronium {
  internal static partial class Main {
    [ConfigFile(SharedConfigLocation: true, IndentOutput: true)]
    public class Settings {

      [Option(
        title: "Excavation difficulty (relative to Obsidian):",
        tooltip: "Neutronium dig time is calculated by multiplying Obsidian dig time by this value.\n" +
                 "\n" +
                 "Default is 5.00 (500%), meaning it takes 5 times longer to dig up Neutronium than Obsidian."
      )]
      [Limit(min: 1.00, max: 10.00)]
      public Single DigTimeMultiplier { get; set; } = 5.00f;
      
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
                 "\n" +
                 "Default is 0.01kg, meaning 100g of dust for every 10t of Neutronium.\n",
        category: "Neutronium Dust",
        Format = "F3"
      )]
      [Limit(min: 0.001, max: 1000)]
      public Single DustAmount { get; set; } = 0.01f;

      [Option(
        title: "Dust amount multiplier",
        tooltip: "Multiplies drop amount to compensate for the game halving it\n" +
                 "(so that the dust amount setting is applied exactly).\n" +
                 "Reduce this accordingly if you're using other mods that increase drop rates.\n" +
                 "\n" +
                 "Default is 2.00.",
        category: "Neutronium Dust"
      )]
      [Limit(min: 1, max: 2)]
      public Single DustMultiplier { get; set; } = 2;
      
      
      /// <summary>
      /// Load mod settings. If the config file does not exist, it will be created with default values.
      /// </summary>
      public static Settings Load() {
        Log.Info("Loading settings...");
        if (POptions.ReadSettings<Settings>() is Settings cfg)
          return cfg;

        var result = new Settings();
        POptions.WriteSettings(result);
        return result;
      }
    }
  }
}