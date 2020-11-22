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
	class brokenFirearmFailures : BaseUnityPlugin
	{
		private static ConfigEntry<bool> enableBrokenFirearmFailures;

		private static ConfigEntry<bool> enableConsoleDebugging;

		private static ConfigEntry<float> generalMult;

		private static ConfigEntry<float> HammerFollowRate;
		private static ConfigEntry<float> failureToLockSlide;
		private static ConfigEntry<float> SlamfireRate;

		public static System.Random rnd;
		void Awake()
		{
			enableBrokenFirearmFailures = Config.Bind("_General Settings", "Enable Broken Firearm Failures", false, "Enables failures related to permanent firearm damage.");
			enableConsoleDebugging = Config.Bind("_General Settings", "Enable Console Debugging", false, "Exports values and failures to console.");

			generalMult = Config.Bind("_Multipliers", "Failure Chance Multiplier", 1f, "default at 1x is 1%, so this is a more 'pick failure percentage chance'.");

			HammerFollowRate = Config.Bind("Failures - Broken Firearm", "Hammer Follow Rate", 0.05f, "Valid numbers are 0-100");
			failureToLockSlide = Config.Bind("Failures - Broken Firearm", "Failure to Lock Slide Rate", 0.3f, "Valid numbers are 0-100");
			SlamfireRate = Config.Bind("Failures - Broken Firearm", "Slam Fire Rate", 0.05f, "Valid numbers are 0-100");

			Harmony.CreateAndPatchAll(typeof(FirearmFailures));
			rnd = new System.Random();
		}

		[HarmonyPatch(typeof(ClosedBoltWeapon), "CockHammer")]
		[HarmonyPrefix]
		static bool FailureToLockBackPatch()
		{
			if (!enableBrokenFirearmFailures.Value) { return true; }
			var rand = (float)rnd.Next(0, 10001) / 100;
			if (enableConsoleDebugging.Value) { Debug.Log("Random number generated for Hammer Follow: " + rand); };
			if (rand <= HammerFollowRate.Value * generalMult.Value)
			{
				if (enableConsoleDebugging.Value) { Debug.Log("Hammer follow!"); };
				return false;
			}
			return true;
		}

		[HarmonyPatch(typeof(ClosedBoltWeapon), "CockHammer")]
		[HarmonyPrefix]
		static bool hammerFollowClosedBoltPatch()
		{
			if (!enableBrokenFirearmFailures.Value) { return true; }
			var rand = (float)rnd.Next(0, 10001) / 100;
			if (enableConsoleDebugging.Value) { Debug.Log("Random number generated for Hammer Follow: " + rand); };
			if (rand <= HammerFollowRate.Value * generalMult.Value)
			{
				if (enableConsoleDebugging.Value) { Debug.Log("Hammer follow!"); };
				return false;
			}
			return true;
		}

		[HarmonyPatch(typeof(Handgun), "CockHammer")]
		[HarmonyPrefix]
		static bool hammerFollowHandgunPatch(bool ___isManual)
		{
			if (!enableBrokenFirearmFailures.Value) { return true; }
			var rand = (float)rnd.Next(0, 10001) / 100;
			if (enableConsoleDebugging.Value) { Debug.Log("Random number generated for Hammer Follow: " + rand); };
			if (rand <= HammerFollowRate.Value * generalMult.Value || !___isManual)
			{
				if (enableConsoleDebugging.Value) { Debug.Log("Hammer follow!"); };
				return false;
			}
				return true;
		}
	}
}
