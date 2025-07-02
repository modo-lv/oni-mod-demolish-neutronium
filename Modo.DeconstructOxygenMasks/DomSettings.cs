using System.Diagnostics.CodeAnalysis;
using PeterHan.PLib.Options;

namespace Modo.DeconstructOxygenMasks;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

/// <summary>Data model holding all mod's configuration settings.</summary>
[ConfigFile(SharedConfigLocation: true, IndentOutput: true)]
public class DomSettings {
  [Option(
    title: "Oxygen mask mass set by:",
    tooltip: "What determines the mass of oxygen masks\n" +
             "(how much material you get back when deconstructing them).\n" +
             "\n" +
             "Game: Use the default, built-in mass (originally 15 kg).\n" +
             "Mod: Use the custom mass configured in the mod settings."
  )]
  public MassController MassSetBy { get; set; } = MassController.Mod;
  
  [Option(
    title: "Custom oxygen mass:",
    tooltip: "If set by the mod, what mass should oxygen masks have."
  )]
  [Limit(min: 0.1, max: 20000)]
  public Single OxygenMaskMass { get; set; } = 50;
  
  public enum MassController { Game, Mod }
  
  [Option(
    title: "Log debugging info",
    tooltip: "Enable this to log extra information that may be useful in solving problems with the mod.",
    category: "Meta"
  )]
  public Boolean DebugLogging { get; set; } = false;
}