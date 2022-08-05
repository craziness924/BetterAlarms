using HarmonyLib;
using PulsarModLoader.Patches;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using System.Reflection;
using PulsarModLoader.Utilities;

namespace Alarms
{
    class AlarmsPatches
    {
        [HarmonyPatch(typeof(PLShipInfo), "Update")]
        [HarmonyDebug]
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

                InjectedSequence = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldloc_S, 80), //use the number after the V_ for flags (like flag9)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PLShipInfoBase),"IsReactorTempCritical")),
                new CodeInstruction(OpCodes.Or),
                new CodeInstruction(OpCodes.Stloc_S, 80)
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
                new CodeInstruction(OpCodes.Ldloc_S, 80), //use the number after the V_ for flags (like flag9)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PLShipInfoBase),"IsReactorTempCritical")),
                new CodeInstruction(OpCodes.Or),
                new CodeInstruction(OpCodes.Stloc_S, 80)
            };
                InjectedSequence = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldloc_S, 80), //start blackhole
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PLServer),"GetCurrentSector")),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PLSectorInfo),"get_VisualIndication")),
                new CodeInstruction(OpCodes.Ldc_I4_S, 0x1F),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Or),
                new CodeInstruction(OpCodes.Stloc_S, 80), //end blackhole
                new CodeInstruction(OpCodes.Ldloc_S, 80), //start CU asteroid encounter
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PLServer),"GetCurrentSector")),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PLSectorInfo),"get_VisualIndication")),
                new CodeInstruction(OpCodes.Ldc_I4_S, 0x70),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Or),
                new CodeInstruction(OpCodes.Stloc_S, 80), //end CU asteroid encounter
                new CodeInstruction(OpCodes.Ldloc_S, 80), // start warp guardian
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PLServer),"GetCurrentSector")),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PLSectorInfo),"get_VisualIndication")),
                new CodeInstruction(OpCodes.Ldc_I4, 0x86),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Or),
                new CodeInstruction(OpCodes.Stloc_S, 80), // end warp guardian
           /*   new CodeInstruction(OpCodes.Ldloc_S, 80), // start unseen threat
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PLServer),"GetCurrentSector")),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PLSectorInfo),"get_VisualIndication")),
                new CodeInstruction(OpCodes.Ldc_I4, 0x8C),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Or),
                new CodeInstruction(OpCodes.Stloc_S, 80), // end unseen threat */
                new CodeInstruction(OpCodes.Ldloc_S, 80), //start fire yes
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PLShipInfo),"CountNonNullFires")),
                new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Global), "firecount")), //default value is 17
                new CodeInstruction(OpCodes.Cgt),
                new CodeInstruction(OpCodes.Or),
                new CodeInstruction(OpCodes.Stloc_S, 80), //end fire yes
                new CodeInstruction(OpCodes.Ldloc_S, 80), //start o2
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PLShipInfoBase), "MyStats")),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PLShipStats),"get_OxygenLevel")),
                new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Global), "o2")), //o2 is .18f by default
                new CodeInstruction(OpCodes.Clt),
                new CodeInstruction(OpCodes.Or),
                new CodeInstruction(OpCodes.Stloc_S, 80), //end o2 
            };
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
                IEnumerable<CodeInstruction> alarmspeed = HarmonyHelpers.PatchBySequence(alertflashes, TargetSequence, InjectedSequence, HarmonyHelpers.PatchMode.REPLACE, HarmonyHelpers.CheckMode.NONNULL);
                TargetSequence = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldc_R4, 0f),
                new CodeInstruction(OpCodes.Ldc_R4, 300f),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Time), "get_deltaTime")),
                new CodeInstruction(OpCodes.Mul),
                new CodeInstruction(OpCodes.Newobj, AccessTools.Constructor(typeof(Vector3), new Type[] { typeof(float), typeof(float) } )),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Transform), "Rotate", new Type[] { typeof(Vector3) } ))
            };
                InjectedSequence = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Global), "verspeed")),
                new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(Global), "horspeed")),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Time), "get_deltaTime")),
                new CodeInstruction(OpCodes.Mul),
                new CodeInstruction(OpCodes.Newobj, AccessTools.Constructor(typeof(Vector3), new Type[] { typeof(float), typeof(float) } )),
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(Transform), "Rotate", new Type[] { typeof(Vector3) } ))
            };

                return HarmonyHelpers.PatchBySequence(alarmspeed, TargetSequence, InjectedSequence, HarmonyHelpers.PatchMode.REPLACE, HarmonyHelpers.CheckMode.NONNULL);
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

        //[HarmonyPatch(typeof(PLPlayer), "Update")]
        //class MeltdownSfx
        //{
        //    static bool debugmode = true;
        //    static void Prefix(PLPlayer __instance, ref bool ___AlarmsArePlayingSFX, ref Dictionary<string, bool> __state)
        //    {
        //        __state = new Dictionary<string, bool>();
        //        PLShipInfo pLShipInfo;

        //        __state.Add("enabledAlarms", false);
        //        __state.Add("disabledAlarms", false);
        //        if (__instance.GetPawn() != null)
        //        {
        //            pLShipInfo = __instance.GetPawn().CurrentShip;
        //        }
        //        else
        //        {
        //            pLShipInfo = PLEncounterManager.Instance.PlayerShip;
        //        }
        //        if (pLShipInfo == null)
        //        {
        //            return;
        //        }
        //        // pLShipInfo.IsReactorInMeltdown() && pLShipInfo.IsReactorTempCritical() && pLShipInfo.MyReactor != null || 
        //        if (pLShipInfo.IsReactorInMeltdown() && pLShipInfo.IsReactorTempCritical() && pLShipInfo.MyReactor != null || debugmode)
        //        {
        //            if (!___AlarmsArePlayingSFX)
        //            {
        //                __state["enabledAlarms"] = true;
        //                __state["disabledAlarms"] = false;
        //            }
        //        }
        //        else if (___AlarmsArePlayingSFX)
        //        {
        //            __state["enabledAlarms"] = false;
        //            __state["disabledAlarms"] = true;
        //        }
        //        __state.Add("alarmsArePlaying", ___AlarmsArePlayingSFX);
        //        return;
        //    }
        //    static void Postfix(PLPlayer __instance, ref bool ___AlarmsArePlayingSFX, Dictionary<string, bool> __state)
        //    {
        //        bool shouldCustomAlarmsPlay = __state["enabledAlarms"] && !__state["disabledAlarms"];
        //        bool gameChangedAlarms = shouldCustomAlarmsPlay != ___AlarmsArePlayingSFX;
        //        if (___AlarmsArePlayingSFX || (!___AlarmsArePlayingSFX && gameChangedAlarms))
        //        {
        //            return;
        //        }
        //        else if (!shouldCustomAlarmsPlay)
        //        {
        //            PLMusic.PostEvent("stop_ship_generic_internal_alarm", __instance.gameObject);
        //        }
        //        else if (shouldCustomAlarmsPlay)
        //        {
        //            ___AlarmsArePlayingSFX = true;
        //            PLMusic.PostEvent("play_ship_generic_internal_alarm", __instance.gameObject);
        //        }

        //    }
        //}
    }
}
