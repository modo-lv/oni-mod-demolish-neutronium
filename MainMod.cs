using System;
using DemolishNeutronium.Extensions;
using HarmonyLib;
using JetBrains.Annotations;
using KMod;
using Newtonsoft.Json;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;

// ReSharper disable ArrangeTypeModifiers
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedType.Local

namespace DemolishNeutronium {
  // ReSharper disable once UnusedType.Global
  [HarmonyPatch]
  class MainMod : UserMod2 {
    /// <summary>
    /// Neutronium Dust element (added by Rocketry Expanded mod).
    /// </summary>
    [ItemCanBeNull] public static readonly Lazy<Element> NeutroniumDust = new Lazy<Element>(() => {
      var result = ElementLoader.elements.FindNeutroniumDust();
      Main.Log.Info(
        NeutroniumDust != null
          ? "Neutronium Dust element found, will be dropped by Neutronium digs."
          : "Neutronium Dust not found, digs will drop nothing."
      );
      return result;
    });

    /// <summary>
    /// Mod settings. 
    /// </summary>
    public static Lazy<Main.Settings> Config = new Lazy<Main.Settings>(Main.Settings.Load);

    
    /// <summary>
    /// Initialize PLib stuff and configure the settings window.
    /// </summary>
    public override void OnLoad(Harmony harmony) {
      base.OnLoad(harmony);
      PUtil.InitLibrary();
      new POptions().RegisterOptions(this, typeof(Main.Settings));
    }

    /// <summary>
    /// (Re)read the settings every time the game is loaded,
    /// to allow for a direct (config file edit) updates without having to go through the main menu,
    /// as well as the normal GUI approach. 
    /// </summary>
    [HarmonyPatch(typeof(SaveLoader), "OnSpawn")]
    [HarmonyPostfix]
    public static void OnSaveGameLoad() {
      Config = new Lazy<Main.Settings>(Main.Settings.Load);
      Main.Log.Info($"Settings loaded: {JsonConvert.SerializeObject(Config.Value)}");
    }

    /// <summary>
    /// Make Neutronium diggable.   
    /// </summary>
    [HarmonyPatch(typeof(Diggable), nameof(Diggable.Undiggable))]
    [HarmonyPostfix]
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

      private static Single _neutroniumDigTime = Single.MaxValue;


      static void Prefix(Int32 cell) {
        if (!cell.Element().IsNeutronium()) return;
        if (_neutroniumDigTime < Single.MaxValue) return;

        cell.Element().hardness = 254;
      }


      static void Postfix(Int32 cell, ref Single __result) {
        if (!cell.Element().IsNeutronium()) return;

        if (_neutroniumDigTime < Single.MaxValue) {
          __result = _neutroniumDigTime;
        }
        else {
          _neutroniumDigTime = __result *= 5;
          cell.Element().hardness = 255;
        }
      }
    }


    /// <summary>
    /// Ensure Neutronium can be dug with Superduperhard Digging skill & has correct work time set.
    /// </summary>
    /// <remarks>
    /// Digging materials with hardness over 250 requires "Hazmat Digging" skill only available in the DLC,
    /// so for compatibility with vanilla we'll use Obsidian's hardness value to lower the requirement to
    /// "Superduperhard Digging".
    /// </remarks>
    [HarmonyPatch(typeof(Diggable), "OnSpawn")]
    class OnSpawn {

      static void Prefix(ref Diggable __instance) {
        if (!__instance.IsNeutronium()) return;

        // Use Obsidian's hardness for dig skill determination.
        __instance.Element().hardness = ElementLoader.FindElementByHash(SimHashes.Obsidian).hardness;
      }


      static void Postfix(ref Diggable __instance) {
        if (!__instance.IsNeutronium()) return;

        // Netronium's work time is set to infinity, so we have to override it manually. 
        __instance.SetWorkTime(Diggable.GetApproximateDigTime(__instance.Cell()));
        __instance.WorkTimeRemaining = __instance.workTime;
        // Skill requirement has been determined, restore hardness to original
        __instance.Element().hardness = 255;
      }

    }

    /// <summary>
    /// Use the purple demolishing laser instead of the regular red since we're not getting any resources from the dig.
    /// </summary>
    [HarmonyPatch(typeof(Diggable), "UpdateColor")]
    [HarmonyPostfix]
    static void UpdateColor(ref Diggable __instance, ref HashedString ___multitoolContext) {
      if (!__instance.IsNeutronium()) return;

      ___multitoolContext = "demolish";
    }

    /// <summary>
    /// Prevent Neutronium resource from dropping when the tile has been dug out.
    /// </summary>
    [HarmonyPatch(typeof(WorldDamage), nameof(WorldDamage.OnDigComplete))]
    [HarmonyPrefix]
    static void OnDigComplete(ref Single mass, ref UInt16 element_idx) {
      if (!ElementLoader.elements[element_idx].IsNeutronium()) return;

      if (Config.Value.DustEnabled && NeutroniumDust.Value is Element dust) {
        element_idx = dust.idx;
        mass = Config.Value.DustMultiplier * (mass / 1000) * Config.Value.DustAmount;
      }
      else {
        mass = 0;
      }
    }
  }
}