using System;
using FistVR;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace H3VRUtils.UniqueCode
{
	public class DumpInternalMag : MonoBehaviour
	{
		public Handgun handgun;
		public H3VRUtilsMagRelease.TouchpadDirType presstoejectbutton;

		public void FixedUpdate()
		{
			if (!handgun.IsHeld) return;
			var hand = handgun.m_hand;
			var dir = H3VRUtilsMagRelease.TouchpadDirTypeToVector2(presstoejectbutton);
			if (Vector2.Angle(hand.Input.TouchpadAxes, dir) <= 45f
			    && hand.Input.TouchpadDown
			    && hand.Input.TouchpadAxes.magnitude > 0.2f)
			{
				if (handgun.Magazine.HasARound() && !handgun.Chamber.IsFull && !handgun.m_proxy.IsFull)
				{
					var go = handgun.Magazine.RemoveRound(false);
					handgun.m_proxy.SetFromPrefabReference(go);
					handgun.Chamber.SetRound(handgun.m_proxy.Round);
					handgun.EjectExtractedRound();
				}
			}
		}
	}
}