using System;
using UnityEngine;
using FistVR;
using UnityEngine.UI;

namespace H3VRUtils.Vehicles
{
	class SteeringWheel : FVRInteractiveObject
	{
		public VehicleControl vehicle;
		public float resetLerpSpeed;
		public float maxRot;
		//public bool isBraking;
		public bool reverseRot;
		
		[Header("Debug Values")]
		public Text rotText;
		public float rot;
		public float rh;
		public float lr;
		public float inlerp;
		public float lerp;
		public float rotAmt;

		public VehicleAudioSet audioSet;

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

			bool isBraking = Vector2.Angle(hand.Input.TouchpadAxes, -Vector2.up) <= 45f && hand.Input.TouchpadPressed && hand.Input.TouchpadAxes.magnitude > 0.3f;
			//check if switch breaking

			//handle acceleration
			var accelamt = (float)Math.Pow(hand.Input.TriggerFloat, 2);
			//if breaking, switch to negative acceleration
			if (isBraking)
			{
				vehicle.accel = -accelamt;
			}
			else
			{
				//set acceleration
				vehicle.accel = accelamt;
			}
		}

		void FixedUpdate()
		{
			if (m_hand == null)
			{
				var rLS = resetLerpSpeed;
				if (rotAmt > 0) rLS = -rLS;
				//prevent it from jigglign back n forth
				if (rotAmt > rLS || rotAmt < -rLS)
				{
					rotAmt += rLS;
					transform.localEulerAngles = new Vector3(transform.localEulerAngles.x,
						transform.localEulerAngles.y - rLS, transform.localEulerAngles.z);
					vehicle.accel = 0;
					SetRot();
				}
			}
		}

		void SetRot()
		{
			//TODO: remove the lerp calcs pretty sure they're useless now
			bool isNeg = rotAmt <= 0;

			lerp = Mathf.Abs(rotAmt) / maxRot;
			lerp *= -1;
			if (isNeg) lerp *= -1;

			if (reverseRot) lerp = -lerp;
			if(rotText != null) rotText.text = lerp.ToString();
			vehicle.steer = lerp;
		}
	}
}
