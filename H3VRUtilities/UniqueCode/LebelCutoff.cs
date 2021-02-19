using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace H3VRUtils.UniqueCode
{
	class LebelCutoff : FVRInteractiveObject
	{
		public FVRFireArmMagazine TubeMagazine;
		public FVRFireArm Firearm;
		public Transform CutoffSwitchFalse;
		public Transform CutoffSwitchTrue;
		public GameObject CutoffSwitch;
		public Transform CutoffFlagFalse;
		public Transform CutoffFlagTrue;
		public GameObject CutoffFlag;
		public bool isCutoff;

		public override void SimpleInteraction(FVRViveHand hand)
		{
			Firearm.PlayAudioEvent(FirearmAudioEventType.Safety);

			isCutoff = !isCutoff;

			if (isCutoff)
			{
				CutoffSwitch.transform.position = CutoffSwitchTrue.position;
				CutoffSwitch.transform.rotation = CutoffSwitchTrue.rotation;

				CutoffFlag.transform.position = CutoffFlagTrue.position;
				CutoffFlag.transform.rotation = CutoffFlagTrue.rotation;
				Firearm.Magazine = null;
			}
			else
			{
				CutoffSwitch.transform.position = CutoffSwitchFalse.position;
				CutoffSwitch.transform.rotation = CutoffSwitchFalse.rotation;

				CutoffFlag.transform.position = CutoffFlagFalse.position;
				CutoffFlag.transform.rotation = CutoffFlagFalse.rotation;
				Firearm.Magazine = TubeMagazine;
			}
		}
	}
}
