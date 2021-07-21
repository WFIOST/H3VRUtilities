using System;
using UnityEngine;
using FistVR;

namespace H3VRUtils.Vehicles
{
	public class SteeringWheel : FVRInteractiveObject
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
		public bool reverseRot;

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);

			//disconnect model
			Transform child = transform.GetChild(0);
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
			Transform child = transform.GetChild(0);
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

			SetRot();

			if(Vector2.Angle(hand.Input.TouchpadAxes, -Vector2.up) <= 45f && hand.Input.TouchpadDown && hand.Input.TouchpadAxes.magnitude > 0.3f){
				isBraking = !isBraking;
				SM.PlayGenericSound(vehicle.audioSet.pedalSwitchSound, transform.position);
			}

			if (isBraking)
			{
				vehicle.SetAcceleration(0);
				vehicle.SetBraking(vehicle.maxDampBreak * hand.Input.TriggerFloat);
			}
			else
			{
				vehicle.SetAcceleration(hand.Input.TriggerFloat * vehicle.maxAcceleration);
				vehicle.SetBraking(0);
			}
		}

		void FixedUpdate()
		{
			if (base.m_hand == null)
			{
				float rLs = resetLerpSpeed;
				if (rotAmt > 0) rLs = -rLs;
				rotAmt += rLs;
				transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + rLs, transform.localEulerAngles.z);
				vehicle.SetAcceleration(0);
				vehicle.SetBraking(0);
				SetRot();
			}
		}

		void SetRot()
		{
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

			if (reverseRot) lerp = -lerp;

			vehicle.SetRotation(lerp);
		}
	}
}
