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
		public Vector3 rotUpwards;
		public Vector3 rotRegular;
		public Vector3 rotDownwards;

		public GameObject lift;
		public float liftSpeed;
		public float minLiftY;
		public float maxLiftY;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 pos = lift.transform.position;
			transform.localEulerAngles = rotRegular;
			if (Vector2.Angle(hand.Input.TouchpadAxes, Vector2.up) <= 45f && hand.Input.TouchpadPressed && hand.Input.TouchpadAxes.magnitude > 0.2f)
			{
				pos.y += liftSpeed / 50;
				transform.localEulerAngles = rotUpwards;
			}
			
			if (Vector2.Angle(hand.Input.TouchpadAxes, Vector2.down) <= 45f && hand.Input.TouchpadPressed && hand.Input.TouchpadAxes.magnitude > 0.2f)
			{
				pos.y -= liftSpeed / 50;
				transform.localEulerAngles = rotDownwards;
			}

			if (pos.y > maxLiftY)
			{
				pos.y = maxLiftY;
			}
			else if (pos.y < minLiftY)
			{
				pos.y = minLiftY;
			}

			lift.transform.position = pos;
		}
	}
}
