﻿namespace Modo.Core.Extensions;

public static class CellExtensions {
  public static Element Element(this Int32 cell) =>
    Grid.Element[cell];
}