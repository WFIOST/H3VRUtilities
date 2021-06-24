using System;
using UnityEngine;
using FistVR;

namespace H3VRUtils.Vehicles
{
	class SteeringWheel : FVRInteractiveObject
	{
		public Vehicle vehicle;
		public float resetLerpSpeed;
		[HideInInspector]
		public float rotAmt;

		public float maxRot;

		public float rot;
		public float rh;
		public float lr;
		public float inlerp;
		public float lerp;

		public bool isBraking;

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);

			//disconnect model
			var child = transform.GetChild(0);
			child.parent = null;

			//look at hands
			Vector3 lastrot = transform.localEulerAngles;
			transform.LookAt(hand.transform);
			transform.localEulerAngles = new Vector3(lastrot.x, transform.localEulerAngles.y, lastrot.z);

			//connect model
			child.parent = this.transform;
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			//undo BeginInteraction

			//dc model
			var child = transform.GetChild(0);
			child.parent = null;

			//zero rotation of wheel
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, 0, transform.localEulerAngles.z);

			//conncet model
			child.parent = this.transform;
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 lastrot = transform.localEulerAngles;
			transform.LookAt(hand.transform);
			Vector3 rothand = transform.localEulerAngles;

			//get diff between two angles
			rot = Mathf.DeltaAngle((float)Math.Round(rothand.y), (float)Math.Round(lastrot.y));
			rotAmt += rot;
			if (rotAmt >= maxRot)
			{
				rotAmt = maxRot;
				transform.localEulerAngles = lastrot;
			}
			else
			if (rotAmt <= -maxRot)
			{
				rotAmt = -maxRot;
				transform.localEulerAngles = lastrot;
			}
			else //if within bounds
			{
				transform.localEulerAngles = new Vector3(lastrot.x, rothand.y, lastrot.z);
			}
			rh = rothand.y;
			lr = lastrot.y;

			//float inlerp = 0;
			//float lerp = 0;
			if (rotAmt > 0)
			{
				inlerp = Mathf.InverseLerp(0, maxRot, rotAmt);
				lerp = -Mathf.Lerp(0, vehicle.maxRotation, inlerp);
			}
			else //if rotAmt is negative
			{
				inlerp = Mathf.InverseLerp(0, -maxRot, rotAmt);
				lerp = Mathf.Lerp(0, vehicle.maxRotation, inlerp);
			}

			//if (lerp < 2 && lerp > -2) lerp = 0;

			vehicle.setRotation(lerp);

			if(Vector2.Angle(hand.Input.TouchpadAxes, -Vector2.up) <= 45f && hand.Input.TouchpadDown && hand.Input.TouchpadAxes.magnitude > 0.3f){
				isBraking = !isBraking;
				SM.PlayGenericSound(vehicle.AudioSet.PedalSwitchSound, transform.position);
			}

			if (isBraking)
			{
				vehicle.setAcceleration(0);
				vehicle.setBraking(vehicle.maxDampBreak * hand.Input.TriggerFloat);
			}
			else
			{
				vehicle.setAcceleration(hand.Input.TriggerFloat * vehicle.maxAcceleration);
				vehicle.setBraking(0);
			}
		}

		void FixedUpdate()
		{
			if (base.m_hand == null)
			{
				var rLS = resetLerpSpeed;
				if (rotAmt > 0) rLS = -rLS;
				rotAmt += rLS;
				transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + rLS, transform.localEulerAngles.z);
			}
		}
	}
}
