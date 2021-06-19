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
		static bool ClosedBoltForcePaddleOnPatch(ClosedBoltWeapon instance)
		{
			var f = MagReplacerData.GetPaddleData().Concat(MagReplacerData.GetMagDropData()).ToArray();
			foreach(var id in f)
			{
				if (instance.ObjectWrapper.ItemID == id)
				{
					Debug.Log("Applying paddle release to object ID " + id);
					var objs = FindObjectsOfType<ClosedBoltMagEjectionTrigger>(); //fuck your cpu
					foreach (var files in objs)
					{
						if (files.transform.parent == instance.transform)
						{
							var mr = files.gameObject.AddComponent(typeof(H3VRUtilsMagRelease)) as H3VRUtilsMagRelease;
							mr.PositionInterpSpeed = 1;
							mr.RotationInterpSpeed = 1;
							mr.EndInteractionIfDistant = true;
							mr.EndInteractionDistance = 0.25f;
							mr.closedBoltReceiver = files.Receiver;
							mr.pressDownToRelease = true;

							mr.touchpadDir = MagReplacerData.GetPaddleData().Contains(id) ? H3VRUtilsMagRelease.TouchpadDirType.Down : H3VRUtilsMagRelease.TouchpadDirType.NoDirection;

							mr.SetWepType();
							Destroy(files);
							break;
						}
					}
					break;
				}
			}
			return true;
		}
	}


	static class MagReplacerData
	{
		public struct Directories
		{
			public static string PaddleMagReleaseLoc = Directory.GetCurrentDirectory() + "/H3VRUtilities/ForcePaddleMagRelease.txt";
			public static string ForcedMagDrop = Directory.GetCurrentDirectory() + "/H3VRUtilities/ForceForcedMagDrop.txt";
		}

		private static string[] _savedPaddleData = null;
		public static string[] GetPaddleData(bool reset = false)
		{
			if (!File.Exists(Directories.PaddleMagReleaseLoc)) { File.CreateText(Directories.PaddleMagReleaseLoc); }
			if (_savedPaddleData != null && !reset) return _savedPaddleData;
			_savedPaddleData = File.ReadAllLines(Directories.PaddleMagReleaseLoc);
			return _savedPaddleData;
		}

		private static string[] _savedMagDropData = null;
		public static string[] GetMagDropData(bool reset = false)
		{
			if (!File.Exists(Directories.ForcedMagDrop)) { File.CreateText(Directories.ForcedMagDrop); }
			if (_savedMagDropData != null && !reset) return _savedMagDropData;
			_savedMagDropData = File.ReadAllLines(Directories.ForcedMagDrop);
			return _savedMagDropData;
		}
	}
}
