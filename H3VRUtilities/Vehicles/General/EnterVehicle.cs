using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;

namespace H3VRUtils.Vehicles
{
	class EnterVehicle : FVRInteractiveObject
	{
		public Vehicle vehicle;

		public override void SimpleInteraction(FVRViveHand hand)
		{
			if (hand.IsThisTheRightHand)
			{
				hand = hand.OtherHand;
			}
			base.SimpleInteraction(hand);
			if (vehicle.hand == null)
			{
				vehicle.hand = hand;
			}
			else
			{
				vehicle.hand = null;
			}
		}
	}
}
