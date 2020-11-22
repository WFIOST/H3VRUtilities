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
	class AmmunitionFailures : BaseUnityPlugin
	{

		private static ConfigEntry<bool> enableAmmunitionFailures;

		private static ConfigEntry<bool> enableConsoleDebugging;

		private static ConfigEntry<float> generalMult;

		private static ConfigEntry<float> lightPrimerStrikeFailureRate;
		private static ConfigEntry<float> HangFireRate;

		public static System.Random rnd;

		void Awaken()
		{
			enableAmmunitionFailures = Config.Bind("_General Settings", "Enable Ammunition Failures", true, "Enables ammunition related failures.");
			enableConsoleDebugging = Config.Bind("_General Settings", "Enable Console Debugging", false, "Exports values and failures to console.");
			
			generalMult = Config.Bind("_Multipliers", "Failure Chance Multiplier", 1f, "default at 1x is 1%, so this is a more 'pick failure percentage chance'.");
			
			lightPrimerStrikeFailureRate = Config.Bind("Failures - Ammo", "Light Primer Strike Failure Rate", 0.25f, "Valid numbers are 0-100");
			HangFireRate = Config.Bind("Failures - Ammo", "Hang Fire Rate", 0.1f, "Valid numbers are 0-100");

			Harmony.CreateAndPatchAll(typeof(FirearmFailures));
			rnd = new System.Random();
		}

		[HarmonyPatch(typeof(FVRFireArmChamber), "Fire")]
		[HarmonyPrefix]
		static bool LightPrimerStrikePatch(ref bool __result, FVRFireArmChamber __instance, FVRFireArmRound ___m_round)
		{
			var rand = (float)rnd.Next(0, 10001) / 100;
			if (!enableAmmunitionFailures.Value) { return true; }
			if (enableConsoleDebugging.Value) { Debug.Log("Random number generated for LightPrimerStrike: " + rand); }
			if (rand >= lightPrimerStrikeFailureRate.Value * generalMult.Value)
			{
				if (__instance.IsFull && ___m_round != null && !__instance.IsSpent)
				{
					__instance.IsSpent = true;
					__instance.UpdateProxyDisplay();
					__result = true;
					return false;
				}
			}
			else
			{
				/*				if (rand >= (100 - FailureToExtractRate.Value))
								{
									__instance.IsSpent = true;
									__instance.UpdateProxyDisplay();
									__result = false;
									return false;
								}*/


				if (enableConsoleDebugging.Value) { Debug.Log("Light primer strike!"); };
			}
			__result = false;
			return false;
		}
	}
}
