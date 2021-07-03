using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace H3VRUtils.UniqueCode
{
	class RevolverChargingHandle : FVRInteractiveObject
	{
		public SingleActionRevolver wep;
		public Transform frontMostPoint;
		public Transform rearMostPoint;
		public float forwardspeed;

		private bool pulled;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 closestValidPoint = base.GetClosestValidPoint(this.frontMostPoint.position, this.rearMostPoint.position, this.m_hand.Input.Pos);
			base.transform.position = closestValidPoint;
			float m_currentHandleZ = transform.localPosition.z;
			float l = Mathf.InverseLerp(frontMostPoint.localPosition.z, rearMostPoint.localPosition.z, transform.localPosition.z);
		}

		public override void FVRUpdate()
		{
			base.FVRUpdate();
			if (!base.IsHeld && Mathf.Abs(transform.localPosition.z - frontMostPoint.localPosition.z) > 0.001f)
			{
				base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, Mathf.MoveTowards(transform.localPosition.z, frontMostPoint.localPosition.z, Time.deltaTime * forwardspeed));
			}
			else if (Mathf.Abs(transform.localPosition.z - rearMostPoint.localPosition.z) < 0.01f)
			{
				try { wep.PlayAudioEvent(FirearmAudioEventType.HandleBack); } catch { Console.WriteLine("Forgot to assign FireArmAudioEventType.HandleBack!"); }
				if (!pulled)
				{
					FVRViveHand fakehand = new FVRViveHand();
					fakehand.IsInStreamlinedMode = true;
					fakehand.Input.AXButtonDown = true;
					wep.UpdateInteraction(fakehand);
					pulled = true;
				}
			}
			else if (Mathf.Abs(transform.localPosition.z - frontMostPoint.localPosition.z) < 0.01f)
			{
				try { wep.PlayAudioEvent(FirearmAudioEventType.HandleForward); } catch { Console.WriteLine("Forgot to assign FireArmAudioEventType.HandleForward!"); }
				pulled = false;
			}
		}
	}
}
