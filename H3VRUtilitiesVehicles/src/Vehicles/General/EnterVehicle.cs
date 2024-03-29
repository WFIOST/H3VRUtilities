﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;

namespace H3VRUtils.Vehicles
{
	class EnterVehicle : FVRInteractiveObject
	{
		public VehicleSeat vehicleSeat;

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			if (hand.IsThisTheRightHand) hand = hand.OtherHand;
			if (vehicleSeat.hand == null)
			{
				//kick hand's old spot if still there
				if (VehicleSeat.currentSeat.ContainsKey(hand)) { VehicleSeat.currentSeat[hand].RemoveHand(); }
				VehicleSeat.currentSeat.Add(hand, vehicleSeat);
				//only sit at one loc!
				vehicleSeat.hand = hand;
				hand.MovementManager.TeleportToPoint(vehicleSeat.SitPos.transform.position, false, vehicleSeat.SitPos.transform.localEulerAngles);
				
				//var rot = hand.Head.transform.rotation;
				
				//rot.y = vehicleSeat.SitPos.transform.rotation.y;
				//hand.Head.transform.rotation = rot;
			}
			else
			{
				//so someone can't just eject someone else
				if (hand == vehicleSeat.hand)
				{
					vehicleSeat.RemoveHand();
					if (vehicleSeat.EjectPos != null)
						hand.MovementManager.transform.position = vehicleSeat.EjectPos.transform.position;
				}
			}
		}
	}
}
