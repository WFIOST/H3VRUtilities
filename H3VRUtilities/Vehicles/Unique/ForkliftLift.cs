using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using FistVR;

namespace H3VRUtils.Vehicles
{
	class ForkliftLift : FVRInteractiveObject
	{
		public float minRot;
		public float defRot;
		public float maxRot;

		public GameObject lift;
		public float liftSpeed;
		public float minLiftY;
		public float maxLiftY;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			transform.LookAt(hand.transform);
			var rot = transform.localEulerAngles.x;
			if (rot > maxRot) rot = maxRot;
			if (rot < minRot) rot = minRot;

			transform.localEulerAngles = new Vector3(rot, 0, 0);
			var pos = lift.transform.localPosition;
			if (rot > defRot)
			{
				pos.y += liftSpeed;
			}
			else
			{
				pos.y -= liftSpeed;
			}

			if (pos.y > maxLiftY)
			{
				pos.y = maxLiftY;
			}
			else if (pos.y < minLiftY)
			{
				pos.y = minLiftY;
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			transform.localEulerAngles = new Vector3(defRot, 0, 0);
		}
	}
}
