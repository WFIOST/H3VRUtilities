using System;
using UnityEngine;
using FistVR;

namespace H3VRUtils.Vehicles
{
	class SteeringWheel : FVRInteractiveObject
	{
		public Vehicle vehicle;
		public float resetLerpSpeed;
		public float lerp;
		public float inlerp;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			transform.LookAt(hand.transform);
			float clamp = transform.localEulerAngles.y;
			transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);


			if (transform.localEulerAngles.y < 180f)
			{
				inlerp = Mathf.InverseLerp(0, 180, transform.localEulerAngles.y);
				lerp = Mathf.Lerp(0, vehicle.maxRotation, inlerp);
				vehicle.setRotation(lerp);
			}
			else
			{
				inlerp = Mathf.InverseLerp(360, 180, transform.localEulerAngles.y);
				lerp = -Mathf.Lerp(0, vehicle.maxRotation, inlerp);
				vehicle.setRotation(lerp);
			}
			if (transform.localEulerAngles.y < 5 && transform.localEulerAngles.y > 355)
			{
				vehicle.setRotation(0);
			}

			vehicle.setAcceleration(hand.Input.TriggerFloat * vehicle.maxAcceleration);
		}

		void FixedUpdate()
		{
			if (base.m_hand == null)
			{
				if (transform.localEulerAngles.y < 180)
				{
					transform.localEulerAngles = new Vector3(0, Mathf.Lerp(1, transform.localEulerAngles.y, resetLerpSpeed), 0);
				}
				else
				{
					transform.localEulerAngles = new Vector3(0, Mathf.Lerp(359, transform.localEulerAngles.y, resetLerpSpeed), 0);
				}

			}
		}
	}
}
