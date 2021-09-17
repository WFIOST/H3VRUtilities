using System.Collections.Generic;
using FistVR;
using HarmonyLib;
using UnityEngine;

namespace H3VRUtilsConfig.QOLPatches
{
	public class OcclusionPatch
	{
		[HarmonyPatch(typeof(GM), "OnLevelWasLoaded")]
		[HarmonyPostfix]
		public void GMPatch_OnLevelWasLoaded_AddOCToStatics()
		{
			var go = new GameObject();
			go.AddComponent(typeof(OcclusionHandler));
		}
	}
}