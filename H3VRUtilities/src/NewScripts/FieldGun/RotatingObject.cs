﻿using System;
using FistVR;
using UnityEngine;

namespace H3VRUtils.FVRInteractiveObjects
{
	public class RotatingObject : FVRInteractiveObject
	{
		public float minRot;
		public float maxRot;
		//public bool isBraking;
		public bool reverseRot;
		//public BreakOpenFlareGun flareGun;
		public float resetLerpSpeed;
		
		float rot;
		float rh;
		float lr;
		float inlerp;
		public float lerp;
		float rotAmt;

		public override void BeginInteraction(FVRViveHand hand)
		{
			//this bit just sets the 0 point to look at the players hands so while the
			//object rotates correctly to the player's hands, the model does not.
			//REMEMBER: THE MODEL MUST BE A CHILD!
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
			//undoes BeginInteraction

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
			CalcRot(hand.transform);
			GetLerp();
			//if(rotAmt == maxRot) flareGun.Fire();
		}

		private void FixedUpdate()
		{
			if (m_hand == null)
			{
				ResetRot();
			}
		}

		private void CalcRot(Transform hand)
		{
			Vector3 lastrot = transform.localEulerAngles;
			transform.LookAt(hand);
			Vector3 rothand = transform.localEulerAngles;
			
			//get diff between two angles
			rot = Mathf.DeltaAngle(Mathf.Round(rothand.y), Mathf.Round(lastrot.y));
			rotAmt += rot;
			//clamp the rot. i know mathf.clamp exists; i dont care and neither do i trust it
			if (rotAmt >= maxRot) {
				rotAmt = maxRot;
				transform.localEulerAngles = lastrot;
			} else if (rotAmt <= minRot) {
				rotAmt = -maxRot;
				transform.localEulerAngles = lastrot;
			}
			else //if it does not need to be clamped
			{
				transform.localEulerAngles = new Vector3(lastrot.x, rothand.y, lastrot.z);
			}
			rh = rothand.y;
			lr = lastrot.y;
		}

		private void GetLerp()
		{
			bool isNeg = rotAmt <= 0;
			
			//what the fuck was i smoking here?
			lerp = Mathf.Abs(rotAmt) / maxRot;
			lerp *= -1;
			if (isNeg) lerp *= -1;
			
			if (reverseRot) lerp = -lerp;
		}

		private void ResetRot()
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
				}
			}
		}
	}
}