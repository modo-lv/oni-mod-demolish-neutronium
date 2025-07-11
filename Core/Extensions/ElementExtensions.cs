﻿using System.Collections.Generic;
using System.Linq;

namespace Modo.Core.Extensions {
  public static class ElementExtensions {
    public static Boolean IsNeutronium(this Element e) =>
      e.id == SimHashes.Unobtanium;

    /// <summary>
    /// Find Neutronium Dust (added by Rocketry Expanded mod) in a list of elements.
    /// </summary>
    /// <returns>Neutronium Dust element, or <c>null</c> if it is not in the list.</returns>
    public static Element? FindNeutroniumDust(this IEnumerable<Element> elements) {
      return elements.FirstOrDefault(e => e.id.ToString() == "UnobtaniumDust");
    }
  }
}