namespace DemolishNeutronium.Extensions {
  public static class SimHashesExtensions {
    /// <summary>
    /// Find an Element by it's <see cref="SimHashes"/> value.
    /// </summary>
    public static Element Element(this SimHashes simHash) =>
      ElementLoader.FindElementByHash(simHash);
  }
}