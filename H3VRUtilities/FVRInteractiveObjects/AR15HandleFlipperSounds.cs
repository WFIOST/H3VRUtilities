using System;
using UnityEngine;
using FistVR;

namespace FistVR
{
	public class AR15HandleFlipperSounds : FVRInteractiveObject
	{
		protected override void Awake()
		{
			base.Awake();
		}

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			this.m_isLargeAperture = !this.m_isLargeAperture;
			try
			{
				if (!m_isLargeAperture) SM.PlayGenericSound(AudClipOpen, transform.position);
				else SM.PlayGenericSound(AudClipClose, transform.position);
			}
			catch
			{
				Console.WriteLine(this.name + " failed to play sound!");
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (this.m_isLargeAperture)
			{
				this.m_tarFlipLerp = 0f;
			}
			else
			{
				this.m_tarFlipLerp = 1f;
			}
			this.m_curFlipLerp = Mathf.MoveTowards(this.m_curFlipLerp, this.m_tarFlipLerp, Time.deltaTime * 4f);
			if (Mathf.Abs(this.m_curFlipLerp - this.m_lastFlipLerp) > 0.01f)
			{
				this.m_flipsightCurRotX = Mathf.Lerp(this.m_flipsightStartRotX, this.m_flipsightEndRotX, this.m_curFlipLerp);
				AR15HandleSightFlipper.Axis rotAxis = this.RotAxis;
				if (rotAxis != AR15HandleSightFlipper.Axis.X)
				{
					if (rotAxis != AR15HandleSightFlipper.Axis.Y)
					{
						if (rotAxis == AR15HandleSightFlipper.Axis.Z)
						{
							this.Flipsight.localEulerAngles = new Vector3(0f, 0f, this.m_flipsightCurRotX);
						}
					}
					else
					{
						this.Flipsight.localEulerAngles = new Vector3(0f, this.m_flipsightCurRotX, 0f);
					}
				}
				else
				{
					this.Flipsight.localEulerAngles = new Vector3(this.m_flipsightCurRotX, 0f, 0f);
				}
			}
			this.m_lastFlipLerp = this.m_curFlipLerp;
		}

		private bool m_isLargeAperture = true;
		public Transform Flipsight;
		public float m_flipsightStartRotX;
		public float m_flipsightEndRotX = -90f;
		private float m_flipsightCurRotX;
		public AR15HandleSightFlipper.Axis RotAxis;
		private float m_curFlipLerp;
		private float m_tarFlipLerp;
		private float m_lastFlipLerp;
		public AudioEvent AudClipOpen;
		public AudioEvent AudClipClose;
		public enum Axis
		{
			X,
			Y,
			Z
		}
	}
}
