using System;
using System.Collections.Generic;
using UnityEngine;
using FistVR;

namespace H3VRUtils.Weapons
{
	public class TatoWeapon : FVRFireArm
	{
		public bool HasExtractedRound()
		{
			return this.m_proxy.IsFull;
		}

		public bool IsHammerCocked => this.m_isHammerCocked;

		public int FireSelectorModeIndex => this.m_fireSelectorMode;

		protected override void Awake()
		{
			base.Awake();
			this.m_CamBurst = 1;
			GameObject gameObject = new GameObject("m_proxyRound");
			this.m_proxy = gameObject.AddComponent<FVRFirearmMovingProxyRound>();
			this.m_proxy.Init(base.transform);
		}

		public override int GetTutorialState()
		{
			if (this.Magazine == null)
			{
				return 0;
			}
			if (this.Magazine != null && !this.Magazine.HasARound())
			{
				return 5;
			}
			if (this.FireSelector_Modes[this.m_fireSelectorMode].ModeType == TatoWeapon.FireSelectorModeType.Safe)
			{
				return 1;
			}
			if (!this.Chamber.IsFull & this.m_timeSinceFiredShot > 0.4f)
			{
				return 2;
			}
			return base.AltGrip == null ? 3 : 4;
		}

		public void SecondaryFireSelectorClicked()
		{
			base.PlayAudioEvent(FirearmAudioEventType.FireSelector, 1f);
		}

		public void CockHammer()
		{
			if (this.m_isHammerCocked) return;
			this.m_isHammerCocked = true;
			base.PlayAudioEvent(FirearmAudioEventType.Prefire, 1f);
		}

		public void DropHammer()
		{
			if (!this.m_isHammerCocked) return;
			this.m_isHammerCocked = false;
			base.PlayAudioEvent(FirearmAudioEventType.HammerHit, 1f);
			this.Fire();
		}

		// Token: 0x060016E9 RID: 5865 RVA: 0x000A6191 File Offset: 0x000A4591
		public bool IsWeaponOnSafe()
		{
			return this.FireSelector_Modes.Length != 0 && this.FireSelector_Modes[this.m_fireSelectorMode].ModeType == TatoWeapon.FireSelectorModeType.Safe;
		}

		// Token: 0x060016EA RID: 5866 RVA: 0x000A61BC File Offset: 0x000A45BC
		public void ResetCamBurst()
		{
			TatoWeapon.FireSelectorMode fireSelectorMode = this.FireSelector_Modes[this.m_fireSelectorMode];
			this.m_CamBurst = fireSelectorMode.BurstAmount;
		}

		// Token: 0x060016EB RID: 5867 RVA: 0x000A61E4 File Offset: 0x000A45E4
		protected virtual void ToggleFireSelector()
		{
			if (this.FireSelector_Modes.Length <= 1) return;
			if (this.Bolt.UsesAKSafetyLock && !this.Bolt.IsBoltForwardOfSafetyLock())
			{
				int num = this.m_fireSelectorMode + 1;
				if (num >= this.FireSelector_Modes.Length)
				{
					num -= this.FireSelector_Modes.Length;
				}
				if (this.FireSelector_Modes[num].ModeType == TatoWeapon.FireSelectorModeType.Safe)
				{
					return;
				}
			}
			this.m_fireSelectorMode++;
			if (this.m_fireSelectorMode >= this.FireSelector_Modes.Length)
			{
				this.m_fireSelectorMode -= this.FireSelector_Modes.Length;
			}
			TatoWeapon.FireSelectorMode fireSelectorMode = this.FireSelector_Modes[this.m_fireSelectorMode];
			if (this.m_triggerFloat < 0.1f)
			{
				this.m_CamBurst = fireSelectorMode.BurstAmount;
			}
			base.PlayAudioEvent(FirearmAudioEventType.FireSelector, 1f);
			if (this.FireSelectorSwitch != null)
			{
				base.SetAnimatedComponent(this.FireSelectorSwitch, fireSelectorMode.SelectorPosition, this.FireSelector_InterpStyle, this.FireSelector_Axis);
			}
			if (this.FireSelectorSwitch2 != null)
			{
				TatoWeapon.FireSelectorMode fireSelectorMode2 = this.FireSelector_Modes2[this.m_fireSelectorMode];
				base.SetAnimatedComponent(this.FireSelectorSwitch2, fireSelectorMode2.SelectorPosition, this.FireSelector_InterpStyle2, this.FireSelector_Axis2);
			}
		}

		// Token: 0x060016EC RID: 5868 RVA: 0x000A6330 File Offset: 0x000A4730
		public void EjectExtractedRound()
		{
			if (this.Chamber.IsFull)
			{
				this.Chamber.EjectRound(this.RoundPos_Ejection.position, base.transform.right * this.EjectionSpeed.x + base.transform.up * this.EjectionSpeed.y + base.transform.forward * this.EjectionSpeed.z, base.transform.right * this.EjectionSpin.x + base.transform.up * this.EjectionSpin.y + base.transform.forward * this.EjectionSpin.z, false);
			}
		}

		// Token: 0x060016ED RID: 5869 RVA: 0x000A641C File Offset: 0x000A481C
		public void BeginChamberingRound()
		{
			bool flag = false;
			GameObject fromPrefabReference = null;
			if (this.HasBelt)
			{
				if (!this.m_proxy.IsFull && this.BeltDD.HasARound())
				{
					flag = true;
					fromPrefabReference = this.BeltDD.RemoveRound(false);
				}
			}
			else if (!this.m_proxy.IsFull && this.Magazine != null && !this.Magazine.IsBeltBox && this.Magazine.HasARound())
			{
				flag = true;
				fromPrefabReference = this.Magazine.RemoveRound(false);
			}
			if (!flag)
			{
				return;
			}
			if (flag)
			{
				this.m_proxy.SetFromPrefabReference(fromPrefabReference);
			}
		}

		// Token: 0x060016EE RID: 5870 RVA: 0x000A64D8 File Offset: 0x000A48D8
		public bool ChamberRound()
		{
			if (!this.m_proxy.IsFull || this.Chamber.IsFull) return false;
			this.Chamber.SetRound(this.m_proxy.Round);
			this.m_proxy.ClearProxy();
			return true;
		}

		// Token: 0x060016EF RID: 5871 RVA: 0x000A6529 File Offset: 0x000A4929
		public override Transform GetMagMountingTransform()
		{
			return this.UsesMagMountTransformOverride ? this.MagMountTransformOverride : base.GetMagMountingTransform();
		}

		// Token: 0x060016F0 RID: 5872 RVA: 0x000A6544 File Offset: 0x000A4944
		protected override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			if (!this.UsesStickyDetonation || !(this.m_stickyChargeUp > 0f)) return;
			base.RootRigidbody.velocity += UnityEngine.Random.onUnitSphere * 0.2f * this.m_stickyChargeUp;
			base.RootRigidbody.angularVelocity += UnityEngine.Random.onUnitSphere * 1f * this.m_stickyChargeUp;
		}

		// Token: 0x060016F1 RID: 5873 RVA: 0x000A65D4 File Offset: 0x000A49D4
		public bool Fire()
		{
			if (!this.Chamber.Fire())
			{
				return false;
			}
			this.m_timeSinceFiredShot = 0f;
			float velMult = 1f;
			if (this.UsesStickyDetonation)
			{
				velMult = 1f + Mathf.Lerp(0f, 1.3f, this.m_stickyChargeUp);
			}
			base.Fire(this.Chamber, this.GetMuzzle(), true, velMult);
			bool twoHandStabilized = this.IsTwoHandStabilized();
			bool foregripStabilized = base.AltGrip != null;
			bool shoulderStabilized = this.IsShoulderStabilized();
			this.Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized, null, 1f);
			bool flag = false;
			TatoWeapon.FireSelectorMode fireSelectorMode = this.FireSelector_Modes[this.m_fireSelectorMode];
			if (fireSelectorMode.ModeType == TatoWeapon.FireSelectorModeType.SuperFastBurst)
			{
				for (int i = 0; i < fireSelectorMode.BurstAmount - 1; i++)
				{
					if (!this.Magazine.HasARound()) continue;
					this.Magazine.RemoveRound();
					base.Fire(this.Chamber, this.GetMuzzle(), false, 1f);
					flag = true;
					this.Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized, null, 1f);
				}
			}
			this.FireMuzzleSmoke();
			if (this.UsesDelinker && this.HasBelt)
			{
				this.DelinkerSystem.Emit(1);
			}
			if (this.HasBelt)
			{
				this.BeltDD.AddJitter();
			}
			if (flag)
			{
				base.PlayAudioGunShot(false, this.Chamber.GetRound().TailClass, this.Chamber.GetRound().TailClassSuppressed, GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
			}
			else
			{
				base.PlayAudioGunShot(this.Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment(), 1f);
			}
			this.Bolt.ImpartFiringImpulse();
			return true;
		}

		// Token: 0x060016F2 RID: 5874 RVA: 0x000A6794 File Offset: 0x000A4B94
		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			this.UpdateComponents();
			if (this.HasHandle)
			{
				this.Handle.UpdateHandle();
				this.Bolt.UpdateHandleHeldState(this.Handle.ShouldControlBolt(), 1f - this.Handle.GetBoltLerpBetweenLockAndFore());
			}
			this.Bolt.UpdateBolt();
			if (this.UsesClips && this.DoesClipEntryRequireBoltBack)
			{
				if (this.Bolt.CurPos >= ClosedBolt.BoltPos.Locked)
				{
					if (!this.ClipTrigger.activeSelf)
					{
						this.ClipTrigger.SetActive(true);
					}
				}
				else if (this.ClipTrigger.activeSelf)
				{
					this.ClipTrigger.SetActive(false);
				}
			}
			this.UpdateDisplayRoundPositions();
			if (this.m_timeSinceFiredShot < 1f)
			{
				this.m_timeSinceFiredShot += Time.deltaTime;
			}
		}

		// Token: 0x060016F3 RID: 5875 RVA: 0x000A6881 File Offset: 0x000A4C81
		public override void LoadMag(FVRFireArmMagazine mag)
		{
			base.LoadMag(mag);
			if (this.BoltLocksWhenNoMagazineFound && mag != null && this.Bolt.IsBoltLocked())
			{
				this.Bolt.ReleaseBolt();
			}
		}

		// Token: 0x060016F4 RID: 5876 RVA: 0x000A68BC File Offset: 0x000A4CBC
		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			this.UpdateInputAndAnimate(hand);
		}

		// Token: 0x060016F5 RID: 5877 RVA: 0x000A68CC File Offset: 0x000A4CCC
		private void UpdateInputAndAnimate(FVRViveHand hand)
		{
			this.IsMagReleaseButtonHeld = false;
			this.IsBoltReleaseButtonHeld = false;
			this.IsBoltCatchButtonHeld = false;
			if (this.IsAltHeld)
			{
				return;
			}
			if (this.m_hasTriggeredUpSinceBegin)
			{
				this.m_triggerFloat = hand.Input.TriggerFloat;
			}
			else
			{
				this.m_triggerFloat = 0f;
			}
			if (!this.m_hasTriggerReset && this.m_triggerFloat <= this.TriggerResetThreshold)
			{
				this.m_hasTriggerReset = true;
				if (this.FireSelector_Modes.Length > 0)
				{
					this.m_CamBurst = this.FireSelector_Modes[this.m_fireSelectorMode].BurstAmount;
				}
				base.PlayAudioEvent(FirearmAudioEventType.TriggerReset, 1f);
			}
			Vector2 touchpadAxes = hand.Input.TouchpadAxes;
			if (hand.Input.TouchpadDown)
			{
				if (this.UsesStickyDetonation)
				{
					this.Detonate();
				}
				if (touchpadAxes.magnitude > 0.2f)
				{
					if (Vector2.Angle(touchpadAxes, Vector2.left) <= 45f)
					{
						this.ToggleFireSelector();
					}
					else if (Vector2.Angle(touchpadAxes, Vector2.up) <= 45f)
					{
						if (this.HasBoltReleaseButton)
						{
							this.Bolt.ReleaseBolt();
						}
					}
					else if (Vector2.Angle(touchpadAxes, Vector2.down) <= 45f && this.HasMagReleaseButton && (!this.EjectsMagazineOnEmpty || (this.Bolt.CurPos >= ClosedBolt.BoltPos.Locked && this.Bolt.IsHeld && !this.m_proxy.IsFull)))
					{
						this.ReleaseMag();
					}
				}
			}
			if (this.UsesStickyDetonation)
			{
				if (hand.Input.TouchpadDown)
				{
					base.SetAnimatedComponent(this.StickyTrigger, this.StickyRotRange.y, FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
				}
				else if (hand.Input.TouchpadUp)
				{
					base.SetAnimatedComponent(this.StickyTrigger, this.StickyRotRange.x, FVRPhysicalObject.InterpStyle.Rotation, FVRPhysicalObject.Axis.X);
				}
			}
			if (hand.Input.TouchpadPressed && touchpadAxes.magnitude > 0.2f)
			{
				if (Vector2.Angle(touchpadAxes, Vector2.down) <= 45f)
				{
					if (this.HasMagReleaseButton)
					{
						this.IsMagReleaseButtonHeld = true;
					}
				}
				else if (Vector2.Angle(touchpadAxes, Vector2.up) <= 45f)
				{
					if (this.HasBoltReleaseButton)
					{
						this.IsBoltReleaseButtonHeld = true;
					}
				}
				else if (Vector2.Angle(touchpadAxes, Vector2.right) <= 45f && this.HasBoltCatchButton)
				{
					this.IsBoltCatchButtonHeld = true;
				}
			}
			if (this.IsMagReleaseButtonHeld)
			{
			}
			TatoWeapon.FireSelectorModeType modeType = this.FireSelector_Modes[this.m_fireSelectorMode].ModeType;
			if (modeType != TatoWeapon.FireSelectorModeType.Safe && this.m_hasTriggeredUpSinceBegin)
			{
				if (this.UsesStickyDetonation)
				{
					if (this.Bolt.CurPos == ClosedBolt.BoltPos.Forward && this.Chamber.IsFull && !this.Chamber.IsSpent)
					{
						if (hand.Input.TriggerPressed && this.m_hasTriggerReset)
						{
							this.m_hasStickTriggerDown = true;
							this.m_stickyChargeUp += Time.deltaTime * 0.25f;
							this.m_stickyChargeUp = Mathf.Clamp(this.m_stickyChargeUp, 0f, 1f);
							if (this.m_stickyChargeUp > 0.05f && !this.m_chargeSound.isPlaying)
							{
								this.m_chargeSound.Play();
							}
						}
						else
						{
							if (this.m_chargeSound.isPlaying)
							{
								this.m_chargeSound.Stop();
							}
							this.m_stickyChargeUp = 0f;
						}
						if (this.m_hasStickTriggerDown && (hand.Input.TriggerUp || this.m_stickyChargeUp >= 1f))
						{
							this.m_hasStickTriggerDown = false;
							this.DropHammer();
							this.EndStickyCharge();
						}
					}
					return;
				}
				if (this.m_triggerFloat >= this.TriggerFiringThreshold && this.Bolt.CurPos == ClosedBolt.BoltPos.Forward && (this.m_hasTriggerReset || (modeType == TatoWeapon.FireSelectorModeType.FullAuto && !this.UsesDualStageFullAuto) || (modeType == TatoWeapon.FireSelectorModeType.FullAuto && this.UsesDualStageFullAuto && this.m_triggerFloat > this.TriggerDualStageThreshold) || (modeType == TatoWeapon.FireSelectorModeType.Burst && this.m_CamBurst > 0 || (isOpenBolt = true))))
				{
					this.DropHammer();
					this.m_hasTriggerReset = false;
					if (this.m_CamBurst > 0)
					{
						this.m_CamBurst--;
					}
				}
			}
		}

		// Token: 0x060016F6 RID: 5878 RVA: 0x000A6D62 File Offset: 0x000A5162
		public override void EndInteraction(FVRViveHand hand)
		{
			if (this.UsesStickyDetonation)
			{
				this.EndStickyCharge();
			}
			base.EndInteraction(hand);
		}

		// Token: 0x060016F7 RID: 5879 RVA: 0x000A6D7C File Offset: 0x000A517C
		private void EndStickyCharge()
		{
			this.m_chargeSound.Stop();
			this.m_stickyChargeUp = 0f;
		}

		// Token: 0x060016F8 RID: 5880 RVA: 0x000A6D94 File Offset: 0x000A5194
		private void UpdateComponents()
		{
			if (this.HasTrigger)
			{
				base.SetAnimatedComponent(this.Trigger, Mathf.Lerp(this.Trigger_ForwardValue, this.Trigger_RearwardValue, this.m_triggerFloat), this.TriggerInterpStyle, this.TriggerAxis);
			}
			if (this.UsesStickyDetonation)
			{
				float t = Mathf.Clamp((float)this.m_primedBombs.Count / 8f, 0f, 1f);
				float y = Mathf.Lerp(0.56f, 0.23f, t);
				float num = Mathf.Lerp(5f, 15f, t);
				this.StickyScreen.material.SetTextureOffset("_IncandescenceMap", new Vector2(0f, y));
			}
		}

		// Token: 0x060016F9 RID: 5881 RVA: 0x000A6E4C File Offset: 0x000A524C
		private void UpdateDisplayRoundPositions()
		{
			float boltLerpBetweenLockAndFore = this.Bolt.GetBoltLerpBetweenLockAndFore();
			if (this.Chamber.IsFull)
			{
				this.Chamber.ProxyRound.position = Vector3.Lerp(this.RoundPos_Ejecting.position, this.Chamber.transform.position, boltLerpBetweenLockAndFore);
				this.Chamber.ProxyRound.rotation = Quaternion.Slerp(this.RoundPos_Ejecting.rotation, this.Chamber.transform.rotation, boltLerpBetweenLockAndFore);
			}
			if (this.m_proxy.IsFull)
			{
				this.m_proxy.ProxyRound.position = Vector3.Lerp(this.RoundPos_MagazinePos.position, this.Chamber.transform.position, boltLerpBetweenLockAndFore);
				this.m_proxy.ProxyRound.rotation = Quaternion.Slerp(this.RoundPos_MagazinePos.rotation, this.Chamber.transform.rotation, boltLerpBetweenLockAndFore);
			}
		}

		// Token: 0x060016FA RID: 5882 RVA: 0x000A6F49 File Offset: 0x000A5349
		public void ReleaseMag()
		{
			if (this.Magazine != null)
			{
				base.EjectMag();
			}
		}

		// Token: 0x060016FB RID: 5883 RVA: 0x000A6F62 File Offset: 0x000A5362
		public void RegisterStickyBomb(MF2_StickyBomb sb)
		{
			if (sb != null)
			{
				this.m_primedBombs.Add(sb);
			}
		}

		// Token: 0x060016FC RID: 5884 RVA: 0x000A6F7C File Offset: 0x000A537C
		public void Detonate()
		{
			bool flag = false;
			if (this.m_primedBombs.Count > 0)
			{
				for (int i = this.m_primedBombs.Count - 1; i >= 0; i--)
				{
					if (this.m_primedBombs[i] != null && this.m_primedBombs[i].m_hasStuck && !this.m_primedBombs[i].m_hasExploded)
					{
						flag = true;
						this.m_primedBombs[i].Detonate();
						this.m_primedBombs.RemoveAt(i);
					}
				}
			}
			if (flag)
			{
				SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.StickyDetonateSound, base.transform.position);
			}
		}

		// Token: 0x060016FD RID: 5885 RVA: 0x000A703C File Offset: 0x000A543C
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

		// Token: 0x060016FE RID: 5886 RVA: 0x000A7088 File Offset: 0x000A5488
		public override void SetLoadedChambers(List<FireArmRoundClass> rounds)
		{
			if (rounds.Count > 0)
			{
				this.Chamber.Autochamber(rounds[0]);
			}
		}

		// Token: 0x060016FF RID: 5887 RVA: 0x000A70A8 File Offset: 0x000A54A8
		public override List<string> GetFlagList()
		{
			return null;
		}

		// Token: 0x06001700 RID: 5888 RVA: 0x000A70AB File Offset: 0x000A54AB
		public override void SetFromFlagList(List<string> flags)
		{
		}

		// Token: 0x06001701 RID: 5889 RVA: 0x000A70B0 File Offset: 0x000A54B0
		public override void ConfigureFromFlagDic(Dictionary<string, string> f)
		{
			string key = string.Empty;
			string text = string.Empty;
			key = "HammerState";
			if (f.ContainsKey(key))
			{
				text = f[key];
				if (text == "Cocked")
				{
					this.m_isHammerCocked = true;
				}
			}
			if (this.FireSelector_Modes.Length > 1)
			{
				key = "FireSelectorState";
				if (f.ContainsKey(key))
				{
					text = f[key];
					int.TryParse(text, out this.m_fireSelectorMode);
				}
				if (this.FireSelectorSwitch != null)
				{
					TatoWeapon.FireSelectorMode fireSelectorMode = this.FireSelector_Modes[this.m_fireSelectorMode];
					base.SetAnimatedComponent(this.FireSelectorSwitch, fireSelectorMode.SelectorPosition, this.FireSelector_InterpStyle, this.FireSelector_Axis);
				}
				if (this.FireSelectorSwitch2 != null)
				{
					TatoWeapon.FireSelectorMode fireSelectorMode2 = this.FireSelector_Modes2[this.m_fireSelectorMode];
					base.SetAnimatedComponent(this.FireSelectorSwitch2, fireSelectorMode2.SelectorPosition, this.FireSelector_InterpStyle2, this.FireSelector_Axis2);
				}
			}
		}

		// Token: 0x06001702 RID: 5890 RVA: 0x000A71AC File Offset: 0x000A55AC
		public override Dictionary<string, string> GetFlagDic()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string key = "HammerState";
			string value = "Uncocked";
			if (this.m_isHammerCocked)
			{
				value = "Cocked";
			}
			dictionary.Add(key, value);
			if (this.FireSelector_Modes.Length > 1)
			{
				key = "FireSelectorState";
				value = this.m_fireSelectorMode.ToString();
				dictionary.Add(key, value);
			}
			return dictionary;
		}

		// Token: 0x04002D2B RID: 11563
		[Header("ClosedBoltWeapon Config")]
		public bool HasFireSelectorButton = true;

		// Token: 0x04002D2C RID: 11564
		public bool HasMagReleaseButton = true;

		// Token: 0x04002D2D RID: 11565
		public bool HasBoltReleaseButton = true;

		// Token: 0x04002D2E RID: 11566
		public bool HasBoltCatchButton = true;

		// Token: 0x04002D2F RID: 11567
		public bool HasHandle = true;

		// Token: 0x04002D30 RID: 11568
		[Header("Component Connections")]
		public ClosedBolt Bolt;

		// Token: 0x04002D31 RID: 11569
		public ClosedBoltHandle Handle;

		// Token: 0x04002D32 RID: 11570
		public FVRFireArmChamber Chamber;

		// Token: 0x04002D33 RID: 11571
		public Transform Trigger;

		// Token: 0x04002D34 RID: 11572
		public Transform FireSelectorSwitch;

		// Token: 0x04002D35 RID: 11573
		public Transform FireSelectorSwitch2;

		// Token: 0x04002D36 RID: 11574
		[Header("Round Positions")]
		public Transform RoundPos_Ejecting;

		// Token: 0x04002D37 RID: 11575
		public Transform RoundPos_Ejection;

		// Token: 0x04002D38 RID: 11576
		public Transform RoundPos_MagazinePos;

		// Token: 0x04002D39 RID: 11577
		private FVRFirearmMovingProxyRound m_proxy;

		// Token: 0x04002D3A RID: 11578
		public Vector3 EjectionSpeed = new Vector3(4f, 2.5f, -1.2f);

		// Token: 0x04002D3B RID: 11579
		public Vector3 EjectionSpin = new Vector3(20f, 180f, 30f);

		// Token: 0x04002D3C RID: 11580
		public bool UsesDelinker;

		// Token: 0x04002D3D RID: 11581
		public ParticleSystem DelinkerSystem;

		// Token: 0x04002D3E RID: 11582
		[Header("Trigger Config")]
		public bool HasTrigger;

		// Token: 0x04002D3F RID: 11583
		public float TriggerFiringThreshold = 0.8f;

		// Token: 0x04002D40 RID: 11584
		public float TriggerResetThreshold = 0.4f;

		// Token: 0x04002D41 RID: 11585
		public float TriggerDualStageThreshold = 0.95f;

		// Token: 0x04002D42 RID: 11586
		public float Trigger_ForwardValue;

		// Token: 0x04002D43 RID: 11587
		public float Trigger_RearwardValue;

		// Token: 0x04002D44 RID: 11588
		public FVRPhysicalObject.Axis TriggerAxis;

		// Token: 0x04002D45 RID: 11589
		public FVRPhysicalObject.InterpStyle TriggerInterpStyle = FVRPhysicalObject.InterpStyle.Rotation;

		// Token: 0x04002D46 RID: 11590
		public bool UsesDualStageFullAuto;

		// Token: 0x04002D47 RID: 11591
		private float m_triggerFloat;

		// Token: 0x04002D48 RID: 11592
		private bool m_hasTriggerReset;

		// Token: 0x04002D49 RID: 11593
		private int m_CamBurst;

		// Token: 0x04002D4A RID: 11594
		private bool m_isHammerCocked;

		// Token: 0x04002D4B RID: 11595
		private int m_fireSelectorMode;

		// Token: 0x04002D4C RID: 11596
		[Header("Fire Selector Config")]
		public FVRPhysicalObject.InterpStyle FireSelector_InterpStyle = FVRPhysicalObject.InterpStyle.Rotation;

		// Token: 0x04002D4D RID: 11597
		public FVRPhysicalObject.Axis FireSelector_Axis;

		// Token: 0x04002D4E RID: 11598
		public TatoWeapon.FireSelectorMode[] FireSelector_Modes;

		// Token: 0x04002D4F RID: 11599
		[Header("Secondary Fire Selector Config")]
		public FVRPhysicalObject.InterpStyle FireSelector_InterpStyle2 = FVRPhysicalObject.InterpStyle.Rotation;

		// Token: 0x04002D50 RID: 11600
		public FVRPhysicalObject.Axis FireSelector_Axis2;

		// Token: 0x04002D51 RID: 11601
		public TatoWeapon.FireSelectorMode[] FireSelector_Modes2;

		// Token: 0x04002D52 RID: 11602
		[Header("SpecialFeatures")]
		public bool EjectsMagazineOnEmpty;

		// Token: 0x04002D53 RID: 11603
		public bool BoltLocksWhenNoMagazineFound;

		// Token: 0x04002D54 RID: 11604
		public bool DoesClipEntryRequireBoltBack = true;

		// Token: 0x04002D55 RID: 11605
		public bool UsesMagMountTransformOverride;

		// Token: 0x04002D56 RID: 11606
		public Transform MagMountTransformOverride;

		// Token: 0x04002D57 RID: 11607
		[Header("StickySystem")]
		public bool UsesStickyDetonation;

		// Token: 0x04002D58 RID: 11608
		public AudioEvent StickyDetonateSound;

		// Token: 0x04002D59 RID: 11609
		public Transform StickyTrigger;

		// Token: 0x04002D5A RID: 11610
		public Vector2 StickyRotRange = new Vector2(0f, 20f);

		// Token: 0x04002D5B RID: 11611
		private float m_stickyChargeUp;

		// Token: 0x04002D5C RID: 11612
		public AudioSource m_chargeSound;

		// Token: 0x04002D5D RID: 11613
		public Renderer StickyScreen;

		// Token: 0x04002D5E RID: 11614
		[HideInInspector]
		public bool IsMagReleaseButtonHeld;

		// Token: 0x04002D5F RID: 11615
		[HideInInspector]
		public bool IsBoltReleaseButtonHeld;

		// Token: 0x04002D60 RID: 11616
		[HideInInspector]
		public bool IsBoltCatchButtonHeld;

		// Token: 0x04002D61 RID: 11617
		private float m_timeSinceFiredShot = 1f;

		// Token: 0x04002D62 RID: 11618
		private bool m_hasStickTriggerDown;

		// Token: 0x04002D63 RID: 11619
		private List<MF2_StickyBomb> m_primedBombs = new List<MF2_StickyBomb>();

		// Token: 0x02000454 RID: 1108
		public enum FireSelectorModeType
		{
			// Token: 0x04002D65 RID: 11621
			Safe,
			// Token: 0x04002D66 RID: 11622
			Single,
			// Token: 0x04002D67 RID: 11623
			Burst,
			// Token: 0x04002D68 RID: 11624
			FullAuto,
			// Token: 0x04002D69 RID: 11625
			SuperFastBurst
		}

		// Token: 0x02000455 RID: 1109
		[Serializable]
		public class FireSelectorMode
		{
			// Token: 0x04002D6A RID: 11626
			public float SelectorPosition;

			// Token: 0x04002D6B RID: 11627
			public TatoWeapon.FireSelectorModeType ModeType;

			// Token: 0x04002D6C RID: 11628
			public int BurstAmount = 3;
		}
		public bool isOpenBolt;
	}
}
