using HarmonyLib;
using PulsarPluginLoader.Patches;
using System.Collections.Generic;
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
                new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Global), "hull")), //default value is .4f
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
                new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Global), "shield" )), //default value is .98f
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

            IEnumerable<CodeInstruction> sectorlights = HarmonyHelpers.PatchBySequence(meltdownlights, TargetSequence, InjectedSequence, HarmonyHelpers.PatchMode.AFTER); //adds sectors that trigger alarm lights
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
                new CodeInstruction(OpCodes.Ldloc_S, 77), //start blackhole
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PLServer),"GetCurrentSector")),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PLSectorInfo),"get_VisualIndication")),
                new CodeInstruction(OpCodes.Ldc_I4_S, 0x1F),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Or),
                new CodeInstruction(OpCodes.Stloc_S, 77), //end blackhole
                new CodeInstruction(OpCodes.Ldloc_S, 77), //start CU asteroid encounter
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PLServer),"GetCurrentSector")),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PLSectorInfo),"get_VisualIndication")),
                new CodeInstruction(OpCodes.Ldc_I4_S, 0x70),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Or),
                new CodeInstruction(OpCodes.Stloc_S, 77), //end CU asteroid encounter
                new CodeInstruction(OpCodes.Ldloc_S, 77), 
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PLServer),"GetCurrentSector")),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PLSectorInfo),"get_VisualIndication")),
                new CodeInstruction(OpCodes.Ldc_I4_S, 0x86),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Or),
                new CodeInstruction(OpCodes.Stloc_S, 77), 
                new CodeInstruction(OpCodes.Ldloc_S, 77), //start fire yes
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PLShipInfo),"CountNonNullFires")),
                new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Global), "firecount")), //default value is 17
                new CodeInstruction(OpCodes.Cgt),
                new CodeInstruction(OpCodes.Or),
                new CodeInstruction(OpCodes.Stloc_S, 77), //end fire yes
                new CodeInstruction(OpCodes.Ldloc_S, 77), //start o2
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PLShipInfoBase), "MyStats")),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PLShipStats),"get_OxygenLevel")),
                new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Global), "o2")), //o2 is .18f by default
                new CodeInstruction(OpCodes.Clt),
                new CodeInstruction(OpCodes.Or),
                new CodeInstruction(OpCodes.Stloc_S, 77), //end o2 
            };
            return HarmonyHelpers.PatchBySequence(sectorlights, TargetSequence, InjectedSequence, HarmonyHelpers.PatchMode.AFTER, HarmonyHelpers.CheckMode.NONNULL);
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
                new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Global),"shield")),
                new CodeInstruction(OpCodes.Mul),
            };
            return HarmonyHelpers.PatchBySequence(instructions, TargetSequence, InjectedSequence, HarmonyHelpers.PatchMode.REPLACE);
        }
    }
}
/* hi if you're reading this, then here's the code that should trigger the AlarmSFX during a meltdown, but it's broken for w/e reason. Lmk if you can get it fixed
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