﻿using HarmonyLib;
using PulsarPluginLoader.Patches;
using PulsarPluginLoader.Utilities;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace Alarms
{
    [HarmonyPatch(typeof(PLShipInfo), "Update")]
    internal class AlarmsPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            /*List<CodeInstruction> instructionlist = instructions.ToList();
            instructionlist[3234].opcode = OpCodes.Ldc_R4;
            instructionlist[3234].operand = .5f;
            return instructionlist.AsEnumerable(); use for simple transpiling!, use below for better and more future-proof transpiling!*/

            List<CodeInstruction> TargetSequence = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PLShipStats), "get_HullMax")),
                new CodeInstruction(OpCodes.Ldc_R4, .2f),
                new CodeInstruction(OpCodes.Mul),
            };
            List<CodeInstruction> InjectedSequence = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PLShipStats), "get_HullMax")),
                new CodeInstruction(OpCodes.Ldc_R4, .4f),
                new CodeInstruction(OpCodes.Mul),
            };
            IEnumerable<CodeInstruction> shieldlights = HarmonyHelpers.PatchBySequence(instructions, TargetSequence, InjectedSequence, HarmonyHelpers.PatchMode.REPLACE);
            TargetSequence = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PLShipStats), "get_ShieldsMax")),
                new CodeInstruction(OpCodes.Ldc_R4, .3f),
                new CodeInstruction(OpCodes.Mul),
            };
            InjectedSequence = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PLShipStats), "get_ShieldsMax")),
                new CodeInstruction(OpCodes.Ldc_R4, .98f),
                new CodeInstruction(OpCodes.Mul),
            };
            IEnumerable<CodeInstruction> meltdownlights = HarmonyHelpers.PatchBySequence(shieldlights, TargetSequence, InjectedSequence, HarmonyHelpers.PatchMode.REPLACE);
            TargetSequence = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PLServer),"GetCurrentSector")) // use AcessTools.Field(typeof(class), "Method)) to call bools in IL, also this isn't futureproof at all lmao 
            };
            InjectedSequence = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldloc_S, 77), //use the number after the V_ for flags (like flag9)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PLShipInfoBase),"IsReactorTempCritical")),
                new CodeInstruction(OpCodes.Or),
                new CodeInstruction(OpCodes.Stloc_S, 77)
            };
            IEnumerable<CodeInstruction> blackhole = HarmonyHelpers.PatchBySequence(meltdownlights, TargetSequence, InjectedSequence, HarmonyHelpers.PatchMode.AFTER);
            TargetSequence = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PLShipInfoBase), "InWarp")),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Br_S),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Or),
                new CodeInstruction(OpCodes.Stloc_S)
            };
            InjectedSequence = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldloc_S, 77), //use the number after the V_ for flags (like flag9)
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PLServer),"GetCurrentSector")),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PLSectorInfo),"get_VisualIndication")),
                new CodeInstruction(OpCodes.Ldc_I4_S, 0x1F),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Or),
                new CodeInstruction(OpCodes.Stloc_S, 77)
            };
            return HarmonyHelpers.PatchBySequence(blackhole, TargetSequence, InjectedSequence, HarmonyHelpers.PatchMode.AFTER, HarmonyHelpers.CheckMode.NONNULL);
        }
    }
    [HarmonyPatch(typeof(PLPlayer), "Update")]
    internal class AlarmSFXPatch
    {
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> TargetSequence = new List<CodeInstruction>() //changes the alarm siren threshold for shields
            {
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PLShipStats), "get_ShieldsMax")),
                new CodeInstruction(OpCodes.Ldc_R4, .3f),
                new CodeInstruction(OpCodes.Mul),
            };
            List<CodeInstruction> InjectedSequence = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PLShipStats), "get_ShieldsMax")),
                new CodeInstruction(OpCodes.Ldc_R4, .98f),
                new CodeInstruction(OpCodes.Mul),
            };
            return HarmonyHelpers.PatchBySequence(instructions, TargetSequence, InjectedSequence, HarmonyHelpers.PatchMode.REPLACE);
        }
    }

}
/* it's difficult to make meltdown alarm work so it's here for now haha funny
[HarmonyPatch(typeof(PLPlayer), "Update")]
[HarmonyDebug]
class MeltdownSFX
{
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        Label ifMelty = generator.DefineLabel();
        List<CodeInstruction> instructionlist = instructions.ToList();
        instructionlist[1963].opcode = OpCodes.Callvirt;
        instructionlist[1963].operand = AccessTools.Method(typeof(PLShipInfoBase),"IsReactorInMeltdown");
        instructionlist[1964].opcode = OpCodes.Brfalse_S;
        instructionlist[1964].operand = ifMelty;
        instructionlist[1976].operand = ifMelty;
        return instructionlist.AsEnumerable();
    }


}

/*
IEnumerable<CodeInstruction> newSirenThreshold = HarmonyHelpers.PatchBySequence(instructions, TargetSequence, InjectedSequence, HarmonyHelpers.PatchMode.REPLACE);

Label ifMelty = generator.DefineLabel(); // borked code here
TargetSequence = new List<CodeInstruction>()
{
    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PLLevelSync), "PlayerShip")),
    new CodeInstruction(OpCodes.Stloc_3)
};
InjectedSequence = new List<CodeInstruction>()
{
    new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(PLEncounterManager), "Instance")),
    new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PLLevelSync), "PlayerShip")),
    new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PLShipInfoBase), "IsReactorInMeltdown")),
    new CodeInstruction(OpCodes.Brtrue_S, ifMelty),
};
IEnumerable<CodeInstruction> meltdownSfx = HarmonyHelpers.PatchBySequence(newSirenThreshold, TargetSequence, InjectedSequence, HarmonyHelpers.PatchMode.BEFORE);

TargetSequence = new List<CodeInstruction>()
{
    new CodeInstruction(OpCodes.Ldarg_0),
    new CodeInstruction(OpCodes.Ldc_I4_1),
    new CodeInstruction(OpCodes.Stfld, AccessTools.Field(typeof(PLPlayer),"AlarmsArePlayingSFX")),
    new CodeInstruction(OpCodes.Ldstr, "play_ship_generic_internal_alarm"),
    new CodeInstruction(OpCodes.Ldarg_0),
    new CodeInstruction(OpCodes.Call),
    new CodeInstruction(OpCodes.Call),
    new CodeInstruction(OpCodes.Br_S),
    new CodeInstruction(OpCodes.Ldarg_0), 
};
InjectedSequence = new List<CodeInstruction>()
{
    new CodeInstruction(OpCodes.Nop),
};
InjectedSequence[InjectedSequence.Count - 1].labels.Add(ifMelty); */