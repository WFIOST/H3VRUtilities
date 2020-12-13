using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace H3VRUtils
{
	public class H3VRUtilsPhysBoltRelease : FVRInteractiveObject
	{
		public ClosedBoltWeapon ClosedBoltReceiver;
		[HideInInspector]
		public OpenBoltReceiver OpenBoltWeapon;
		[HideInInspector]
		public Handgun HandgunReceiver;

		public bool ButtonPressToRelease;

		public enum TouchpadDirType
		{
			Up,
			Down,
			Left,
			Right,
			Trigger
		}
		public TouchpadDirType TouchpadDir;

		[HideInInspector]
		public int WepType = 0;

		protected override void Awake()
		{
			base.Awake();
			if (ClosedBoltReceiver != null) WepType = 1;
			if (OpenBoltWeapon != null) WepType = 2;
			if (HandgunReceiver != null) WepType = 3;
			if (TouchpadDir == TouchpadDirType.Trigger) this.IsSimpleInteract = true;
		}

		protected void OnHoverStay(FVRViveHand hand)
		{
			if (TouchpadDir == TouchpadDirType.Trigger && !hand.IsInStreamlinedMode) return;
			if (hand.IsInStreamlinedMode && !hand.Input.AXButtonPressed) return;
			ReleaseBolt(hand);
		}

		public override void SimpleInteraction(FVRViveHand hand)
		{
			if (IsSimpleInteract == false) return;
			ReleaseBolt(hand);
		}

		public void ReleaseBolt(FVRViveHand hand, bool _forceDrop = false)
		{
			bool flag = false;

			if (WepType == 1)
			{
				if (this.ClosedBoltReceiver.m_hand != null || hand != this.ClosedBoltReceiver.m_hand) flag = true;
			}
			if (WepType == 2)
			{
				if (this.OpenBoltWeapon.m_hand != null || hand != this.OpenBoltWeapon.m_hand) flag = true;
			}
			if (WepType == 3)
			{
				if (this.HandgunReceiver.m_hand != null || hand != this.HandgunReceiver.m_hand) flag = true;
			}

			if (flag)
			{
				bool flag2 = false;
				if (TouchpadDir == TouchpadDirType.Up && hand.Input.TouchpadNorthDown) flag2 = true;
				if (TouchpadDir == TouchpadDirType.Down && hand.Input.TouchpadSouthDown) flag2 = true;
				if (TouchpadDir == TouchpadDirType.Left && hand.Input.TouchpadWestDown) flag2 = true;
				if (TouchpadDir == TouchpadDirType.Right && hand.Input.TouchpadEastDown) flag2 = true;
				if (TouchpadDir == TouchpadDirType.Trigger) flag2 = true;

				if (flag2 == true)
				{
					if (WepType == 1)
					{
						this.ClosedBoltReceiver.Bolt.ReleaseBolt();
					}
					if (WepType == 2)
					{
						
					}
					if (WepType == 3)
					{
						
					}
				}
			}



				if (this.ClosedBoltReceiver.m_hand == null || hand != this.ClosedBoltReceiver.m_hand)
			{
				if (hand.Input.TouchpadNorthDown) this.ClosedBoltReceiver.Bolt.ReleaseBolt();
			}
		}
	}
}
