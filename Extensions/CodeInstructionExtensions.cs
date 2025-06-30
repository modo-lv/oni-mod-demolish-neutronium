using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using static SimHashes;

namespace DemolishNeutronium.Extensions;

public static class CodeInstructionExtensions {
  /// <summary>Replaces neutronium's ID with obsidian's in a list of IL instructions.</summary>
  /// <remarks>
  /// The game directly checks for neutronium in various methods.
  /// This method can be used in transpiler patches to replace neutronium's ID lookups with obsidian's,
  /// bypassing the game's "if neutronium then can't do anything" limits.  
  /// </remarks>
  public static IEnumerable<CodeInstruction> NeutroniumToObsidian(
    this IEnumerable<CodeInstruction> instructions
  ) {
    return instructions.Select(instruction => {
      // Whenever the method tries to do anything with Neutronium ID, use Obsidian's ID instead
      if (instruction.opcode == OpCodes.Ldc_I4 && instruction.operand is (Int32) Unobtanium) {
        instruction.operand = (Int32) Obsidian;
      }
      return instruction;
    });
  }
}