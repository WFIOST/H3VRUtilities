using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using HarmonyLib;
using FistVR;
using H3VRUtils;
using BepInEx.Logging;

namespace H3VRUtils
{
	class MagReplacer : MonoBehaviour
	{
		[HarmonyPatch(typeof(ClosedBoltWeapon), "Awake")]
		[HarmonyPrefix]
		static bool ClosedBoltForcePaddleOnPatch(ClosedBoltWeapon __instance)
		{
			GetAndAddUniqueMagRelease(__instance.ObjectWrapper.ItemID, __instance.transform, "ClosedBoltWeapon");
			return true;
		}

		static void GetAndAddUniqueMagRelease(string itemID, Transform objTransform, string WepType)
		{
			string[] f = MagReplacerData.GetPaddleData().Concat(MagReplacerData.GetMagDropData()).ToArray();
			foreach (var id in f)
			{
				if (itemID == id)
				{
					Debug.Log("Applying paddle release to object ID " + id);
					if (WepType == "ClosedBoltWeapon")
					{
						ApplyUniqueMagReleaseClosedBolt(itemID, objTransform);
					} else
					if (WepType == "OpenBoltReceiver")
					{
						ApplyUniqueMagReleaseOpenBolt(itemID, objTransform);
					}
					break;
				}
			}
		}

		static void ApplyUniqueMagReleaseClosedBolt(string itemID, Transform objTransform)
		{
			var objs = FindObjectsOfType<ClosedBoltMagEjectionTrigger>(); //fuck your cpu
			foreach (var files in objs)
			{
				if (files.transform.parent == objTransform)
				{
					var mr = files.gameObject.AddComponent(typeof(H3VRUtilsMagRelease)) as H3VRUtilsMagRelease;
					mr.PositionInterpSpeed = 1;
					mr.RotationInterpSpeed = 1;
					mr.EndInteractionIfDistant = true;
					mr.EndInteractionDistance = 0.25f;
					mr.ClosedBoltReceiver = files.Receiver;
					mr.PressDownToRelease = true;

					if (MagReplacerData.GetPaddleData().Contains(itemID))
					{
						mr.TouchpadDir = H3VRUtilsMagRelease.TouchpadDirType.Down;
					}
					else
					{
						mr.TouchpadDir = H3VRUtilsMagRelease.TouchpadDirType.NoDirection;
					}

					mr.setWepType();
					Destroy(files);
					break;
				}
			}
		}

		static void ApplyUniqueMagReleaseOpenBolt(string itemID, Transform objTransform)
		{
			var objs = FindObjectsOfType<OpenBoltMagReleaseTrigger>(); //fuck your cpu
			foreach (var files in objs)
			{
				if (files.transform.parent == objTransform)
				{
					var mr = files.gameObject.AddComponent(typeof(H3VRUtilsMagRelease)) as H3VRUtilsMagRelease;
					mr.PositionInterpSpeed = 1;
					mr.RotationInterpSpeed = 1;
					mr.EndInteractionIfDistant = true;
					mr.EndInteractionDistance = 0.25f;
					mr.OpenBoltWeapon = files.Receiver;
					mr.PressDownToRelease = true;

					if (MagReplacerData.GetPaddleData().Contains(itemID))
					{
						mr.TouchpadDir = H3VRUtilsMagRelease.TouchpadDirType.Down;
					}
					else
					{
						mr.TouchpadDir = H3VRUtilsMagRelease.TouchpadDirType.NoDirection;
					}

					mr.setWepType();
					Destroy(files);
					break;
				}
			}
		}
	}


	static class MagReplacerData
	{
		public struct dirs
		{
			public static string PaddleMagReleaseLoc = Directory.GetCurrentDirectory() + "/H3VRUtilities/ForcePaddleMagRelease.txt";
			public static string ForcedMagDrop = Directory.GetCurrentDirectory() + "/H3VRUtilities/ForceForcedMagDrop.txt";
		}

		static string[] SavedPaddleData = null;
		public static string[] GetPaddleData(bool reset = false)
		{
			if (!File.Exists(dirs.PaddleMagReleaseLoc)) { File.CreateText(dirs.PaddleMagReleaseLoc); }
			if (SavedPaddleData != null && !reset) return SavedPaddleData;
			SavedPaddleData = File.ReadAllLines(dirs.PaddleMagReleaseLoc);
			return SavedPaddleData;
		}

		static string[] SavedMagDropData = null;
		public static string[] GetMagDropData(bool reset = false)
		{
			if (!File.Exists(dirs.ForcedMagDrop)) { File.CreateText(dirs.ForcedMagDrop); }
			if (SavedMagDropData != null && !reset) return SavedMagDropData;
			SavedMagDropData = File.ReadAllLines(dirs.ForcedMagDrop);
			return SavedMagDropData;
		}
	}
}
