using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using UnityEngine.Serialization;

namespace H3VRUtils
{
	public class H3VRUtilsPhysBoltRelease : FVRInteractiveObject
	{
		[FormerlySerializedAs("ClosedBoltReceiver")] public ClosedBoltWeapon closedBoltReceiver;

		[FormerlySerializedAs("ButtonPressToRelease")] public bool buttonPressToRelease;

		[HideInInspector]
		public Vector2 dir;

		[FormerlySerializedAs("TouchpadDir")] public H3VRUtilsMagRelease.TouchpadDirType touchpadDir;

		[FormerlySerializedAs("WepType")] [HideInInspector]
		public int wepType = 0;

		public override void Awake()
		{
			base.Awake();
			if (closedBoltReceiver != null) wepType = 1;
		}

		public override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			if (touchpadDir == H3VRUtilsMagRelease.TouchpadDirType.Up) dir = Vector2.up;
			if (touchpadDir == H3VRUtilsMagRelease.TouchpadDirType.Down) dir = Vector2.down;
			if (touchpadDir == H3VRUtilsMagRelease.TouchpadDirType.Left) dir = Vector2.left;
			if (touchpadDir == H3VRUtilsMagRelease.TouchpadDirType.Right) dir = Vector2.right;
			if (touchpadDir == H3VRUtilsMagRelease.TouchpadDirType.Trigger) this.IsSimpleInteract = true; else this.IsSimpleInteract = false;
		}

		protected void OnHoverStay(FVRViveHand hand)
		{
			if (touchpadDir == H3VRUtilsMagRelease.TouchpadDirType.Trigger && !hand.IsInStreamlinedMode) return;
			if (hand.IsInStreamlinedMode && !hand.Input.AXButtonPressed) return;
			ReleaseBolt(hand);
		}

		public override void SimpleInteraction(FVRViveHand hand)
		{
			if (IsSimpleInteract == false) return;
			ReleaseBolt(hand);
		}

		public void ReleaseBolt(FVRViveHand hand, bool forceDrop = false)
		{
			bool flag2 = false;
			if (Vector2.Angle(hand.Input.TouchpadAxes, dir) <= 45f && hand.Input.TouchpadDown && hand.Input.TouchpadAxes.magnitude > 0.2f) flag2 = true;

			if (flag2 || hand.IsInStreamlinedMode && hand.Input.AXButtonPressed)
			{
				this.closedBoltReceiver.Bolt.ReleaseBolt();
			}

			if (this.closedBoltReceiver.m_hand == null || hand != this.closedBoltReceiver.m_hand)
			{

			}
		}
	}
}
