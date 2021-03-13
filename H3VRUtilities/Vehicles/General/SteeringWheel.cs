using System;
using UnityEngine;
using FistVR;

namespace H3VRUtils.Vehicles
{
	public class Lever : FVRInteractiveObject
	{
		protected override void Awake()
		{
			base.Awake();
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			Vector3 vector = hand.transform.position - this.LeverTransform.position;
			Vector3 to = Vector3.ProjectOnPlane(vector, this.LeverTransform.right);
			if (Vector3.Dot(to.normalized, this.Base.up) > 0f)
			{
				this.m_curRot = -Vector3.Angle(this.Base.forward, to);
			}
			else
			{
				this.m_curRot = Vector3.Angle(this.Base.forward, to);
			}
		}

		public float GetLeverValue()
		{
			return Mathf.InverseLerp(this.minValue, this.maxValue, this.m_curRot);
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			this.m_curRot = Mathf.Clamp(this.m_curRot, minValue, maxValue);
			this.LeverTransform.localEulerAngles = new Vector3(this.m_curRot, 0f, 0f);
		}

		public Transform LeverTransform;
		public Transform Base;
		public float m_curRot = -22.5f;
		public float minValue = -22.5f;
		public float maxValue = 22.5f;

		public enum LeverState
		{
			Off,
			Mid,
			On
		}
	}
}
