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

		public bool ButtonPressToRelease;

		[HideInInspector]
		public Vector2 dir;

		public H3VRUtilsMagRelease.TouchpadDirType TouchpadDir;

		[HideInInspector]
		public int WepType = 0;

		public override void Awake()
		{
			base.Awake();
			if (ClosedBoltReceiver != null) WepType = 1;
		}

		public override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			if (TouchpadDir == H3VRUtilsMagRelease.TouchpadDirType.Up) dir = Vector2.up;
			if (TouchpadDir == H3VRUtilsMagRelease.TouchpadDirType.Down) dir = Vector2.down;
			if (TouchpadDir == H3VRUtilsMagRelease.TouchpadDirType.Left) dir = Vector2.left;
			if (TouchpadDir == H3VRUtilsMagRelease.TouchpadDirType.Right) dir = Vector2.right;
			if (TouchpadDir == H3VRUtilsMagRelease.TouchpadDirType.Trigger) this.IsSimpleInteract = true; else this.IsSimpleInteract = false;
		}

		protected void OnHoverStay(FVRViveHand hand)
		{
			if (TouchpadDir == H3VRUtilsMagRelease.TouchpadDirType.Trigger && !hand.IsInStreamlinedMode) return;
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
			bool flag2 = false;
			if (Vector2.Angle(hand.Input.TouchpadAxes, dir) <= 45f && hand.Input.TouchpadDown && hand.Input.TouchpadAxes.magnitude > 0.2f) flag2 = true;

			if (flag2 || hand.IsInStreamlinedMode && hand.Input.AXButtonPressed)
			{
				this.ClosedBoltReceiver.Bolt.ReleaseBolt();
			}

			if (this.ClosedBoltReceiver.m_hand == null || hand != this.ClosedBoltReceiver.m_hand)
			{

			}
		}
	}
}
