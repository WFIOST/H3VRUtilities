using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;

namespace H3VRUtils
{
	class H3VRUtilsMagRelease : FVRInteractiveObject
	{

		public ClosedBoltWeapon ClosedBoltReceiver;
		public OpenBoltReceiver OpenBoltWeapon;
		public Handgun HandgunReceiver;

		[HideInInspector] public int WepType;

		public bool PressDownToRelease;
		public enum TouchpadDirType
		{
			Up,
			Down,
			Left,
			Right
		}
		public TouchpadDirType TouchpadDir;


		protected override void Awake()
		{
			base.Awake();
			if (ClosedBoltReceiver != null) WepType = 1;
			if (OpenBoltWeapon != null) WepType = 2;
			if (HandgunReceiver != null) WepType = 3;
		}

		public override bool IsInteractable()
		{
			if (WepType == 1) return !(this.ClosedBoltReceiver.Magazine == null);
			if (WepType == 2) return !(this.OpenBoltWeapon.Magazine == null);
			return !(this.HandgunReceiver.Magazine == null);
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			bool flag = false;

			if (WepType == 1)
			{
				if (this.ClosedBoltReceiver.Magazine != null) flag = true;
			}
			if (WepType == 2)
			{
				if (this.OpenBoltWeapon.Magazine != null) flag = true;
			}
			if (WepType == 3)
			{
				if (this.HandgunReceiver.Magazine != null) flag = true;
			}

			if (flag)
			{
				bool flag2 = false;
				if (TouchpadDir == TouchpadDirType.Up && hand.Input.TouchpadNorthDown) flag2 = true;
				if (TouchpadDir == TouchpadDirType.Down && hand.Input.TouchpadSouthDown) flag2 = true;
				if (TouchpadDir == TouchpadDirType.Left && hand.Input.TouchpadWestDown) flag2 = true;
				if (TouchpadDir == TouchpadDirType.Right && hand.Input.TouchpadEastDown) flag2 = true;

				if (flag2 || !PressDownToRelease || hand.IsInStreamlinedMode && hand.Input.AXButtonPressed)
				{
					this.EndInteraction(hand);
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

					hand.ForceSetInteractable(magazine);
					magazine.BeginInteraction(hand);
				}
			}
		}
	}
}
