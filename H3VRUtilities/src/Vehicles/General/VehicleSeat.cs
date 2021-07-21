using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace H3VRUtils.Vehicles
{
	public class VehicleSeat : MonoBehaviour
	{
		public FVRViveHand hand;
		[FormerlySerializedAs("SitPos")] public GameObject sitPos;
		[FormerlySerializedAs("EjectPos")] public GameObject ejectPos;
		public void Update()
		{
			if (hand != null)
			{
				//this is NOT a good way to do it, pls find an alternative soon lol
				hand.MovementManager.transform.position = sitPos.transform.position;
				//hand.MovementManager.TeleportToPoint(SitPos.transform.position, false);

				//rotation locks
				Quaternion rot = hand.MovementManager.transform.rotation;
				if (UtilsBepInExLoader.VehicleLockXRot.Value) rot.x = sitPos.transform.rotation.x;
				if (UtilsBepInExLoader.VehicleLockYRot.Value) rot.y = sitPos.transform.rotation.y;
				if (UtilsBepInExLoader.VehicleLockZRot.Value) rot.z = sitPos.transform.rotation.z;
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
