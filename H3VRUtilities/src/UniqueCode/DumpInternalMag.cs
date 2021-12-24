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
			    && hand.Input.TouchpadPressed
			    && hand.Input.TouchpadAxes.magnitude > 0.2f
			    && (handgun.Slide.CurPos == HandgunSlide.SlidePos.LockedToRear || handgun.Slide.CurPos == HandgunSlide.SlidePos.Rear))
			{
				if (handgun.Chamber.IsFull) handgun.EjectExtractedRound(); //insert chamber into the woorld
				else if (handgun.m_proxy.IsFull) handgun.ChamberRound(); //insert proxy into chamber
				else if (handgun.Magazine.HasARound()) { //insert mag round into proxy
					var go = handgun.Magazine.RemoveRound(false);
					handgun.PlayAudioEvent(FirearmAudioEventType.MagazineEjectRound);
					handgun.m_proxy.SetFromPrefabReference(go);
				}
			}
		}
	}
}