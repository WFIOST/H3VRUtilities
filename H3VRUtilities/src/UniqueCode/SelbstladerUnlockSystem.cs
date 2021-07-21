using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace H3VRUtils.UniqueCode
{
	public class SelbstladerUnlockSystem : FVRInteractiveObject
	{
		public GameObject lockingpiece;
		public Transform lockingpieceunlocked;
		public Transform lockingpiecelocked;
		public ClosedBoltWeapon wep;
		public H3VRUtilsMagRelease magrelease;
		public FVRFireArmReloadTriggerWell magreloadtrigger;

		private float _velocity;

		private bool _unlocked;

		private Collider _col;
		private Collider _mrtcol;

		public enum ChangePositionType
		{
			Swap,
			Unlock,
			Lock
		}

		public enum Locktype
		{
			MagLocking,
			BoltLocking
		}

		public Locktype locker;

		public override void Awake()
		{
			base.Awake();
			IsSimpleInteract = true;
			_col = wep.Bolt.GetComponent<Collider>();
			ChangePosition(ChangePositionType.Lock);
			_velocity = wep.Chamber.ChamberVelocityMultiplier;
			if (locker == Locktype.MagLocking) _mrtcol = magreloadtrigger.GetComponent<Collider>();
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

		public override void FVRUpdate()
		{
			base.FVRUpdate();
			if (locker == Locktype.BoltLocking)
			{
				if (wep.Bolt.CurPos == ClosedBolt.BoltPos.ForwardToMid) ChangePosition(ChangePositionType.Lock);
				if (wep.Bolt.CurPos == ClosedBolt.BoltPos.Locked) ChangePosition(ChangePositionType.Unlock);
				if (_unlocked) wep.Chamber.ChamberVelocityMultiplier = 0.1f; else wep.Chamber.ChamberVelocityMultiplier = _velocity;
			}
			if (locker == Locktype.MagLocking)
			{
				if (wep.Bolt.CurPos == ClosedBolt.BoltPos.ForwardToMid && _unlocked) magrelease.Dropmag(null, true);
			}
		}


		public void ChangePosition(ChangePositionType ct)
		{
			if (ct == ChangePositionType.Swap) _unlocked = !_unlocked;
			if (ct == ChangePositionType.Unlock) _unlocked = true;
			if (ct == ChangePositionType.Lock) _unlocked = false;

			if (_unlocked)
			{
				lockingpiece.transform.localPosition = lockingpieceunlocked.localPosition;
				lockingpiece.transform.localRotation = lockingpieceunlocked.localRotation;
			}
			else
			{
				lockingpiece.transform.localPosition = lockingpiecelocked.localPosition;
				lockingpiece.transform.localRotation = lockingpiecelocked.localRotation;
			}
			if (locker == Locktype.BoltLocking) _col.enabled = _unlocked;
			if (locker == Locktype.MagLocking)
			{
				magrelease.disallowEjection = !_unlocked;
				_mrtcol.enabled = _unlocked;
			}
		}
	}
}
