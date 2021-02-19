using System;
using System.Collections.Generic;
using FistVR;
using UnityEngine;
using Random = UnityEngine.Random;

namespace H3VRUtils.Weapons
{
    public class TatoWeapon : FVRFireArm
    {
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

        // Token: 0x04002D2B RID: 11563
        [Header("ClosedBoltWeapon Config")] public bool HasFireSelectorButton = true;

        // Token: 0x04002D2C RID: 11564
        public bool HasMagReleaseButton = true;

        // Token: 0x04002D2D RID: 11565
        public bool HasBoltReleaseButton = true;

        // Token: 0x04002D2E RID: 11566
        public bool HasBoltCatchButton = true;

        // Token: 0x04002D2F RID: 11567
        public bool HasHandle = true;

        // Token: 0x04002D30 RID: 11568
        [Header("Component Connections")] public ClosedBolt Bolt;

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
        [Header("Round Positions")] public Transform RoundPos_Ejecting;

        // Token: 0x04002D37 RID: 11575
        public Transform RoundPos_Ejection;

        // Token: 0x04002D38 RID: 11576
        public Transform RoundPos_MagazinePos;

        // Token: 0x04002D3A RID: 11578
        public Vector3 EjectionSpeed = new Vector3(4f, 2.5f, -1.2f);

        // Token: 0x04002D3B RID: 11579
        public Vector3 EjectionSpin = new Vector3(20f, 180f, 30f);

        // Token: 0x04002D3C RID: 11580
        public bool UsesDelinker;

        // Token: 0x04002D3D RID: 11581
        public ParticleSystem DelinkerSystem;

        // Token: 0x04002D3E RID: 11582
        [Header("Trigger Config")] public bool HasTrigger;

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
        public Axis TriggerAxis;

        // Token: 0x04002D45 RID: 11589
        public InterpStyle TriggerInterpStyle = InterpStyle.Rotation;

        // Token: 0x04002D46 RID: 11590
        public bool UsesDualStageFullAuto;

        // Token: 0x04002D4C RID: 11596
        [Header("Fire Selector Config")] public InterpStyle FireSelector_InterpStyle = InterpStyle.Rotation;

        // Token: 0x04002D4D RID: 11597
        public Axis FireSelector_Axis;

        // Token: 0x04002D4E RID: 11598
        public FireSelectorMode[] FireSelector_Modes;

        // Token: 0x04002D4F RID: 11599
        [Header("Secondary Fire Selector Config")]
        public InterpStyle FireSelector_InterpStyle2 = InterpStyle.Rotation;

        // Token: 0x04002D50 RID: 11600
        public Axis FireSelector_Axis2;

        // Token: 0x04002D51 RID: 11601
        public FireSelectorMode[] FireSelector_Modes2;

        // Token: 0x04002D52 RID: 11602
        [Header("SpecialFeatures")] public bool EjectsMagazineOnEmpty;

        // Token: 0x04002D53 RID: 11603
        public bool BoltLocksWhenNoMagazineFound;

        // Token: 0x04002D54 RID: 11604
        public bool DoesClipEntryRequireBoltBack = true;

        // Token: 0x04002D55 RID: 11605
        public bool UsesMagMountTransformOverride;

        // Token: 0x04002D56 RID: 11606
        public Transform MagMountTransformOverride;

        // Token: 0x04002D57 RID: 11607
        [Header("StickySystem")] public bool UsesStickyDetonation;

        // Token: 0x04002D58 RID: 11608
        public AudioEvent StickyDetonateSound;

        // Token: 0x04002D59 RID: 11609
        public Transform StickyTrigger;

        // Token: 0x04002D5A RID: 11610
        public Vector2 StickyRotRange = new Vector2(0f, 20f);

        // Token: 0x04002D5C RID: 11612
        public AudioSource m_chargeSound;

        // Token: 0x04002D5D RID: 11613
        public Renderer StickyScreen;

        // Token: 0x04002D5E RID: 11614
        [HideInInspector] public bool IsMagReleaseButtonHeld;

        // Token: 0x04002D5F RID: 11615
        [HideInInspector] public bool IsBoltReleaseButtonHeld;

        // Token: 0x04002D60 RID: 11616
        [HideInInspector] public bool IsBoltCatchButtonHeld;

        public bool isOpenBolt;

        // Token: 0x04002D49 RID: 11593
        private int m_CamBurst;

        // Token: 0x04002D4B RID: 11595
        private int m_fireSelectorMode;

        // Token: 0x04002D62 RID: 11618
        private bool m_hasStickTriggerDown;

        // Token: 0x04002D48 RID: 11592
        private bool m_hasTriggerReset;

        // Token: 0x04002D4A RID: 11594

        // Token: 0x04002D63 RID: 11619
        private readonly List<MF2_StickyBomb> m_primedBombs = new List<MF2_StickyBomb>();

        // Token: 0x04002D39 RID: 11577
        private FVRFirearmMovingProxyRound m_proxy;

        // Token: 0x04002D5B RID: 11611
        private float m_stickyChargeUp;

        // Token: 0x04002D61 RID: 11617
        private float m_timeSinceFiredShot = 1f;

        // Token: 0x04002D47 RID: 11591
        private float m_triggerFloat;

        public bool IsHammerCocked { get; private set; }

        public int FireSelectorModeIndex => m_fireSelectorMode;

        protected override void Awake()
        {
            base.Awake();
            m_CamBurst = 1;
            var gameObject = new GameObject("m_proxyRound");
            m_proxy = gameObject.AddComponent<FVRFirearmMovingProxyRound>();
            m_proxy.Init(transform);
        }

        public bool HasExtractedRound()
        {
            return m_proxy.IsFull;
        }

        public override int GetTutorialState()
        {
            if (Magazine == null) return 0;
            if (Magazine != null && !Magazine.HasARound()) return 5;
            if (FireSelector_Modes[m_fireSelectorMode].ModeType == FireSelectorModeType.Safe) return 1;
            if (!Chamber.IsFull & (m_timeSinceFiredShot > 0.4f)) return 2;
            if (AltGrip == null) return 3;
            return 4;
        }

        public void SecondaryFireSelectorClicked()
        {
            PlayAudioEvent(FirearmAudioEventType.FireSelector);
        }

        public void CockHammer()
        {
            if (!IsHammerCocked)
            {
                IsHammerCocked = true;
                PlayAudioEvent(FirearmAudioEventType.Prefire);
            }
        }

        public void DropHammer()
        {
            if (IsHammerCocked)
            {
                IsHammerCocked = false;
                PlayAudioEvent(FirearmAudioEventType.HammerHit);
                Fire();
            }
        }

        // Token: 0x060016E9 RID: 5865 RVA: 0x000A6191 File Offset: 0x000A4591
        public bool IsWeaponOnSafe()
        {
            return FireSelector_Modes.Length != 0 &&
                   FireSelector_Modes[m_fireSelectorMode].ModeType == FireSelectorModeType.Safe;
        }

        // Token: 0x060016EA RID: 5866 RVA: 0x000A61BC File Offset: 0x000A45BC
        public void ResetCamBurst()
        {
            var fireSelectorMode = FireSelector_Modes[m_fireSelectorMode];
            m_CamBurst = fireSelectorMode.BurstAmount;
        }

        // Token: 0x060016EB RID: 5867 RVA: 0x000A61E4 File Offset: 0x000A45E4
        protected virtual void ToggleFireSelector()
        {
            if (FireSelector_Modes.Length > 1)
            {
                if (Bolt.UsesAKSafetyLock && !Bolt.IsBoltForwardOfSafetyLock())
                {
                    var num = m_fireSelectorMode + 1;
                    if (num >= FireSelector_Modes.Length) num -= FireSelector_Modes.Length;
                    if (FireSelector_Modes[num].ModeType == FireSelectorModeType.Safe) return;
                }

                m_fireSelectorMode++;
                if (m_fireSelectorMode >= FireSelector_Modes.Length) m_fireSelectorMode -= FireSelector_Modes.Length;
                var fireSelectorMode = FireSelector_Modes[m_fireSelectorMode];
                if (m_triggerFloat < 0.1f) m_CamBurst = fireSelectorMode.BurstAmount;
                PlayAudioEvent(FirearmAudioEventType.FireSelector);
                if (FireSelectorSwitch != null)
                    SetAnimatedComponent(FireSelectorSwitch, fireSelectorMode.SelectorPosition,
                        FireSelector_InterpStyle, FireSelector_Axis);
                if (FireSelectorSwitch2 != null)
                {
                    var fireSelectorMode2 = FireSelector_Modes2[m_fireSelectorMode];
                    SetAnimatedComponent(FireSelectorSwitch2, fireSelectorMode2.SelectorPosition,
                        FireSelector_InterpStyle2, FireSelector_Axis2);
                }
            }
        }

        // Token: 0x060016EC RID: 5868 RVA: 0x000A6330 File Offset: 0x000A4730
        public void EjectExtractedRound()
        {
            if (Chamber.IsFull)
                Chamber.EjectRound(RoundPos_Ejection.position,
                    transform.right * EjectionSpeed.x + transform.up * EjectionSpeed.y +
                    transform.forward * EjectionSpeed.z,
                    transform.right * EjectionSpin.x + transform.up * EjectionSpin.y +
                    transform.forward * EjectionSpin.z);
        }

        // Token: 0x060016ED RID: 5869 RVA: 0x000A641C File Offset: 0x000A481C
        public void BeginChamberingRound()
        {
            var flag = false;
            GameObject fromPrefabReference = null;
            if (HasBelt)
            {
                if (!m_proxy.IsFull && BeltDD.HasARound())
                {
                    flag = true;
                    fromPrefabReference = BeltDD.RemoveRound(false);
                }
            }
            else if (!m_proxy.IsFull && Magazine != null && !Magazine.IsBeltBox && Magazine.HasARound())
            {
                flag = true;
                fromPrefabReference = Magazine.RemoveRound(false);
            }

            if (!flag) return;
            if (flag) m_proxy.SetFromPrefabReference(fromPrefabReference);
        }

        // Token: 0x060016EE RID: 5870 RVA: 0x000A64D8 File Offset: 0x000A48D8
        public bool ChamberRound()
        {
            if (m_proxy.IsFull && !Chamber.IsFull)
            {
                Chamber.SetRound(m_proxy.Round);
                m_proxy.ClearProxy();
                return true;
            }

            return false;
        }

        // Token: 0x060016EF RID: 5871 RVA: 0x000A6529 File Offset: 0x000A4929
        public override Transform GetMagMountingTransform()
        {
            if (UsesMagMountTransformOverride) return MagMountTransformOverride;
            return base.GetMagMountingTransform();
        }

        // Token: 0x060016F0 RID: 5872 RVA: 0x000A6544 File Offset: 0x000A4944
        protected override void FVRFixedUpdate()
        {
            base.FVRFixedUpdate();
            if (UsesStickyDetonation && m_stickyChargeUp > 0f)
            {
                RootRigidbody.velocity += Random.onUnitSphere * 0.2f * m_stickyChargeUp;
                RootRigidbody.angularVelocity += Random.onUnitSphere * 1f * m_stickyChargeUp;
            }
        }

        // Token: 0x060016F1 RID: 5873 RVA: 0x000A65D4 File Offset: 0x000A49D4
        public bool Fire()
        {
            if (!Chamber.Fire()) return false;
            m_timeSinceFiredShot = 0f;
            var velMult = 1f;
            if (UsesStickyDetonation) velMult = 1f + Mathf.Lerp(0f, 1.3f, m_stickyChargeUp);
            base.Fire(Chamber, GetMuzzle(), true, velMult);
            var twoHandStabilized = IsTwoHandStabilized();
            var foregripStabilized = AltGrip != null;
            var shoulderStabilized = IsShoulderStabilized();
            Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
            var flag = false;
            var fireSelectorMode = FireSelector_Modes[m_fireSelectorMode];
            if (fireSelectorMode.ModeType == FireSelectorModeType.SuperFastBurst)
                for (var i = 0; i < fireSelectorMode.BurstAmount - 1; i++)
                    if (Magazine.HasARound())
                    {
                        Magazine.RemoveRound();
                        base.Fire(Chamber, GetMuzzle(), false);
                        flag = true;
                        Recoil(twoHandStabilized, foregripStabilized, shoulderStabilized);
                    }

            FireMuzzleSmoke();
            if (UsesDelinker && HasBelt) DelinkerSystem.Emit(1);
            if (HasBelt) BeltDD.AddJitter();
            if (flag)
                PlayAudioGunShot(false, Chamber.GetRound().TailClass, Chamber.GetRound().TailClassSuppressed,
                    GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
            else
                PlayAudioGunShot(Chamber.GetRound(), GM.CurrentPlayerBody.GetCurrentSoundEnvironment());
            Bolt.ImpartFiringImpulse();
            return true;
        }

        // Token: 0x060016F2 RID: 5874 RVA: 0x000A6794 File Offset: 0x000A4B94
        protected override void FVRUpdate()
        {
            base.FVRUpdate();
            UpdateComponents();
            if (HasHandle)
            {
                Handle.UpdateHandle();
                Bolt.UpdateHandleHeldState(Handle.ShouldControlBolt(), 1f - Handle.GetBoltLerpBetweenLockAndFore());
            }

            Bolt.UpdateBolt();
            if (UsesClips && DoesClipEntryRequireBoltBack)
            {
                if (Bolt.CurPos >= ClosedBolt.BoltPos.Locked)
                {
                    if (!ClipTrigger.activeSelf) ClipTrigger.SetActive(true);
                }
                else if (ClipTrigger.activeSelf)
                {
                    ClipTrigger.SetActive(false);
                }
            }

            UpdateDisplayRoundPositions();
            if (m_timeSinceFiredShot < 1f) m_timeSinceFiredShot += Time.deltaTime;
        }

        // Token: 0x060016F3 RID: 5875 RVA: 0x000A6881 File Offset: 0x000A4C81
        public override void LoadMag(FVRFireArmMagazine mag)
        {
            base.LoadMag(mag);
            if (BoltLocksWhenNoMagazineFound && mag != null && Bolt.IsBoltLocked()) Bolt.ReleaseBolt();
        }

        // Token: 0x060016F4 RID: 5876 RVA: 0x000A68BC File Offset: 0x000A4CBC
        public override void UpdateInteraction(FVRViveHand hand)
        {
            base.UpdateInteraction(hand);
            UpdateInputAndAnimate(hand);
        }

        // Token: 0x060016F5 RID: 5877 RVA: 0x000A68CC File Offset: 0x000A4CCC
        private void UpdateInputAndAnimate(FVRViveHand hand)
        {
            IsMagReleaseButtonHeld = false;
            IsBoltReleaseButtonHeld = false;
            IsBoltCatchButtonHeld = false;
            if (IsAltHeld) return;
            if (m_hasTriggeredUpSinceBegin)
                m_triggerFloat = hand.Input.TriggerFloat;
            else
                m_triggerFloat = 0f;
            if (!m_hasTriggerReset && m_triggerFloat <= TriggerResetThreshold)
            {
                m_hasTriggerReset = true;
                if (FireSelector_Modes.Length > 0) m_CamBurst = FireSelector_Modes[m_fireSelectorMode].BurstAmount;
                PlayAudioEvent(FirearmAudioEventType.TriggerReset);
            }

            var touchpadAxes = hand.Input.TouchpadAxes;
            if (hand.Input.TouchpadDown)
            {
                if (UsesStickyDetonation) Detonate();
                if (touchpadAxes.magnitude > 0.2f)
                {
                    if (Vector2.Angle(touchpadAxes, Vector2.left) <= 45f)
                    {
                        ToggleFireSelector();
                    }
                    else if (Vector2.Angle(touchpadAxes, Vector2.up) <= 45f)
                    {
                        if (HasBoltReleaseButton) Bolt.ReleaseBolt();
                    }
                    else if (Vector2.Angle(touchpadAxes, Vector2.down) <= 45f && HasMagReleaseButton &&
                             (!EjectsMagazineOnEmpty || Bolt.CurPos >= ClosedBolt.BoltPos.Locked && Bolt.IsHeld &&
                                 !m_proxy.IsFull))
                    {
                        ReleaseMag();
                    }
                }
            }

            if (UsesStickyDetonation)
            {
                if (hand.Input.TouchpadDown)
                    SetAnimatedComponent(StickyTrigger, StickyRotRange.y, InterpStyle.Rotation, Axis.X);
                else if (hand.Input.TouchpadUp)
                    SetAnimatedComponent(StickyTrigger, StickyRotRange.x, InterpStyle.Rotation, Axis.X);
            }

            if (hand.Input.TouchpadPressed && touchpadAxes.magnitude > 0.2f)
            {
                if (Vector2.Angle(touchpadAxes, Vector2.down) <= 45f)
                {
                    if (HasMagReleaseButton) IsMagReleaseButtonHeld = true;
                }
                else if (Vector2.Angle(touchpadAxes, Vector2.up) <= 45f)
                {
                    if (HasBoltReleaseButton) IsBoltReleaseButtonHeld = true;
                }
                else if (Vector2.Angle(touchpadAxes, Vector2.right) <= 45f && HasBoltCatchButton)
                {
                    IsBoltCatchButtonHeld = true;
                }
            }

            if (IsMagReleaseButtonHeld)
            {
            }

            var modeType = FireSelector_Modes[m_fireSelectorMode].ModeType;
            if (modeType != FireSelectorModeType.Safe && m_hasTriggeredUpSinceBegin)
            {
                if (UsesStickyDetonation)
                {
                    if (Bolt.CurPos == ClosedBolt.BoltPos.Forward && Chamber.IsFull && !Chamber.IsSpent)
                    {
                        if (hand.Input.TriggerPressed && m_hasTriggerReset)
                        {
                            m_hasStickTriggerDown = true;
                            m_stickyChargeUp += Time.deltaTime * 0.25f;
                            m_stickyChargeUp = Mathf.Clamp(m_stickyChargeUp, 0f, 1f);
                            if (m_stickyChargeUp > 0.05f && !m_chargeSound.isPlaying) m_chargeSound.Play();
                        }
                        else
                        {
                            if (m_chargeSound.isPlaying) m_chargeSound.Stop();
                            m_stickyChargeUp = 0f;
                        }

                        if (m_hasStickTriggerDown && (hand.Input.TriggerUp || m_stickyChargeUp >= 1f))
                        {
                            m_hasStickTriggerDown = false;
                            DropHammer();
                            EndStickyCharge();
                        }
                    }

                    return;
                }

                if (m_triggerFloat >= TriggerFiringThreshold && Bolt.CurPos == ClosedBolt.BoltPos.Forward &&
                    (m_hasTriggerReset || modeType == FireSelectorModeType.FullAuto && !UsesDualStageFullAuto ||
                     modeType == FireSelectorModeType.FullAuto && UsesDualStageFullAuto &&
                     m_triggerFloat > TriggerDualStageThreshold ||
                     modeType == FireSelectorModeType.Burst && m_CamBurst > 0 || (isOpenBolt = true)))
                {
                    DropHammer();
                    m_hasTriggerReset = false;
                    if (m_CamBurst > 0) m_CamBurst--;
                }
            }
        }

        // Token: 0x060016F6 RID: 5878 RVA: 0x000A6D62 File Offset: 0x000A5162
        public override void EndInteraction(FVRViveHand hand)
        {
            if (UsesStickyDetonation) EndStickyCharge();
            base.EndInteraction(hand);
        }

        // Token: 0x060016F7 RID: 5879 RVA: 0x000A6D7C File Offset: 0x000A517C
        private void EndStickyCharge()
        {
            m_chargeSound.Stop();
            m_stickyChargeUp = 0f;
        }

        // Token: 0x060016F8 RID: 5880 RVA: 0x000A6D94 File Offset: 0x000A5194
        private void UpdateComponents()
        {
            if (HasTrigger)
                SetAnimatedComponent(Trigger, Mathf.Lerp(Trigger_ForwardValue, Trigger_RearwardValue, m_triggerFloat),
                    TriggerInterpStyle, TriggerAxis);
            if (UsesStickyDetonation)
            {
                var t = Mathf.Clamp(m_primedBombs.Count / 8f, 0f, 1f);
                var y = Mathf.Lerp(0.56f, 0.23f, t);
                var num = Mathf.Lerp(5f, 15f, t);
                StickyScreen.material.SetTextureOffset("_IncandescenceMap", new Vector2(0f, y));
            }
        }

        // Token: 0x060016F9 RID: 5881 RVA: 0x000A6E4C File Offset: 0x000A524C
        private void UpdateDisplayRoundPositions()
        {
            var boltLerpBetweenLockAndFore = Bolt.GetBoltLerpBetweenLockAndFore();
            if (Chamber.IsFull)
            {
                Chamber.ProxyRound.position = Vector3.Lerp(RoundPos_Ejecting.position, Chamber.transform.position,
                    boltLerpBetweenLockAndFore);
                Chamber.ProxyRound.rotation = Quaternion.Slerp(RoundPos_Ejecting.rotation, Chamber.transform.rotation,
                    boltLerpBetweenLockAndFore);
            }

            if (m_proxy.IsFull)
            {
                m_proxy.ProxyRound.position = Vector3.Lerp(RoundPos_MagazinePos.position, Chamber.transform.position,
                    boltLerpBetweenLockAndFore);
                m_proxy.ProxyRound.rotation = Quaternion.Slerp(RoundPos_MagazinePos.rotation,
                    Chamber.transform.rotation, boltLerpBetweenLockAndFore);
            }
        }

        // Token: 0x060016FA RID: 5882 RVA: 0x000A6F49 File Offset: 0x000A5349
        public void ReleaseMag()
        {
            if (Magazine != null) base.EjectMag();
        }

        // Token: 0x060016FB RID: 5883 RVA: 0x000A6F62 File Offset: 0x000A5362
        public void RegisterStickyBomb(MF2_StickyBomb sb)
        {
            if (sb != null) m_primedBombs.Add(sb);
        }

        // Token: 0x060016FC RID: 5884 RVA: 0x000A6F7C File Offset: 0x000A537C
        public void Detonate()
        {
            var flag = false;
            if (m_primedBombs.Count > 0)
                for (var i = m_primedBombs.Count - 1; i >= 0; i--)
                    if (m_primedBombs[i] != null && m_primedBombs[i].m_hasStuck && !m_primedBombs[i].m_hasExploded)
                    {
                        flag = true;
                        m_primedBombs[i].Detonate();
                        m_primedBombs.RemoveAt(i);
                    }

            if (flag) SM.PlayCoreSound(FVRPooledAudioType.GenericClose, StickyDetonateSound, transform.position);
        }

        // Token: 0x060016FD RID: 5885 RVA: 0x000A703C File Offset: 0x000A543C
        public override List<FireArmRoundClass> GetChamberRoundList()
        {
            if (Chamber.IsFull && !Chamber.IsSpent)
                return new List<FireArmRoundClass>
                {
                    Chamber.GetRound().RoundClass
                };
            return null;
        }

        // Token: 0x060016FE RID: 5886 RVA: 0x000A7088 File Offset: 0x000A5488
        public override void SetLoadedChambers(List<FireArmRoundClass> rounds)
        {
            if (rounds.Count > 0) Chamber.Autochamber(rounds[0]);
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
            var key = string.Empty;
            var text = string.Empty;
            key = "HammerState";
            if (f.ContainsKey(key))
            {
                text = f[key];
                if (text == "Cocked") IsHammerCocked = true;
            }

            if (FireSelector_Modes.Length > 1)
            {
                key = "FireSelectorState";
                if (f.ContainsKey(key))
                {
                    text = f[key];
                    int.TryParse(text, out m_fireSelectorMode);
                }

                if (FireSelectorSwitch != null)
                {
                    var fireSelectorMode = FireSelector_Modes[m_fireSelectorMode];
                    SetAnimatedComponent(FireSelectorSwitch, fireSelectorMode.SelectorPosition,
                        FireSelector_InterpStyle, FireSelector_Axis);
                }

                if (FireSelectorSwitch2 != null)
                {
                    var fireSelectorMode2 = FireSelector_Modes2[m_fireSelectorMode];
                    SetAnimatedComponent(FireSelectorSwitch2, fireSelectorMode2.SelectorPosition,
                        FireSelector_InterpStyle2, FireSelector_Axis2);
                }
            }
        }

        // Token: 0x06001702 RID: 5890 RVA: 0x000A71AC File Offset: 0x000A55AC
        public override Dictionary<string, string> GetFlagDic()
        {
            var dictionary = new Dictionary<string, string>();
            var key = "HammerState";
            var value = "Uncocked";
            if (IsHammerCocked) value = "Cocked";
            dictionary.Add(key, value);
            if (FireSelector_Modes.Length > 1)
            {
                key = "FireSelectorState";
                value = m_fireSelectorMode.ToString();
                dictionary.Add(key, value);
            }

            return dictionary;
        }

        // Token: 0x02000455 RID: 1109
        [Serializable]
        public class FireSelectorMode
        {
            // Token: 0x04002D6A RID: 11626
            public float SelectorPosition;

            // Token: 0x04002D6B RID: 11627
            public FireSelectorModeType ModeType;

            // Token: 0x04002D6C RID: 11628
            public int BurstAmount = 3;
        }
    }
}