using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using UnityEngine.Serialization;

namespace H3VRUtils.UniqueCode
{
	class LebelCutoff : FVRInteractiveObject
	{
		[FormerlySerializedAs("TubeMagazine")] public FVRFireArmMagazine tubeMagazine;
		[FormerlySerializedAs("Firearm")] public FVRFireArm firearm;
		[FormerlySerializedAs("CutoffSwitchFalse")] public Transform cutoffSwitchFalse;
		[FormerlySerializedAs("CutoffSwitchTrue")] public Transform cutoffSwitchTrue;
		[FormerlySerializedAs("CutoffSwitch")] public GameObject cutoffSwitch;
		[FormerlySerializedAs("CutoffFlagFalse")] public Transform cutoffFlagFalse;
		[FormerlySerializedAs("CutoffFlagTrue")] public Transform cutoffFlagTrue;
		[FormerlySerializedAs("CutoffFlag")] public GameObject cutoffFlag;
		public bool isCutoff;

		public override void SimpleInteraction(FVRViveHand hand)
		{
			firearm.PlayAudioEvent(FirearmAudioEventType.Safety);

			isCutoff = !isCutoff;

			if (isCutoff)
			{
				cutoffSwitch.transform.position = cutoffSwitchTrue.position;
				cutoffSwitch.transform.rotation = cutoffSwitchTrue.rotation;

				cutoffFlag.transform.position = cutoffFlagTrue.position;
				cutoffFlag.transform.rotation = cutoffFlagTrue.rotation;
				firearm.Magazine = null;
			}
			else
			{
				cutoffSwitch.transform.position = cutoffSwitchFalse.position;
				cutoffSwitch.transform.rotation = cutoffSwitchFalse.rotation;

				cutoffFlag.transform.position = cutoffFlagFalse.position;
				cutoffFlag.transform.rotation = cutoffFlagFalse.rotation;
				firearm.Magazine = tubeMagazine;
			}
		}
	}
}
