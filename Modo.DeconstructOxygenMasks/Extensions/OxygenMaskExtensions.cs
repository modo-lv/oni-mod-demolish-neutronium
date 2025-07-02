using UnityEngine;

namespace Modo.DeconstructOxygenMasks.Extensions;

public static class OxygenMaskExtensions {
  public static ItemDeconstructable Deconstruction(this GameObject mask) => 
    mask.RequireComponent<ItemDeconstructable>();
}