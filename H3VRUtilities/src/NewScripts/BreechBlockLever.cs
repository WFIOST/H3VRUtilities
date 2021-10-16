using UnityEngine;
using FistVR;

namespace H3VRUtils.FVRInteractiveObjects
{
	public class BreechBlockLever : FVRInteractiveObject
	{
		public float minRot;
		public float maxRot;
		//public bool isBraking;
		public bool reverseRot;
		public BreakOpenFlareGun flareGun;
		
		float rot;
		float rh;
		float lr;
		float inlerp;
		float lerp;
		float rotAmt;

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
			CalcRot(hand.transform);
			if(rotAmt == maxRot) flareGun.Unlatch();
			if (rotAmt == minRot)
			{
				flareGun.CockHammer();
				flareGun.Latch();
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
			}
			else if (rotAmt <= minRot) {
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
	}
}