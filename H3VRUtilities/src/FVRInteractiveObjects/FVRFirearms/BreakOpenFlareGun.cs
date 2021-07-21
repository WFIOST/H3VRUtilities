using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;
using UnityEngine.Serialization;

namespace H3VRUtils
{
	public class BreakOpenFlareGun : FVRFireArm
	{
		// Token: 0x06003434 RID: 13364 RVA: 0x0016D073 File Offset: 0x0016B473
		public override void Awake()
		{
			base.Awake();
			if (this.canUnlatch)
			{
				this.chamber.IsAccessible = false;
			}
			else
			{
				this.chamber.IsAccessible = true;
			}
		}

		// Token: 0x06003435 RID: 13365 RVA: 0x0016D0A4 File Offset: 0x0016B4A4
		public override void FVRUpdate()
		{
			if (this.hasVisibleHammer)
			{
				if (this._mIsHammerCocked)
				{
					this._mHammerXRot = Mathf.Lerp(this._mHammerXRot, this.hammerMaxRot, Time.deltaTime * 12f);
				}
				else
				{
					this._mHammerXRot = Mathf.Lerp(this._mHammerXRot, 0f, Time.deltaTime * 25f);
				}
				this.hammer.localEulerAngles = new Vector3(this._mHammerXRot, 0f, 0f);
			}
			if (!this._mIsLatched && Vector3.Angle(Vector3.up, this.chamber.transform.forward) < 70f && this.chamber.IsFull && this.chamber.IsSpent)
			{
				base.PlayAudioEvent(FirearmAudioEventType.MagazineEjectRound, 1f);
				this.chamber.EjectRound(this.chamber.transform.position + this.chamber.transform.forward * -0.06f, this.chamber.transform.forward * -0.01f, Vector3.right, false);
			}
		}

		// Token: 0x06003436 RID: 13366 RVA: 0x0016D1E6 File Offset: 0x0016B5E6
		private void ToggleLatchState()
		{
			if (this._mIsLatched)
			{
				this.Unlatch();
			}
			else if (!this._mIsLatched)
			{
				this.Latch();
			}
		}

		// Token: 0x06003437 RID: 13367 RVA: 0x0016D210 File Offset: 0x0016B610
		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			float num = 0f;
			FVRPhysicalObject.Axis hingeAxis = this.hingeAxis;
			if (hingeAxis != FVRPhysicalObject.Axis.X)
			{
				if (hingeAxis != FVRPhysicalObject.Axis.Y)
				{
					if (hingeAxis == FVRPhysicalObject.Axis.Z)
					{
						num = base.transform.InverseTransformDirection(hand.Input.VelAngularWorld).z;
					}
				}
				else
				{
					num = base.transform.InverseTransformDirection(hand.Input.VelAngularWorld).y;
				}
			}
			else
			{
				num = base.transform.InverseTransformDirection(hand.Input.VelAngularWorld).x;
			}
			if (num > 15f && this.canUnlatch)
			{
				this.Unlatch();
			}
			else if (num < -15f && this.canUnlatch)
			{
				this.Latch();
			}
			if (hand.Input.TouchpadDown && !this.IsAltHeld)
			{
				Vector2 touchpadAxes = hand.Input.TouchpadAxes;
				if (touchpadAxes.magnitude > 0.2f && Vector2.Angle(touchpadAxes, Vector2.down) < 45f && this.canCockHammer)
				{
					this.CockHammer();
				}
				else if (touchpadAxes.magnitude > 0.2f && (Vector2.Angle(touchpadAxes, Vector2.left) < 45f || Vector2.Angle(touchpadAxes, Vector2.right) < 45f) && this.canUnlatch)
				{
					this.ToggleLatchState();
				}
			}
			if (this._mIsDestroyed)
			{
				return;
			}
			if (this.m_hasTriggeredUpSinceBegin && !this.IsAltHeld)
			{
				this._triggerFloat = hand.Input.TriggerFloat;
			}
			else
			{
				this._triggerFloat = 0f;
			}
			float x = Mathf.Lerp(this.triggerForwardBackRots.x, this.triggerForwardBackRots.y, this._triggerFloat);
			this.trigger.localEulerAngles = new Vector3(x, 0f, 0f);
			if (this._triggerFloat > 0.7f)
			{
				if (this._mIsTriggerReset && this._mIsHammerCocked)
				{
					this._mIsTriggerReset = false;
					this._mIsHammerCocked = false;
					if (this.hammer != null)
					{
						base.SetAnimatedComponent(this.hammer, this.hammerMinRot, this.hammerInterp, this.hammerAxis);
					}
					base.PlayAudioEvent(FirearmAudioEventType.HammerHit, 1f);
					this.Fire();
				}
			}
			else if (this._triggerFloat < 0.2f && !this._mIsTriggerReset)
			{
				this._mIsTriggerReset = true;
			}
		}

		private void Fire()
		{
			if (!this._mIsLatched)
			{
				return;
			}
			if (!this.chamber.Fire())
			{
				return;
			}
			base.Fire(this.chamber, this.GetMuzzle(), true, 1f);
			this.FireMuzzleSmoke();
			bool twoHandStabilized = this.IsTwoHandStabilized();
			bool foregripStabilized = base.AltGrip != null;
			bool shoulderStabilized = this.IsShoulderStabilized();
			if (this.chamber.GetRound().IsHighPressure && !this.isHighPressureTolerant)
			{
				this.Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized, null, 1f);
				this.Destroy();
			}
			else if (this.isHighPressureTolerant)
			{
				this.Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized, null, 1f);
			}
			base.PlayAudioGunShot(this.chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment(), 1f);
			if (GM.CurrentSceneSettings.IsAmmoInfinite || GM.CurrentPlayerBody.IsInfiniteAmmo)
			{
				this.chamber.IsSpent = false;
				this.chamber.UpdateProxyDisplay();
			}
			else if (this.chamber.GetRound().IsCaseless)
			{
				this.chamber.SetRound(null);
			}
			if (this.deletesCartridgeOnFire)
			{
				this.chamber.SetRound(null);
			}
		}

		public void Unlatch()
		{
			if (this._mIsLatched)
			{
				base.PlayAudioEvent(FirearmAudioEventType.BreachOpen, 1f);
				this._mIsLatched = false;
				this.chamber.IsAccessible = true;
				if (this.cocksOnOpen)
				{
					this.CockHammer();
				}
			}
		}

		public void Latch()
		{
			if (!this._mIsLatched)
			{
				base.PlayAudioEvent(FirearmAudioEventType.BreachClose, 1f);
				this._mIsLatched = true;
				this.chamber.IsAccessible = false;
			}
		}

		private void CockHammer()
		{
			if (!this._mIsHammerCocked)
			{
				base.PlayAudioEvent(FirearmAudioEventType.Prefire, 1f);
				this._mIsHammerCocked = true;
				if (this.hammer != null)
				{
					base.SetAnimatedComponent(this.hammer, this.hammerMaxRot, this.hammerInterp, this.hammerAxis);
				}
			}
		}

		private void Destroy()
		{
			if (!this._mIsDestroyed)
			{
				this._mIsDestroyed = true;
				this.destroyPSystem.Emit(25);
				for (int i = 0; i < this.gunUndamaged.Length; i++)
				{
					this.gunUndamaged[i].enabled = false;
					this.gunDamaged[i].enabled = true;
				}
			}
		}

		public override List<FireArmRoundClass> GetChamberRoundList()
		{
			if (this.chamber.IsFull && !this.chamber.IsSpent)
			{
				return new List<FireArmRoundClass>
				{
					this.chamber.GetRound().RoundClass
				};
			}
			return null;
		}

		public override void SetLoadedChambers(List<FireArmRoundClass> rounds)
		{
			if (rounds.Count > 0)
			{
				this.chamber.Autochamber(rounds[0]);
			}
		}

		public override List<string> GetFlagList()
		{
			return null;
		}

		public override void SetFromFlagList(List<string> flags)
		{
		}

		[FormerlySerializedAs("GunUndamaged")] [Header("Flaregun Params")]
		public Renderer[] gunUndamaged;

		[FormerlySerializedAs("GunDamaged")] public Renderer[] gunDamaged;

		[FormerlySerializedAs("Chamber")] public FVRFireArmChamber chamber;

		[FormerlySerializedAs("HingeAxis")] public FVRPhysicalObject.Axis hingeAxis;

		[FormerlySerializedAs("Hinge")] public Transform hinge;

		[FormerlySerializedAs("RotOut")] public float rotOut = 35f;

		[FormerlySerializedAs("CanUnlatch")] public bool canUnlatch = true;

		[FormerlySerializedAs("IsHighPressureTolerant")] public bool isHighPressureTolerant;

		private bool _mIsHammerCocked;

		private bool _mIsTriggerReset = true;

		private bool _mIsLatched = true;

		private bool _mIsDestroyed;

		private float _triggerFloat;

		[FormerlySerializedAs("Hammer")] public Transform hammer;

		[FormerlySerializedAs("HasVisibleHammer")] public bool hasVisibleHammer = true;

		[FormerlySerializedAs("CanCockHammer")] public bool canCockHammer = true;

		[FormerlySerializedAs("CocksOnOpen")] public bool cocksOnOpen;

		private float _mHammerXRot;

		[FormerlySerializedAs("HammerAxis")] public FVRPhysicalObject.Axis hammerAxis;

		[FormerlySerializedAs("HammerInterp")] public FVRPhysicalObject.InterpStyle hammerInterp = FVRPhysicalObject.InterpStyle.Rotation;

		[FormerlySerializedAs("HammerMinRot")] public float hammerMinRot;

		[FormerlySerializedAs("HammerMaxRot")] public float hammerMaxRot = -70f;

		[FormerlySerializedAs("Trigger")] public Transform trigger;

		[FormerlySerializedAs("TriggerForwardBackRots")] public Vector2 triggerForwardBackRots;

		[FormerlySerializedAs("Muzzle")] public Transform muzzle;

		[FormerlySerializedAs("SmokePSystem")] public ParticleSystem smokePSystem;

		[FormerlySerializedAs("DestroyPSystem")] public ParticleSystem destroyPSystem;

		[FormerlySerializedAs("DeletesCartridgeOnFire")] public bool deletesCartridgeOnFire;
	}
}
