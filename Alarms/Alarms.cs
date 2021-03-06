﻿using HarmonyLib;
using PulsarPluginLoader.Patches;
using System;
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
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Method(typeof(PLShipInfoBase),"InWarp")), // use AcessTools.Field(typeof(class), "Method)) to call bools in IL, also this isn't futureproof at all lmao 
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Br_S),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Or),
                new CodeInstruction(OpCodes.Stloc_S)
            };
            PulsarPluginLoader.Utilities.Logger.Info("temp critical patch done");
            InjectedSequence = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldloc_S, 79), //use the number after the V_ for flags (like flag9)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PLShipInfoBase),"IsReactorTempCritical")),
                new CodeInstruction(OpCodes.Or),
                new CodeInstruction(OpCodes.Stloc_S, 79)
            };

            IEnumerable<CodeInstruction> sectorlights = HarmonyHelpers.PatchBySequence(meltdownlights, TargetSequence, InjectedSequence, HarmonyHelpers.PatchMode.AFTER, HarmonyHelpers.CheckMode.NONNULL); //adds sectors that trigger alarm lights
            TargetSequence = new List<CodeInstruction>()
            {
               /* new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PLShipInfoBase), "InWarp")),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Br_S),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Or),
                new CodeInstruction(OpCodes.Stloc_S) */
                new CodeInstruction(OpCodes.Ldloc_S, 79), //use the number after the V_ for flags (like flag9)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PLShipInfoBase),"IsReactorTempCritical")),
                new CodeInstruction(OpCodes.Or),
                new CodeInstruction(OpCodes.Stloc_S, 79)
            };
            InjectedSequence = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldloc_S, 79), //start blackhole
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PLServer),"GetCurrentSector")),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PLSectorInfo),"get_VisualIndication")),
                new CodeInstruction(OpCodes.Ldc_I4_S, 0x1F),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Or),
                new CodeInstruction(OpCodes.Stloc_S, 79), //end blackhole
                new CodeInstruction(OpCodes.Ldloc_S, 79), //start CU asteroid encounter
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PLServer),"GetCurrentSector")),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PLSectorInfo),"get_VisualIndication")),
                new CodeInstruction(OpCodes.Ldc_I4_S, 0x70),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Or),
                new CodeInstruction(OpCodes.Stloc_S, 79), //end CU asteroid encounter
                new CodeInstruction(OpCodes.Ldloc_S, 79), // start warp guardian
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PLServer),"GetCurrentSector")),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PLSectorInfo),"get_VisualIndication")),
                new CodeInstruction(OpCodes.Ldc_I4, 0x86),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Or),
                new CodeInstruction(OpCodes.Stloc_S, 79), // end warp guardian
           /*     new CodeInstruction(OpCodes.Ldloc_S, 79), // start unseen threat
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PLServer),"GetCurrentSector")),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PLSectorInfo),"get_VisualIndication")),
                new CodeInstruction(OpCodes.Ldc_I4, 0x8C),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Or),
                new CodeInstruction(OpCodes.Stloc_S, 79), // end unseen threat */
                new CodeInstruction(OpCodes.Ldloc_S, 79), //start fire yes
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PLShipInfo),"CountNonNullFires")),
                new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Global), "firecount")), //default value is 17
                new CodeInstruction(OpCodes.Cgt),
                new CodeInstruction(OpCodes.Or),
                new CodeInstruction(OpCodes.Stloc_S, 79), //end fire yes
                new CodeInstruction(OpCodes.Ldloc_S, 79), //start o2
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PLShipInfoBase), "MyStats")),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PLShipStats),"get_OxygenLevel")),
                new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Global), "o2")), //o2 is .18f by default
                new CodeInstruction(OpCodes.Clt),
                new CodeInstruction(OpCodes.Or),
                new CodeInstruction(OpCodes.Stloc_S, 79), //end o2 
            };
            PulsarPluginLoader.Utilities.Logger.Info("sector patch done");
            IEnumerable<CodeInstruction> alertflashes = HarmonyHelpers.PatchBySequence(sectorlights, TargetSequence, InjectedSequence, HarmonyHelpers.PatchMode.AFTER, HarmonyHelpers.CheckMode.NONNULL);
            TargetSequence = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PLShipInfo), "timesPlayedFlash")),
                new CodeInstruction(OpCodes.Ldc_I4_S),
            };
            InjectedSequence = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PLShipInfo), "timesPlayedFlash")),
                new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Global), "flashcount")),
            };
            
            return HarmonyHelpers.PatchBySequence(alertflashes, TargetSequence, InjectedSequence, HarmonyHelpers.PatchMode.REPLACE, HarmonyHelpers.CheckMode.NONNULL);
        }

        [HarmonyPatch(typeof(PLPlayer), "Update")]

        internal class AlarmSFXPatch
        {
            private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
              //  Label ifmelty = generator.DefineLabel();
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

                return HarmonyHelpers.PatchBySequence(instructions, TargetSequence, InjectedSequence, HarmonyHelpers.PatchMode.REPLACE, HarmonyHelpers.CheckMode.NONNULL);
            }
        }
    }
}




// hi if you're reading this, then here's the code that should trigger the AlarmSFX during a meltdown, but it's broken for w/e reason. Lmk if you can get it fixed
/*
                IEnumerable<CodeInstruction> meltdownalarm = HarmonyHelpers.PatchBySequence(instructions, TargetSequence, InjectedSequence, HarmonyHelpers.PatchMode.REPLACE, HarmonyHelpers.CheckMode.NONNULL);
                {
                    TargetSequence = new List<CodeInstruction>()
           {
               new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(PLNetworkManager), "Instance")),
               new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PLNetworkManager), "LocalPlayer")),
               new CodeInstruction(OpCodes.Ldarg_0),
               new CodeInstruction(OpCodes.Call)

           };
                    InjectedSequence = new List<CodeInstruction>()
           {
               new CodeInstruction(OpCodes.Ldloc_S, 4), //start blackhole
               new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PLShipInfoBase),"IsReactorInMeltdown")),
               new CodeInstruction(OpCodes.Brtrue_S, ifmelty)
           };
                    IEnumerable<CodeInstruction> meltdownlabeldestination = HarmonyHelpers.PatchBySequence(meltdownalarm, TargetSequence, InjectedSequence, HarmonyHelpers.PatchMode.BEFORE, HarmonyHelpers.CheckMode.NONNULL); //adds sectors that trigger alarm lights
                    TargetSequence = new List<CodeInstruction>()
           {
               new CodeInstruction(OpCodes.Ldarg_0),
               new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PLPlayer), "AlarmsArePlayingSFX"))
           };
                    InjectedSequence = new List<CodeInstruction>()
                    {
                        new CodeInstruction(OpCodes.Nop, ifmelty)
           };
                    return HarmonyHelpers.PatchBySequence(meltdownlabeldestination, TargetSequence, InjectedSequence, HarmonyHelpers.PatchMode.BEFORE, HarmonyHelpers.CheckMode.NONNULL);
                }
            } */


/* 1st attempt:
               IEnumerable<CodeInstruction> meltdownalarm = HarmonyHelpers.PatchBySequence(instructions, TargetSequence, InjectedSequence, HarmonyHelpers.PatchMode.REPLACE);
               Label ifMelty = generator.DefineLabel();
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

               IEnumerable<CodeInstruction> meltdownalarmlabeltarget = HarmonyHelpers.PatchBySequence(meltdownalarm, TargetSequence, InjectedSequence, HarmonyHelpers.PatchMode.AFTER, HarmonyHelpers.CheckMode.NONNULL);
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
   new CodeInstruction(OpCodes.Nop, ifMelty),
}; */