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
		public static Dictionary<FVRViveHand, VehicleSeat> currentSeat = new Dictionary<FVRViveHand, VehicleSeat>();
		public FVRViveHand hand;
		public GameObject SitPos;
		public GameObject EjectPos;
		public void Update()
		{
			if (hand != null)
			{
				//player possibly got tele'd.
				if(Vector3.Distance(hand.MovementManager.transform.position, transform.position) > 25f) RemoveHand();
				//this is NOT a good way to do it, pls find an alternative soon lol
				hand.MovementManager.transform.position = SitPos.transform.position;
				//hand.MovementManager.TeleportToPoint(SitPos.transform.position, false);
				//hand.MovementManager.DelayGround(Time.fixedDeltaTime * 1.15f);
				
				//rotation locks
				var rot = hand.MovementManager.transform.eulerAngles;
				if (UtilsBepInExLoader.VehicleLockXRot.Value) rot.x = SitPos.transform.rotation.x;
				if (UtilsBepInExLoader.VehicleLockYRot.Value) rot.y = SitPos.transform.rotation.y;
				if (UtilsBepInExLoader.VehicleLockZRot.Value) rot.z = SitPos.transform.rotation.z;
				hand.MovementManager.transform.eulerAngles = Vector3.Lerp(hand.MovementManager.transform.eulerAngles, rot, 0.2f * Time.deltaTime);
				
				//kick player if dead
				if(GM.CurrentPlayerBody.GetPlayerHealth() <= 0) RemoveHand();
				//kick player if below kick height
				if (hand.MovementManager.transform.position.y < GM.CurrentSceneSettings.CatchHeight) RemoveHand();
			}
		}

		public void RemoveHand()
		{
			if (currentSeat.ContainsKey(hand)) currentSeat.Remove(hand);
			hand = null;
		}
	}
}
