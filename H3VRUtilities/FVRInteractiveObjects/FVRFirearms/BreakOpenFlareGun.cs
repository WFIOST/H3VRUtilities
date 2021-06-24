using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;

namespace H3VRUtils
{
	public class BreakOpenFlareGun : FVRFireArm
	{
		// Token: 0x06003434 RID: 13364 RVA: 0x0016D073 File Offset: 0x0016B473
		protected void Awake()
		{
			base.Awake();
			if (this.CanUnlatch)
			{
				this.Chamber.IsAccessible = false;
			}
			else
			{
				this.Chamber.IsAccessible = true;
			}
		}

		// Token: 0x06003435 RID: 13365 RVA: 0x0016D0A4 File Offset: 0x0016B4A4
		protected void FVRUpdate()
		{
			if (this.HasVisibleHammer)
			{
				if (this.m_isHammerCocked)
				{
					this.m_hammerXRot = Mathf.Lerp(this.m_hammerXRot, this.HammerMaxRot, Time.deltaTime * 12f);
				}
				else
				{
					this.m_hammerXRot = Mathf.Lerp(this.m_hammerXRot, 0f, Time.deltaTime * 25f);
				}
				this.Hammer.localEulerAngles = new Vector3(this.m_hammerXRot, 0f, 0f);
			}
			if (!this.m_isLatched && Vector3.Angle(Vector3.up, this.Chamber.transform.forward) < 70f && this.Chamber.IsFull && this.Chamber.IsSpent)
			{
				base.PlayAudioEvent(FirearmAudioEventType.MagazineEjectRound, 1f);
				this.Chamber.EjectRound(this.Chamber.transform.position + this.Chamber.transform.forward * -0.06f, this.Chamber.transform.forward * -0.01f, Vector3.right, false);
			}
		}

		// Token: 0x06003436 RID: 13366 RVA: 0x0016D1E6 File Offset: 0x0016B5E6
		private void ToggleLatchState()
		{
			if (this.m_isLatched)
			{
				this.Unlatch();
			}
			else if (!this.m_isLatched)
			{
				this.Latch();
			}
		}

		// Token: 0x06003437 RID: 13367 RVA: 0x0016D210 File Offset: 0x0016B610
		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			float num = 0f;
			FVRPhysicalObject.Axis hingeAxis = this.HingeAxis;
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
			if (num > 15f && this.CanUnlatch)
			{
				this.Unlatch();
			}
			else if (num < -15f && this.CanUnlatch)
			{
				this.Latch();
			}
			if (hand.Input.TouchpadDown && !this.IsAltHeld)
			{
				Vector2 touchpadAxes = hand.Input.TouchpadAxes;
				if (touchpadAxes.magnitude > 0.2f && Vector2.Angle(touchpadAxes, Vector2.down) < 45f && this.CanCockHammer)
				{
					this.CockHammer();
				}
				else if (touchpadAxes.magnitude > 0.2f && (Vector2.Angle(touchpadAxes, Vector2.left) < 45f || Vector2.Angle(touchpadAxes, Vector2.right) < 45f) && this.CanUnlatch)
				{
					this.ToggleLatchState();
				}
			}
			if (this.m_isDestroyed)
			{
				return;
			}
			if (this.m_hasTriggeredUpSinceBegin && !this.IsAltHeld)
			{
				this.TriggerFloat = hand.Input.TriggerFloat;
			}
			else
			{
				this.TriggerFloat = 0f;
			}
			float x = Mathf.Lerp(this.TriggerForwardBackRots.x, this.TriggerForwardBackRots.y, this.TriggerFloat);
			this.Trigger.localEulerAngles = new Vector3(x, 0f, 0f);
			if (this.TriggerFloat > 0.7f)
			{
				if (this.m_isTriggerReset && this.m_isHammerCocked)
				{
					this.m_isTriggerReset = false;
					this.m_isHammerCocked = false;
					if (this.Hammer != null)
					{
						base.SetAnimatedComponent(this.Hammer, this.HammerMinRot, this.HammerInterp, this.HammerAxis);
					}
					base.PlayAudioEvent(FirearmAudioEventType.HammerHit, 1f);
					this.Fire();
				}
			}
			else if (this.TriggerFloat < 0.2f && !this.m_isTriggerReset)
			{
				this.m_isTriggerReset = true;
			}
		}

		private void Fire()
		{
			if (!this.m_isLatched)
			{
				return;
			}
			if (!this.Chamber.Fire())
			{
				return;
			}
			base.Fire(this.Chamber, this.GetMuzzle(), true, 1f);
			this.FireMuzzleSmoke();
			bool twoHandStabilized = this.IsTwoHandStabilized();
			bool foregripStabilized = base.AltGrip != null;
			bool shoulderStabilized = this.IsShoulderStabilized();
			if (this.Chamber.GetRound().IsHighPressure && !this.IsHighPressureTolerant)
			{
				this.Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized, null, 1f);
				this.Destroy();
			}
			else if (this.IsHighPressureTolerant)
			{
				this.Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized, null, 1f);
			}
			base.PlayAudioGunShot(this.Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment(), 1f);
			if (GM.CurrentSceneSettings.IsAmmoInfinite || GM.CurrentPlayerBody.IsInfiniteAmmo)
			{
				this.Chamber.IsSpent = false;
				this.Chamber.UpdateProxyDisplay();
			}
			else if (this.Chamber.GetRound().IsCaseless)
			{
				this.Chamber.SetRound(null);
			}
			if (this.DeletesCartridgeOnFire)
			{
				this.Chamber.SetRound(null);
			}
		}

		public void Unlatch()
		{
			if (this.m_isLatched)
			{
				base.PlayAudioEvent(FirearmAudioEventType.BreachOpen, 1f);
				this.m_isLatched = false;
				this.Chamber.IsAccessible = true;
				if (this.CocksOnOpen)
				{
					this.CockHammer();
				}
			}
		}

		public void Latch()
		{
			if (!this.m_isLatched)
			{
				base.PlayAudioEvent(FirearmAudioEventType.BreachClose, 1f);
				this.m_isLatched = true;
				this.Chamber.IsAccessible = false;
			}
		}

		private void CockHammer()
		{
			if (!this.m_isHammerCocked)
			{
				base.PlayAudioEvent(FirearmAudioEventType.Prefire, 1f);
				this.m_isHammerCocked = true;
				if (this.Hammer != null)
				{
					base.SetAnimatedComponent(this.Hammer, this.HammerMaxRot, this.HammerInterp, this.HammerAxis);
				}
			}
		}

		private void Destroy()
		{
			if (!this.m_isDestroyed)
			{
				this.m_isDestroyed = true;
				this.DestroyPSystem.Emit(25);
				for (int i = 0; i < this.GunUndamaged.Length; i++)
				{
					this.GunUndamaged[i].enabled = false;
					this.GunDamaged[i].enabled = true;
				}
			}
		}

		public override List<FireArmRoundClass> GetChamberRoundList()
		{
			if (this.Chamber.IsFull && !this.Chamber.IsSpent)
			{
				return new List<FireArmRoundClass>
				{
					this.Chamber.GetRound().RoundClass
				};
			}
			return null;
		}

		public override void SetLoadedChambers(List<FireArmRoundClass> rounds)
		{
			if (rounds.Count > 0)
			{
				this.Chamber.Autochamber(rounds[0]);
			}
		}

		public override List<string> GetFlagList()
		{
			return null;
		}

		public override void SetFromFlagList(List<string> flags)
		{
		}

		[Header("Flaregun Params")]
		public Renderer[] GunUndamaged;

		public Renderer[] GunDamaged;

		public FVRFireArmChamber Chamber;

		public FVRPhysicalObject.Axis HingeAxis;

		public Transform Hinge;

		public float RotOut = 35f;

		public bool CanUnlatch = true;

		public bool IsHighPressureTolerant;

		private bool m_isHammerCocked;

		private bool m_isTriggerReset = true;

		private bool m_isLatched = true;

		private bool m_isDestroyed;

		private float TriggerFloat;

		public Transform Hammer;

		public bool HasVisibleHammer = true;

		public bool CanCockHammer = true;

		public bool CocksOnOpen;

		private float m_hammerXRot;

		public FVRPhysicalObject.Axis HammerAxis;

		public FVRPhysicalObject.InterpStyle HammerInterp = FVRPhysicalObject.InterpStyle.Rotation;

		public float HammerMinRot;

		public float HammerMaxRot = -70f;

		public Transform Trigger;

		public Vector2 TriggerForwardBackRots;

		public Transform Muzzle;

		public ParticleSystem SmokePSystem;

		public ParticleSystem DestroyPSystem;

		public bool DeletesCartridgeOnFire;
	}
}
