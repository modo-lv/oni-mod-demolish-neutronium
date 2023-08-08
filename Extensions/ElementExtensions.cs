using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace DemolishNeutronium.Extensions {
  public static class ElementExtensions {
    public static Boolean IsNeutronium(this Element e) =>
      e.id == SimHashes.Unobtanium;

    /// <summary>
    /// Find Neutronium Dust (added by Rocketry Expanded mod) in a list of elements.
    /// </summary>
    /// <returns>Neutronium Dust element, or <c>null</c> if it is not in the list.</returns>
    [CanBeNull]
    public static Element FindNeutroniumDust([NotNull] this IEnumerable<Element> elements) {
      return elements.First(e => e.id.ToString() == "UnobtaniumDust");
    }
  }
}