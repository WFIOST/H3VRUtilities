using System;
using UnityEngine;
using FistVR;

namespace H3VRUtils.Vehicles
{
	class SteeringWheel : FVRInteractiveObject
	{
		public Vehicle vehicle;
		public float resetLerpSpeed;
		public float maxRot;
		public bool isBraking;
		public bool reverseRot;
		
		[Header("Debug Values")]
		public float rot;
		public float rh;
		public float lr;
		public float inlerp;
		public float lerp;
		public float rotAmt;

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
			//clamp the rot. i know mathf.clamp exists; i dont care and neither do i trust it
			if (rotAmt >= maxRot) {
				rotAmt = maxRot;
				transform.localEulerAngles = lastrot;
			}
			else if (rotAmt <= -maxRot) {
				rotAmt = -maxRot;
				transform.localEulerAngles = lastrot;
			}
			else //if it does not need to be clamped
			{
				transform.localEulerAngles = new Vector3(lastrot.x, rothand.y, lastrot.z);
			}
			rh = rothand.y;
			lr = lastrot.y;
			
			SetRot();
			
			//check if switch breaking
			if(Vector2.Angle(hand.Input.TouchpadAxes, -Vector2.up) <= 45f && hand.Input.TouchpadDown && hand.Input.TouchpadAxes.magnitude > 0.3f){
				isBraking = !isBraking;
				SM.PlayGenericSound(vehicle.AudioSet.PedalSwitchSound, transform.position);
			}
			
			//handle acceleration
			var accelamt = hand.Input.TriggerFloat;
			//if breaking, switch to negative acceleration
			if (isBraking)
			{
				vehicle.setBraking(accelamt);
			}
			else
			{
				//set acceleration
				vehicle.setThrottle(accelamt);
			}
		}

		void FixedUpdate()
		{
			if (m_hand != null)
			{
				var rLS = resetLerpSpeed;
				if (rotAmt > 0) rLS = -rLS;
				//prevent it from jigglign back n forth
				if (rotAmt > rLS || rotAmt < -rLS)
				{
					rotAmt += rLS;
					transform.localEulerAngles = new Vector3(transform.localEulerAngles.x,
						transform.localEulerAngles.y + rLS, transform.localEulerAngles.z);
					vehicle.setThrottle(0);
					vehicle.setBraking(0);
					SetRot();
				}
			}
		}

		void SetRot()
		{
			//TODO: remove the lerp calcs pretty sure they're useless now
			if (rotAmt > 0)
			{
				inlerp = Mathf.InverseLerp(0, maxRot, rotAmt);
				lerp = -Mathf.Lerp(0, 1, inlerp);
			}
			else //if rotAmt is negative
			{
				inlerp = Mathf.InverseLerp(0, -maxRot, rotAmt);
				lerp = Mathf.Lerp(0, 1, inlerp);
			}

			if (reverseRot) lerp = -lerp;

			vehicle.setRotation(lerp);
		}
	}
}
