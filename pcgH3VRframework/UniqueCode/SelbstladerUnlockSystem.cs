using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace H3VRUtils.UniqueCode
{
	class SelbstladerUnlockSystem : FVRInteractiveObject
	{
		public GameObject lockingpiece;
		public Transform lockingpieceunlocked;
		public Transform lockingpiecelocked;
		public ClosedBoltWeapon wep;
		public H3VRUtilsMagRelease magrelease;
		public FVRFireArmReloadTriggerWell magreloadtrigger;

		private float velocity;

		private bool unlocked;

		private Collider col;
		private Collider mrtcol;

		public enum ChangePositionType
		{
			Swap,
			Unlock,
			Lock
		}

		public enum locktype
		{
			MagLocking,
			BoltLocking
		}

		public locktype locker;

		protected override void Awake()
		{
			base.Awake();
			IsSimpleInteract = true;
			col = wep.Bolt.GetComponent<Collider>();
			ChangePosition(ChangePositionType.Lock);
			velocity = wep.Chamber.ChamberVelocityMultiplier;
			if (locker == locktype.MagLocking) mrtcol = magreloadtrigger.GetComponent<Collider>();
		}

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			ChangePosition(ChangePositionType.Swap);
			try
			{
				wep.PlayAudioEvent(FirearmAudioEventType.BreachOpen);
			}
			catch
			{
				Console.WriteLine("SelbstladerUnlockSystem.cs failed to play the BreachOpen sound!");
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (locker == locktype.BoltLocking)
			{
				if (wep.Bolt.CurPos == ClosedBolt.BoltPos.ForwardToMid) ChangePosition(ChangePositionType.Lock);
				if (wep.Bolt.CurPos == ClosedBolt.BoltPos.Locked) ChangePosition(ChangePositionType.Unlock);
				if (unlocked) wep.Chamber.ChamberVelocityMultiplier = 0.1f; else wep.Chamber.ChamberVelocityMultiplier = velocity;
			}
			if (locker == locktype.MagLocking)
			{
				if (wep.Bolt.CurPos == ClosedBolt.BoltPos.ForwardToMid && unlocked) magrelease.dropmag(null, true);
			}
		}


		public void ChangePosition(ChangePositionType ct)
		{
			if (ct == ChangePositionType.Swap) unlocked = !unlocked;
			if (ct == ChangePositionType.Unlock) unlocked = true;
			if (ct == ChangePositionType.Lock) unlocked = false;

			if (unlocked)
			{
				lockingpiece.transform.localPosition = lockingpieceunlocked.localPosition;
				lockingpiece.transform.localRotation = lockingpieceunlocked.localRotation;
			}
			else
			{
				lockingpiece.transform.localPosition = lockingpiecelocked.localPosition;
				lockingpiece.transform.localRotation = lockingpiecelocked.localRotation;
			}
			if (locker == locktype.BoltLocking) col.enabled = unlocked;
			if (locker == locktype.MagLocking)
			{
				magrelease.DisallowEjection = !unlocked;
				mrtcol.enabled = unlocked;
			}
		}
	}
}
