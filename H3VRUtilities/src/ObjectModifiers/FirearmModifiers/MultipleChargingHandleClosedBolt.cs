using FistVR;
using UnityEngine;

namespace H3VRUtils.FVRInteractiveObjects
{
	public class MultipleChargingHandleClosedBolt : ClosedBoltHandle
	{
		public override void BeginInteraction(FVRViveHand hand)
		{
			if (Weapon.Handle.m_hand != null)
			{
				base.ForceBreakInteraction();
				return;
			}
			Weapon.Handle = this;
			base.BeginInteraction(hand);
		}
	}
}