using System.Collections.Generic;
using DemolishNeutronium;
using HarmonyLib;
using KMod;
using Modo.Core.Extensions;
using Modo.Core.Services;
using Modo.DemolishNeutronium.Models;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using UnityEngine;
using static SimHashes;

namespace Modo.DemolishNeutronium;

// ReSharper disable UnusedType.Global
[HarmonyPatch]
public class Main : UserMod2 {
  public static LogService Log = null!;
  public static SettingsService<DmSettings> SettingsService = null!;

  public static DmSettings Settings => SettingsService.Cached.Value;

  public override void OnLoad(Harmony harmony) {
    Log = new LogService("Demolish Neutronium");
    SettingsService = new SettingsService<DmSettings>(Log);
    Log.IsDebug = Settings.DebugLogging;
    base.OnLoad(harmony);
    PUtil.InitLibrary(logVersion: true);
    new POptions().RegisterOptions(this, typeof(DmSettings));
  }

  /// <summary>
  /// Neutronium Dust element (added by Rocketry Expanded mod).
  /// </summary>
  public static readonly Lazy<Element?> NeutroniumDust = new Lazy<Element?>(() => {
    var result = ElementLoader.elements.FindNeutroniumDust();
    Log.Info(
      result != null
        ? "Neutronium Dust element found, will be dropped by Neutronium digs."
        : "Neutronium Dust not found, Neutronium digs will drop nothing."
    );
    return result;
  });

  /// <summary>
  /// Holds dig times for different Neutronium tiles.
  /// </summary>
  /// <remarks>
  /// In the regular game at the time of writing, this isn't strictly necessary,
  /// since the game only varies digging times for tiles under 400 kg of mass,
  /// and all Neutronium tiles are either 10 or 20 tons.
  /// However, other Neutronium masses are possible through sandbox or mods,
  /// (or in future game versions), so it's best to calculate and track each tile individually.  
  /// </remarks>
  private static readonly Dictionary<Int32, Single> DigTimes = new Dictionary<Int32, Single>();

  /// <summary>
  /// (Re)read the settings every time the game is loaded,
  /// to allow for a direct (config file edit) updates without having to go through the main menu,
  /// as well as the normal GUI approach. 
  /// </summary>
  [HarmonyPatch(typeof(SaveLoader), "OnSpawn"), HarmonyPostfix]
  public static void OnSaveGameLoad() {
    SettingsService.ClearCache();
    DigTimes.Clear();
  }

  /// <summary>
  /// Make Neutronium diggable.   
  /// </summary>
  [HarmonyPatch(typeof(Diggable), nameof(Diggable.Undiggable)), HarmonyPostfix]
  public static void Undiggable(Element e, ref Boolean __result) {
    if (e.IsNeutronium())
      __result = false;
  }


  /// <summary>
  /// Make dig time calculations work.
  /// </summary>
  /// <remarks>
  /// <see cref="Diggable.GetApproximateDigTime"/> is hardcoded to return near infinity for material hardness 255,
  /// so we set have to set it to something lower to trigger an actual calculation. 
  /// </remarks>
  [HarmonyPatch(typeof(Diggable), nameof(Diggable.GetApproximateDigTime))]
  class GetApproximateDigTime {

    static void Prefix(Int32 cell) {
      if (!cell.Element().IsNeutronium()) return;
      if (DigTimes.Get(cell, Single.MaxValue) < Single.MaxValue) return;

      Log.Debug($"Dig time for cell {cell} (mass: {Grid.Mass[cell]}) not calculated yet, " +
                $"setting hardness to that of Obsidian.");
      cell.Element().hardness = Obsidian.Element().hardness;
    }


    static void Postfix(Int32 cell, ref Single __result) {
      if (!cell.Element().IsNeutronium()) return;

      if (DigTimes.Get(cell, Single.MaxValue) < Single.MaxValue) {
        __result = DigTimes[cell];
        Log.Debug($"Dig time for cell {cell} already known: {__result}");
      } else {
        DigTimes[cell] = __result *= Settings.DigTimeMultiplier;
        Log.Debug(
          $"Dig time for cell {cell} calculated to {__result}, restoring Neutronium hardness.");
        cell.Element().hardness = Unobtanium.Element().hardness;
      }
    }
  }

  /// <summary>Enables direct building on Neutronium.</summary>
  [HarmonyPatch(
    declaringType: typeof(BuildingDef),
    methodName: "IsAreaClear",
    argumentTypes: [
      typeof(GameObject), typeof(Int32), typeof(Orientation), typeof(ObjectLayer),
      typeof(ObjectLayer), typeof(Boolean), typeof(Boolean), typeof(String), typeof(Boolean)
    ],
    argumentVariations: [
      ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal,
      ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Normal
    ]
  )]
  public static class IsAreaClear {
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
      Log.Debug("Patching [BuildingDef.IsAreaClear] to treat neutronium like any other material...");
      return instructions.NeutroniumToObsidian();
    }
  }

  /// <summary>Prevents "Invalid build location" error when placing buildings on neutronium.</summary>
  [HarmonyPatch(declaringType: typeof(BuildingDef), methodName: "IsAreaValid")]
  public static class IsAreaValid {
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
      Log.Debug("Patching [BuildingDef.IsAreaValid] to treat neutronium like any other material...");
      return instructions.NeutroniumToObsidian();
    }
  }


  /// <summary>
  /// Ensure Neutronium can be dug with Super-Duperhard Digging skill & has correct work time set.
  /// </summary>
  /// <remarks>
  /// Digging materials with hardness over 250 requires "Hazmat Digging" skill only available in the DLC,
  /// so for compatibility with vanilla we'll use Obsidian's hardness value to lower the requirement to
  /// "Super-Duperhard Digging".
  /// </remarks>
  [HarmonyPatch(typeof(Diggable), "OnSpawn")]
  class OnSpawn {

    static void Prefix(ref Diggable __instance) {
      if (!__instance.IsNeutronium()) return;

      // Use Obsidian's hardness for dig skill determination.
      __instance.Element().hardness = Obsidian.Element().hardness;
    }


    static void Postfix(ref Diggable __instance) {
      if (!__instance.IsNeutronium()) return;

      // Neutronium's work time is set to infinity, so we have to override it manually. 
      __instance.SetWorkTime(Diggable.GetApproximateDigTime(__instance.Cell()));
      __instance.WorkTimeRemaining = __instance.workTime;
      // Skill requirement has been determined, restore hardness to original
      __instance.Element().hardness = Unobtanium.Element().hardness;
    }

  }

  /// <summary>
  /// Use the purple demolishing laser instead of the regular red since we're not getting any resources from the dig.
  /// </summary>
  [HarmonyPatch(typeof(Diggable), "UpdateColor"), HarmonyPostfix]
  static void UpdateColor(ref Diggable __instance, ref HashedString ___multitoolContext) {
    if (!__instance.IsNeutronium()) return;

    // If we're getting dust then it's mining, not demolishing
    if (Settings.DustEnabled && NeutroniumDust.Value != null) return;

    ___multitoolContext = "demolish";
  }

  /// <summary>
  /// Prevent Neutronium resource from dropping when the tile has been dug out.
  /// </summary>
  [HarmonyPatch(typeof(WorldDamage), nameof(WorldDamage.OnDigComplete)), HarmonyPrefix]
  static void OnDigComplete(ref Single mass, ref UInt16 element_idx) {
    if (!ElementLoader.elements[element_idx].IsNeutronium()) return;

    if (Settings.DustEnabled && NeutroniumDust.Value is { } dust) {
      element_idx = dust.idx;
      mass = Settings.DustMultiplier * (mass / 1000) * Settings.DustAmount;
    } else {
      mass = 0;
    }
  }
}