using System;
using System.Collections.Generic;
using UnityEngine;
using FistVR;

namespace H3VRUtils.Weapons.stub
{
	public class TatoWeapon : FVRFireArm
	{
		[Header("ClosedBoltWeapon Config")]
		public bool HasFireSelectorButton = true;
		public bool HasMagReleaseButton = true;
		public bool HasBoltReleaseButton = true;
		public bool HasBoltCatchButton = true;
		public bool HasHandle = true;
		[Header("Component Connections")]
		public ClosedBolt Bolt;
		public ClosedBoltHandle Handle;
		public FVRFireArmChamber Chamber;
		public Transform Trigger;
		public Transform FireSelectorSwitch;
		public Transform FireSelectorSwitch2;
		[Header("Round Positions")]
		public Transform RoundPos_Ejecting;
		public Transform RoundPos_Ejection;
		public Transform RoundPos_MagazinePos;
		private FVRFirearmMovingProxyRound m_proxy;
		public Vector3 EjectionSpeed = new Vector3(4f, 2.5f, -1.2f);
		public Vector3 EjectionSpin = new Vector3(20f, 180f, 30f);
		public bool UsesDelinker;
		public ParticleSystem DelinkerSystem;
		[Header("Trigger Config")]
		public bool HasTrigger;
		public float TriggerFiringThreshold = 0.8f;
		public float TriggerResetThreshold = 0.4f;
		public float TriggerDualStageThreshold = 0.95f;
		public float Trigger_ForwardValue;
		public float Trigger_RearwardValue;
		public FVRPhysicalObject.Axis TriggerAxis;
		public FVRPhysicalObject.InterpStyle TriggerInterpStyle = FVRPhysicalObject.InterpStyle.Rotation;
		public bool UsesDualStageFullAuto;
		private float m_triggerFloat;
		private bool m_hasTriggerReset;
		private int m_CamBurst;
		private int m_fireSelectorMode;
		[Header("Fire Selector Config")]
		public FVRPhysicalObject.InterpStyle FireSelector_InterpStyle = FVRPhysicalObject.InterpStyle.Rotation;
		public FVRPhysicalObject.Axis FireSelector_Axis;
		public TatoWeapon.FireSelectorMode[] FireSelector_Modes;
		[Header("Secondary Fire Selector Config")]
		public FVRPhysicalObject.InterpStyle FireSelector_InterpStyle2 = FVRPhysicalObject.InterpStyle.Rotation;
		public FVRPhysicalObject.Axis FireSelector_Axis2;
		public TatoWeapon.FireSelectorMode[] FireSelector_Modes2;
		[Header("SpecialFeatures")]
		public bool EjectsMagazineOnEmpty;
		public bool BoltLocksWhenNoMagazineFound;
		public bool DoesClipEntryRequireBoltBack = true;
		public bool UsesMagMountTransformOverride;
		public Transform MagMountTransformOverride;
		[Header("StickySystem")]
		public bool UsesStickyDetonation;
		public AudioEvent StickyDetonateSound;
		public Transform StickyTrigger;
		public Vector2 StickyRotRange = new Vector2(0f, 20f);
		private float m_stickyChargeUp;
		public AudioSource m_chargeSound;
		public Renderer StickyScreen;
		[HideInInspector]
		public bool IsMagReleaseButtonHeld;
		[HideInInspector]
		public bool IsBoltReleaseButtonHeld;
		[HideInInspector]
		public bool IsBoltCatchButtonHeld;
		private float m_timeSinceFiredShot = 1f;
		private bool m_hasStickTriggerDown;
		private List<MF2_StickyBomb> m_primedBombs = new List<MF2_StickyBomb>();
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
