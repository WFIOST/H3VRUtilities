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
				hand.MovementManager.transform.position = SitPos.transform.position;
				//my fucking head hurts when you do this dont do it
				//hand.MovementManager.transform.rotation = SitPos.transform.rotation;
			}
		}
	}
}
