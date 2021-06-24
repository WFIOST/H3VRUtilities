using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;

namespace H3VRUtils
{
	public class H3VRUtilsMagRelease : FVRInteractiveObject
	{

		public ClosedBoltWeapon ClosedBoltReceiver;
		public OpenBoltReceiver OpenBoltWeapon;
		public Handgun HandgunReceiver;
		public BoltActionRifle BoltActionWeapon;

		[HideInInspector] public int WepType;

		[HideInInspector] public bool DisallowEjection;

		private FVRFireArmMagazine mag;

		private Collider col;

		public bool PressDownToRelease;
		public enum TouchpadDirType
		{
			Up,
			Down,
			Left,
			Right,
			Trigger,
			NoDirection
		}
		public TouchpadDirType TouchpadDir;

		public Vector2 dir;


		public override void Awake()
		{
			base.Awake();
			setWepType();
			col = GetComponent<Collider>();
		}

		public void setWepType()
		{
			if (ClosedBoltReceiver != null) WepType = 1;
			if (OpenBoltWeapon != null) WepType = 2;
			if (HandgunReceiver != null) WepType = 3;
			if (BoltActionWeapon != null) WepType = 4;
		}

		public override bool IsInteractable()
		{
			if (WepType == 1) return !(this.ClosedBoltReceiver.Magazine == null);
			if (WepType == 2) return !(this.OpenBoltWeapon.Magazine == null);
			if (WepType == 3) return !(this.HandgunReceiver.Magazine == null);
			return !(this.BoltActionWeapon.Magazine == null);
		}

		public override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			dir = Vector2.up;

			//config override
			if (UtilsBepInExLoader.paddleMagReleaseDir.Value != UtilsBepInExLoader.TouchpadDirTypePT.BasedOnWeapon)
			{
				TouchpadDir = (TouchpadDirType)(int)UtilsBepInExLoader.paddleMagReleaseDir.Value;
			}

			if (TouchpadDir == TouchpadDirType.Up) dir = Vector2.up;
			if (TouchpadDir == TouchpadDirType.Down) dir = Vector2.down;
			if (TouchpadDir == TouchpadDirType.Left) dir = Vector2.left;
			if (TouchpadDir == TouchpadDirType.Right) dir = Vector2.right;
			if (TouchpadDir == TouchpadDirType.Trigger) this.IsSimpleInteract = true; else this.IsSimpleInteract = false;
			col.enabled = !DisallowEjection;
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
		}

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			if (TouchpadDir == TouchpadDirType.Trigger)
				dropmag(hand);
		}

		public void dropmag(FVRViveHand hand, bool _override = false)
		{
			if (DisallowEjection && !_override) return;
			FVRFireArmMagazine magazine = null;

			if (WepType == 1)
			{
				magazine = this.ClosedBoltReceiver.Magazine;
				this.ClosedBoltReceiver.ReleaseMag();
			}
			if (WepType == 2)
			{
				magazine = this.OpenBoltWeapon.Magazine;
				this.OpenBoltWeapon.ReleaseMag();
			}
			if (WepType == 3)
			{
				magazine = this.HandgunReceiver.Magazine;
				this.HandgunReceiver.ReleaseMag();
			}
			if (WepType == 4)
			{
				magazine = this.BoltActionWeapon.Magazine;
				this.BoltActionWeapon.ReleaseMag();
			}
			movemagtohand(hand, magazine);
		}

		public void movemagtohand(FVRViveHand hand, FVRFireArmMagazine magazine)
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
			if (mag != null) { flag = true; prevmag = mag; } //check if mag was previously loaded

			if (WepType == 1) { mag = this.ClosedBoltReceiver.Magazine; }
			if (WepType == 2) { mag = this.OpenBoltWeapon.Magazine; }
			if (WepType == 3) { mag = this.HandgunReceiver.Magazine; }
			if (WepType == 4) { mag = this.BoltActionWeapon.Magazine; }

			if (mag != null)
			{
				bool flag2 = Vector2.Angle(hand.Input.TouchpadAxes, dir) <= 45f && hand.Input.TouchpadDown && hand.Input.TouchpadAxes.magnitude > 0.2f;


				if (
					!PressDownToRelease //if it's not a paddle release anyway
					|| !UtilsBepInExLoader.paddleMagRelease.Value //if paddle release is disabled
					|| (TouchpadDir == TouchpadDirType.NoDirection && !UtilsBepInExLoader.magDropRequiredRelease.Value) //if mag drop required and mag drop is disabled
					|| flag2 //if it is enabled, and user is pressing all the right buttons
					|| (hand.IsInStreamlinedMode && hand.Input.AXButtonPressed)) //if it is enabled, and user is pressing streamlined button (and is in steamlined mode)
				{
					if (TouchpadDir == TouchpadDirType.NoDirection && UtilsBepInExLoader.magDropRequiredRelease.Value) return;
					dropmag(hand);
					this.EndInteraction(hand);
				}
			}
			else
			{
				if (flag) //if mag was previously loaded, but is now not
				{
					movemagtohand(hand, prevmag);
				}
				this.EndInteraction(hand);
			}
		}
	}
}
