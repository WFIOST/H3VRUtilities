using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using FistVR;
using UnityEngine;
using BepInEx.Harmony;
using BepInEx;
using BepInEx.Configuration;

namespace Meatyceiver2
{
	[BepInPlugin("dll.potatoes.meatyceiver2", "Meatyceiver2", "0.1")]
    public class FirearmFailures : BaseUnityPlugin
    {
		private static ConfigEntry<bool> enableFirearmFailures;
		private static ConfigEntry<bool> enableConsoleDebugging;

		private static ConfigEntry<float> generalMult;
		private static ConfigEntry<float> pistolMult;
		private static ConfigEntry<float> closedBoltMult;
		private static ConfigEntry<float> openBoltMult;

		private static ConfigEntry<float> failureToFeedRate;

		private static ConfigEntry<float> FailureToExtractRate;
		private static ConfigEntry<float> DoubleFeedRate;
		private static ConfigEntry<float> StovepipeRate;

		public static float prevSlideZLock = -999f;

		public static System.Random rnd;

		void Awake()
		{
			UnityEngine.Debug.Log("Meatyceiver2 here!");
			enableFirearmFailures = Config.Bind("_General Settings", "Enable Firearm Failures", true, "Enables firearm related failures.");
			enableConsoleDebugging = Config.Bind("_General Settings", "Enable Console Debugging", false, "Exports values and failures to console.");

			generalMult = Config.Bind("_Multipliers", "Failure Chance Multiplier", 1f, "default at 1x is 1%, so this is a more 'pick failure percentage chance'.");
/*			pistolMult = Config.Bind("_Multipliers", "General Failure Chance Multiplier", 1f, "Pistols are higher than others because they are semi.");
			closedBoltMult = Config.Bind("_Multipliers", "General Failure Chance Multiplier", 0.25f, "Any closed bolted gun.");
			openBoltMult = Config.Bind("_Multipliers", "General Failure Chance Multiplier", 0.2f, "Any open bolted gun.");*/

			failureToFeedRate = Config.Bind("Failures - Firearm", "Failure to Feed Rate", 0.25f, "Valid numbers are 0-100");
			FailureToExtractRate = Config.Bind("Failures - Firearm", "Failure to Eject Rate", 0.15f, "Valid numbers are 0-100");
			DoubleFeedRate = Config.Bind("Failures - Firearm", "Double Feed Rate", 0.15f, "Valid numbers are 0-100");
			StovepipeRate = Config.Bind("Failures - Firearm", "Stovepipe Rate", 0.1f, "Valid numbers are 0-100");

			//			var harmoney = new Harmony(Info.Metadata.GUID);
			Harmony.CreateAndPatchAll(typeof(FirearmFailures));
			rnd = new System.Random();
		}



		[HarmonyPatch(typeof(ClosedBoltWeapon), "BeginChamberingRound")]
		[HarmonyPatch(typeof(OpenBoltReceiver), "BeginChamberingRound")]
		[HarmonyPatch(typeof(Handgun), "ExtractRound")]
		[HarmonyPrefix]
		static bool FTFPatch()
		{
			if (!enableFirearmFailures.Value) { return true; }
			var rand = (float)rnd.Next(0, 10001) / 100;
			if (enableConsoleDebugging.Value) { Debug.Log("Random number generated for FTF: " + rand); };
			if (rand <= failureToFeedRate.Value * generalMult.Value)
			{
				if (enableConsoleDebugging.Value) { Debug.Log("Failure to feed!"); };
				return false;
			}
			return true;
		}

/*		[HarmonyPatch(typeof(Handgun), "CockHammer")]
		[HarmonyPrefix]
		static bool HammerFollowPatch(bool ___isManual)
		{
			var rand = (float)rnd.Next(0, 10001) / 100;
			Debug.Log("Random number generated for HammerFollow: " + rand);
			if (rand <= HammerFollowRate.Value && !___isManual)
			{
				Debug.Log("Hammer follow!");
				return false;
			}
			return true;
		}*/
		[HarmonyPatch(typeof(ClosedBolt), "ImpartFiringImpulse")]
		[HarmonyPatch(typeof(HandgunSlide), "ImpartFiringImpulse")]
		[HarmonyPatch(typeof(OpenBoltReceiverBolt), "ImpartFiringImpulse")]
		[HarmonyPrefix]
		static bool FTEPatch(FVRInteractiveObject __instance)
		{
			if (!enableFirearmFailures.Value) { return true; }
			var rand = (float)rnd.Next(0, 10001) / 100;
			if (enableConsoleDebugging.Value) { Debug.Log("Random number generated for Stovepipe: " + rand); }
			if (rand >= 100 - StovepipeRate.Value * generalMult.Value)
			{
				if (enableConsoleDebugging.Value) { Debug.Log("Stovepipe!"); }
				__instance.RotationInterpSpeed = 2;
				return false;
			}
			if (enableConsoleDebugging.Value) { Debug.Log("Random number generated for FTE: " + rand); }
			if (rand <= FailureToExtractRate.Value * generalMult.Value)
			{
				if (enableConsoleDebugging.Value) { Debug.Log("Failure to eject!"); }
				return false;
			}
			return true;
		}

/*		[HarmonyPatch(typeof(HandgunSlide), "UpdateSlide")]
		[HarmonyPrefix]
		static bool StovepipeHandgunSlidePatch(
			HandgunSlide __instance,
			float ___m_slideZ_forward,
			float ___m_slideZ_rear,
			float ___m_slideZ_lock,
			float ___m_slideZ_current)
		{
			if (prevSlideZLock == -999f)
			{
				prevSlideZLock = ___m_slideZ_lock;
			}

			if (__instance.RotationInterpSpeed == 2)
			{
				if (__instance.CurPos == HandgunSlide.SlidePos.Rear)
				{
					if (enableConsoleDebugging.Value) { Debug.Log("Stovepipe cleared!"); }
					__instance.RotationInterpSpeed = 1;
					___m_slideZ_lock = prevSlideZLock;
				}
				else
				{
					var m_slideStovePipe = ___m_slideZ_forward - (___m_slideZ_forward - ___m_slideZ_rear) / 2;
					if (___m_slideZ_current > m_slideStovePipe)
					{
						___m_slideZ_current = m_slideStovePipe;
					}
//				Debug.Log("m_slideStovePipe: " + m_slideStovePipe);
				}

			}
			return true;
		}
/*		[HarmonyPatch(typeof(Handgun), "IsSlideCatchEngaged")]
		[HarmonyPrefix]
		static bool StovepipeHandgunPatch(Handgun __instance, ref bool __result)
		{
			if (__instance.Slide.RotationInterpSpeed == 2)
			{
				__instance.IsSlideLockUp = true;
			}
			return true;
		}*/
	}
}
