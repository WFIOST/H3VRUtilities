using FistVR;
using HarmonyLib;
using UnityEngine;

namespace H3VRUtilsConfig.QOLPatches
{
	public class LaserPointerPatch : MonoBehaviour
	{
		[HarmonyPatch(typeof(LaserPointer), "FVRUpdate")]
		[HarmonyPostfix]
		public static void LaserPointerPatch_Update_FixScale(LaserPointer __instance)
		{
			//get dist between player head and laser
			float num = Vector3.Distance(ManagerSingleton<GM>.Instance.m_currentPlayerBody.Head.transform.position, __instance.BeamHitPoint.transform.position);
			float num2 = Mathf.Lerp(0.01f, 0.2f, num * 0.01f);
			__instance.BeamHitPoint.transform.localScale = new Vector3(num2, num2, num2);
		}
	}
}
