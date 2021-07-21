using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;
using UnityEngine.Serialization;

namespace H3VRUtils
{
	public class H3VRUtilsMagRelease : FVRInteractiveObject
	{

		[FormerlySerializedAs("ClosedBoltReceiver")] public ClosedBoltWeapon closedBoltReceiver;
		[FormerlySerializedAs("OpenBoltWeapon")] public OpenBoltReceiver openBoltWeapon;
		[FormerlySerializedAs("HandgunReceiver")] public Handgun handgunReceiver;
		[FormerlySerializedAs("BoltActionWeapon")] public BoltActionRifle boltActionWeapon;

		[FormerlySerializedAs("WepType")] [HideInInspector] public int wepType;

		[FormerlySerializedAs("DisallowEjection")] [HideInInspector] public bool disallowEjection;

		private FVRFireArmMagazine _mag;

		private Collider _col;

		[FormerlySerializedAs("PressDownToRelease")] public bool pressDownToRelease;
		public enum TouchpadDirType
		{
			Up,
			Down,
			Left,
			Right,
			Trigger,
			NoDirection
		}
		[FormerlySerializedAs("TouchpadDir")] public TouchpadDirType touchpadDir;

		public Vector2 dir;


		public override void Awake()
		{
			base.Awake();
			SetWepType();
			_col = GetComponent<Collider>();
		}

		public void SetWepType()
		{
			if (closedBoltReceiver != null) wepType = 1;
			if (openBoltWeapon != null) wepType = 2;
			if (handgunReceiver != null) wepType = 3;
			if (boltActionWeapon != null) wepType = 4;
		}

		public override bool IsInteractable()
		{
			if (wepType == 1) return !(this.closedBoltReceiver.Magazine == null);
			if (wepType == 2) return !(this.openBoltWeapon.Magazine == null);
			if (wepType == 3) return !(this.handgunReceiver.Magazine == null);
			return !(this.boltActionWeapon.Magazine == null);
		}

		public override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			dir = Vector2.up;

			//config override
			if (UtilsBepInExLoader.paddleMagReleaseDir.Value != UtilsBepInExLoader.TouchpadDirTypePt.BasedOnWeapon)
			{
				touchpadDir = (TouchpadDirType)(int)UtilsBepInExLoader.paddleMagReleaseDir.Value;
			}

			if (touchpadDir == TouchpadDirType.Up) dir = Vector2.up;
			if (touchpadDir == TouchpadDirType.Down) dir = Vector2.down;
			if (touchpadDir == TouchpadDirType.Left) dir = Vector2.left;
			if (touchpadDir == TouchpadDirType.Right) dir = Vector2.right;
			if (touchpadDir == TouchpadDirType.Trigger) this.IsSimpleInteract = true; else this.IsSimpleInteract = false;
			_col.enabled = !disallowEjection;
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
		}

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			if (touchpadDir == TouchpadDirType.Trigger)
				Dropmag(hand);
		}

		public void Dropmag(FVRViveHand hand, bool @override = false)
		{
			if (disallowEjection && !@override) return;
			FVRFireArmMagazine magazine = null;

			if (wepType == 1)
			{
				magazine = this.closedBoltReceiver.Magazine;
				this.closedBoltReceiver.ReleaseMag();
			}
			if (wepType == 2)
			{
				magazine = this.openBoltWeapon.Magazine;
				this.openBoltWeapon.ReleaseMag();
			}
			if (wepType == 3)
			{
				magazine = this.handgunReceiver.Magazine;
				this.handgunReceiver.ReleaseMag();
			}
			if (wepType == 4)
			{
				magazine = this.boltActionWeapon.Magazine;
				this.boltActionWeapon.ReleaseMag();
			}
			Movemagtohand(hand, magazine);
		}

		public void Movemagtohand(FVRViveHand hand, FVRFireArmMagazine magazine)
		{
			//puts mag in hand
			if (hand != null) { hand.ForceSetInteractable(magazine); }
			magazine.BeginInteraction(hand);
		}


		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);

			bool flag = false;
			FVRFireArmMagazine prevmag = null;
			if (_mag != null) { flag = true; prevmag = _mag; } //check if mag was previously loaded

			if (wepType == 1) { _mag = this.closedBoltReceiver.Magazine; }
			if (wepType == 2) { _mag = this.openBoltWeapon.Magazine; }
			if (wepType == 3) { _mag = this.handgunReceiver.Magazine; }
			if (wepType == 4) { _mag = this.boltActionWeapon.Magazine; }

			if (_mag != null)
			{
				bool flag2 = false;
				if (Vector2.Angle(hand.Input.TouchpadAxes, dir) <= 45f && hand.Input.TouchpadDown && hand.Input.TouchpadAxes.magnitude > 0.2f) flag2 = true;


				if (
					   !pressDownToRelease //if it's not a paddle release anyway
					|| !UtilsBepInExLoader.paddleMagRelease.Value //if paddle release is disabled
					|| (touchpadDir == TouchpadDirType.NoDirection && !UtilsBepInExLoader.magDropRequiredRelease.Value) //if mag drop required and mag drop is disabled
					|| flag2 //if it is enabled, and user is pressing all the right buttons
					|| (hand.IsInStreamlinedMode && hand.Input.AXButtonPressed)) //if it is enabled, and user is pressing streamlined button (and is in steamlined mode)
				{
					if (touchpadDir == TouchpadDirType.NoDirection && UtilsBepInExLoader.magDropRequiredRelease.Value) return;
					Dropmag(hand);
					this.EndInteraction(hand);
				}
			}
			else
			{
				if (flag) //if mag was previously loaded, but is now not
				{
					Movemagtohand(hand, prevmag);
				}
				this.EndInteraction(hand);
			}
		}
	}
}
