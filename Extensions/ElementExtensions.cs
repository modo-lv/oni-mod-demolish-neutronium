using System;

namespace DemolishNeutronium.Extensions {
  public static class ElementExtensions {
    public static Boolean IsNeutronium(this Element e) =>
      e.id == SimHashes.Unobtanium;
  }
}