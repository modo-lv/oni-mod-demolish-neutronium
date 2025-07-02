using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DemolishNeutronium;
using HarmonyLib;
using KMod;
using Modo.Core.Extensions;
using Modo.Core.Services;
using PeterHan.PLib.Options;
using UnityEngine;
using static Modo.DeconstructOxygenMasks.DomSettings;

namespace Modo.DeconstructOxygenMasks;

// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable ArrangeTypeModifiers
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedType.Local
/// <summary>Main patches of the mod.</summary>
[HarmonyPatch]
class Main : UserMod2 {
  public static LogService Log = null!;
  public static SettingsService<DomSettings> SettingsService = null!;
  public static DomSettings Settings => SettingsService.Cached.Value;

  public override void OnLoad(Harmony harmony) {
    Log = new LogService("Deconstruct Oxygen Masks");
    SettingsService = new SettingsService<DomSettings>(Log);
    Log.IsDebug = Settings.DebugLogging;
    base.OnLoad(harmony);
    new POptions().RegisterOptions(this, typeof(DomSettings));
  }
  
  /// <summary>Modifies OM mass according to settings.</summary>
  [HarmonyPatch(typeof(OxygenMaskConfig), nameof(OxygenMaskConfig.CreateEquipmentDef))]
  [HarmonyPostfix]
  public static void Mass(ref EquipmentDef __result) {
    if (Settings.MassSetBy == MassController.Mod) {
      __result.Mass = Settings.OxygenMaskMass;
    }
  }

  /// <summary>Attaches <see cref="ItemDeconstructable"/> component to (W)OMs.</summary>
  [HarmonyPatch(typeof(EquipmentConfigManager), nameof(EquipmentConfigManager.RegisterEquipment))]
  [HarmonyPostfix]
  public static void EnableDecon(IEquipmentConfig config) {
    if (config is not OxygenMaskConfig) return;

    new[] { OxygenMaskConfig.ID, OxygenMaskConfig.WORN_ID }.ForEach(id => {
      id.Prefab()
        .AddOrGet<ItemDeconstructable>()
        .Also(decon =>
          Log.Debug($"Attached {nameof(ItemDeconstructable)} component to {decon.goInstance()} prefab...")
        );
    });
  }

  /// <summary>Modifies the primary element of freshly constructed OMs to the ingredient used.</summary>
  /// <remarks>
  /// Normally all OMs in the game are made of dirt, regardless of what was used to construct them.
  /// This runs just after the construction, while recipe data is still available, and modifies it.
  /// </remarks>
  [HarmonyPatch(typeof(ComplexFabricator), "SpawnOrderProduct"), HarmonyPostfix]
  public static void SetOmElement(ComplexRecipe recipe, List<GameObject> __result) {
    var masks = __result.FindAll(it => it.name == OxygenMaskConfig.ID);
    if (masks.Count < 1) return; // Some other, non-oxygen maks construction

    if (recipe.ingredients.Any(it => it.material == OxygenMaskConfig.WORN_ID)) {
      Log.Debug("Worn oxygen masks don't need any special processing.");
      return;
    }

    var ingredientElement = recipe.ingredients
      .Select(ingredient =>
        Assets.GetPrefab(ingredient.material).gameObject.PrimaryElement().Also(m => {
          m.Mass = ingredient.amount;
        })
      )
      .Where(el => el.Element.IsSolid)
      .OrderByDescending(it => it.Mass)
      .FirstOrDefault();

    if (ingredientElement is null) {
      Log.Debug(
        $"Oxygen mask(s) were created with a recipe without any solid ingredients ({recipe}); " +
        $"primary material left unchanged."
      );
      return;
    }

    Log.Debug(
      $"Crafted {masks.Count} oxygen mask(s), setting primary material to the most massive ingredient: " +
      $"{ingredientElement.ElementID}."
    );

    foreach (var mask in masks) {
      mask.PrimaryElement().Also(it => 
        it.SetElement(ingredientElement.ElementID)
      );
    }
  }
  
  /// <summary>
  /// Clear <see cref="PrimaryElement"/>'s internal <c>_Element</c> cache after updating element ID.
  /// </summary>
  [HarmonyPatch(typeof(PrimaryElement), nameof(PrimaryElement.SetElement))]
  [HarmonyPostfix]
  public static void PrimaryElementChanged(PrimaryElement __instance) {
    /*
    SetElement changes the `ElementID`, but if the corresponding element object has already been resolved, 
    it is cached in `_Element` and not updated on ID changes.
    And since `_Element` is a private field, we can't modify it in any way other than through reflection.
    */
    __instance.GetType().GetField("_Element", BindingFlags.NonPublic | BindingFlags.Instance).Also(cache =>
      cache?.SetValue(__instance, null)
    );
  }


#if DEBUG
  /// <summary>Makes OMs wear out with a single use.</summary>
  [HarmonyPatch(typeof(Durability), "OnSpawn")]
  [HarmonyPostfix]
  public static void NoDurability(Durability __instance) {
    __instance.GetType()
      .GetField("difficultySettingMod", BindingFlags.NonPublic | BindingFlags.Instance)
      .Also(cache =>
        cache?.SetValue(__instance, 1000f)
      );
  }
#endif
}