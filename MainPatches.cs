using System;
using DemolishNeutronium.Extensions;
using HarmonyLib;

// ReSharper disable ArrangeTypeModifiers
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedType.Local

namespace DemolishNeutronium {

  [HarmonyPatch(typeof(Diggable))]
  class MainPatches {

    /// <summary>
    /// Make Neutronium diggable.   
    /// </summary>
    [HarmonyPatch(nameof(Diggable.Undiggable))]
    [HarmonyPostfix]
    public static void Undiggable(Element e, ref Boolean __result) {
      if (e.id == SimHashes.Unobtanium)
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
    [HarmonyPatch("UpdateColor")]
    [HarmonyPostfix]
    public static void UpdateColor(ref Diggable __instance, ref HashedString ___multitoolContext) {
      if (!__instance.IsNeutronium()) return;

      ___multitoolContext = "demolish";
    }

    /// <summary>
    /// Prevent Neutronium resource from dropping when the tile has been dug out.
    /// </summary>
    [HarmonyPatch(typeof(WorldDamage), nameof(WorldDamage.OnDigComplete))]
    [HarmonyPrefix]
    static void OnDigComplete(ref Single mass, Byte element_idx) {
      if (!ElementLoader.elements[element_idx].IsNeutronium()) return;
      mass = 0;
    }
  }
}