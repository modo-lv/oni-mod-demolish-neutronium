using PeterHan.PLib.Options;

namespace Modo.DemolishNeutronium.Models;

/// <summary>Data model holding all mod's configuration settings.</summary>
[ConfigFile(SharedConfigLocation: true, IndentOutput: true)]
public class DmSettings {
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

  [Option(
    title: "Log debugging info",
    tooltip: "Enable this to log extra information that may be useful in solving problems with the mod.",
    category: "Meta"
  )]
  public Boolean DebugLogging { get; set; } = false;
}