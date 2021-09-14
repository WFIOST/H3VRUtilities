using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using UnityEngine.UI;

namespace H3VRUtils.Vehicles
{
	class VehicleSeat : MonoBehaviour
	{
		public FVRViveHand hand;
		public GameObject SitPos;
		public GameObject EjectPos;
		public void Update()
		{
			if (hand != null)
			{
				//this is NOT a good way to do it, pls find an alternative soon lol
				hand.MovementManager.transform.position = SitPos.transform.position;
				//hand.MovementManager.TeleportToPoint(SitPos.transform.position, false);
				//hand.MovementManager.DelayGround(Time.fixedDeltaTime * 1.15f);
				
				

				//rotation locks
				var rot = hand.MovementManager.transform.rotation;
				if (UtilsBepInExLoader.VehicleLockXRot.Value) rot.x = SitPos.transform.rotation.x;
				if (UtilsBepInExLoader.VehicleLockYRot.Value) rot.y = SitPos.transform.rotation.y;
				if (UtilsBepInExLoader.VehicleLockZRot.Value) rot.z = SitPos.transform.rotation.z;
				hand.MovementManager.transform.rotation = rot;

				//kick player if dead
				if(GM.CurrentPlayerBody.GetPlayerHealth() <= 0)
				{
					hand = null;
				}
			}
		}
	}
}
