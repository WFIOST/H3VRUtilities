using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;

namespace H3VRUtils.Weapons.Proxy
{
	class TatoWeaponProxy : FVRFireArm
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

		[Header("Fire Selector Config")]
		public FVRPhysicalObject.InterpStyle FireSelector_InterpStyle = FVRPhysicalObject.InterpStyle.Rotation;
		public FVRPhysicalObject.Axis FireSelector_Axis;
		public TatoWeaponProxy.FireSelectorMode[] FireSelector_Modes;

		[Header("Secondary Fire Selector Config")]
		public FVRPhysicalObject.InterpStyle FireSelector_InterpStyle2 = FVRPhysicalObject.InterpStyle.Rotation;
		public FVRPhysicalObject.Axis FireSelector_Axis2;
		public TatoWeaponProxy.FireSelectorMode[] FireSelector_Modes2;

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
		public AudioSource m_chargeSound;
		public Renderer StickyScreen;
		public enum FireSelectorModeType
		{
			Safe,
			Single,
			Burst,
			FullAuto,
			SuperFastBurst
		}
		[Serializable]
		public class FireSelectorMode
		{
			public float SelectorPosition;
			public TatoWeaponProxy.FireSelectorModeType ModeType;
			public int BurstAmount = 3;
		}
		public bool isOpenBolt;
	}
}
