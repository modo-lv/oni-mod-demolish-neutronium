namespace Modo.Core.Extensions;

public static class DiggableExtensions {
  public static Int32 Cell(this Diggable dig) =>
    Grid.PosToCell(dig);

  public static Element Element(this Diggable dig) =>
    Grid.Element[dig.Cell()];

  public static Boolean IsNeutronium(this Diggable dig) =>
    dig.Element().IsNeutronium();
}