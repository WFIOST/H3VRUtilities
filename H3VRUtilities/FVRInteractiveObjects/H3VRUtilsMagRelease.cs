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


		protected override void Awake()
		{
			base.Awake();
			if (ClosedBoltReceiver != null) WepType = 1;
			if (OpenBoltWeapon != null) WepType = 2;
			if (HandgunReceiver != null) WepType = 3;
			col = GetComponent<Collider>();
		}

		public override bool IsInteractable()
		{
			if (WepType == 1) return !(this.ClosedBoltReceiver.Magazine == null);
			if (WepType == 2) return !(this.OpenBoltWeapon.Magazine == null);
			return !(this.HandgunReceiver.Magazine == null);
		}

		protected override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			dir = Vector2.up;
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

			if (mag != null)
			{
				bool flag2 = false;
				if (Vector2.Angle(hand.Input.TouchpadAxes, dir) <= 45f && hand.Input.TouchpadDown && hand.Input.TouchpadAxes.magnitude > 0.2f) flag2 = true;

				if (flag2 || !PressDownToRelease || hand.IsInStreamlinedMode && hand.Input.AXButtonPressed)
				{
					if (TouchpadDir == TouchpadDirType.NoDirection) return;
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
