using System;
using UnityEngine;
using FistVR;
using UnityEngine.Serialization;

namespace FistVR
{
	public class AR15HandleFlipperSounds : FVRInteractiveObject
	{
		public override void Awake()
		{
			base.Awake();
		}

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			this._mIsLargeAperture = !this._mIsLargeAperture;
			try
			{
				if (!_mIsLargeAperture) SM.PlayGenericSound(audClipOpen, transform.position);
				else SM.PlayGenericSound(audClipClose, transform.position);
			}
			catch
			{
				Console.WriteLine(this.name + " failed to play sound!");
			}
		}

		public override void FVRUpdate()
		{
			base.FVRUpdate();
			if (this._mIsLargeAperture)
			{
				this._mTarFlipLerp = 0f;
			}
			else
			{
				this._mTarFlipLerp = 1f;
			}
			this._mCurFlipLerp = Mathf.MoveTowards(this._mCurFlipLerp, this._mTarFlipLerp, Time.deltaTime * 4f);
			if (Mathf.Abs(this._mCurFlipLerp - this._mLastFlipLerp) > 0.01f)
			{
				this._mFlipsightCurRotX = Mathf.Lerp(this.mFlipsightStartRotX, this.mFlipsightEndRotX, this._mCurFlipLerp);
				AR15HandleSightFlipper.Axis rotAxis = this.rotAxis;
				if (rotAxis != AR15HandleSightFlipper.Axis.X)
				{
					if (rotAxis != AR15HandleSightFlipper.Axis.Y)
					{
						if (rotAxis == AR15HandleSightFlipper.Axis.Z)
						{
							this.flipsight.localEulerAngles = new Vector3(0f, 0f, this._mFlipsightCurRotX);
						}
					}
					else
					{
						this.flipsight.localEulerAngles = new Vector3(0f, this._mFlipsightCurRotX, 0f);
					}
				}
				else
				{
					this.flipsight.localEulerAngles = new Vector3(this._mFlipsightCurRotX, 0f, 0f);
				}
			}
			this._mLastFlipLerp = this._mCurFlipLerp;
		}

		private bool _mIsLargeAperture = true;
		[FormerlySerializedAs("Flipsight")] public Transform flipsight;
		[FormerlySerializedAs("m_flipsightStartRotX")] public float mFlipsightStartRotX;
		[FormerlySerializedAs("m_flipsightEndRotX")] public float mFlipsightEndRotX = -90f;
		private float _mFlipsightCurRotX;
		[FormerlySerializedAs("RotAxis")] public AR15HandleSightFlipper.Axis rotAxis;
		private float _mCurFlipLerp;
		private float _mTarFlipLerp;
		private float _mLastFlipLerp;
		[FormerlySerializedAs("AudClipOpen")] public AudioEvent audClipOpen;
		[FormerlySerializedAs("AudClipClose")] public AudioEvent audClipClose;
		public enum Axis
		{
			X,
			Y,
			Z
		}
	}
}
