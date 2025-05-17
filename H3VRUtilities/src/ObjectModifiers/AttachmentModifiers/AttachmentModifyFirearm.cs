using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace H3VRUtils
{
	class AttachmentModifyFirearm : MonoBehaviour
	{
		private FVRFireArmAttachment attachment;
		private FVRFireArm weapon;

		public enum actionType
		{
			attach,
			detach
		}
		public enum CapType
		{
			AddTo,
			SetTo
		}

		[Header("Recoil Modifier")]
		public bool ChangesRecoil;
		private FVRFireArmRecoilProfile originalRecoil;
		public FVRFireArmRecoilProfile modifiedRecoil;

		[Header("Magazine Modifer")]
		public bool ChangesMagCapacity;
		private int prevCapacity;
		public CapType CapacityModifierType;
		[Tooltip("Keep it off unless you're sure it should apply to non-internal mags.")]
		public bool applyToNonInternalMags;
		public int setCapacityTo;

		[Header("Bolt Speed Modifier")]
		public bool ChangesBoltSpeed;
		public bool ChangesBoltSpeedForward;
		public bool ChangesBoltSpeedRearward;
		public bool ChangesBoltSpeedStiffness;
		public CapType BoltSpeedModifierType;
		public float BoltSpeedForward;
		private float prevBoltSpeedForward;
		public float BoltSpeedBackwards;
		private float prevBoltSpeedBackwards;
		public float BoltSpringStiffness;
		private float prevBoltSpringStiffness;

//		[Header("Spread Modifier")]
		[HideInInspector]
		public bool ChangesSpread;
		[HideInInspector]
		public float spreadmult;

		[Header("GrabPos Modifier")]
		public bool ChangesGrabPos;
		public Transform NewPoseOverride;
		private Vector3 oldPoseOverridePos;
		private Quaternion oldPoseOverrideRot;
		public Transform NewPoseOverrideTouch;
		private Vector3 oldPoseOverrideTouchPos;
		private Quaternion oldPoseOverrideTouchRot;
		/*[Header("Weapon Fire Selector Modifier")]
		public bool ChangesFireSelector;
		public CapType FireSelectorModiferType;
		[Tooltip("Leave blank to not change fire selector.")]
		public GameObject FireSelector;

		public float AddedSelectorPosition;
		[Tooltip("burst doesn't apply to open bolts, superfastburst doesn't apply to handguns.")]
		public ClosedBoltWeapon.FireSelectorModeType AddedModeType;
		public int AddedBurstAmount = 3;*/


		public void Start()
		{
			attachment = GetComponent<FVRFireArmAttachment>();
		}

		void FixedUpdate()
		{
			if (attachment.curMount != null) { if (weapon == null) OnAttach(); }
			else if (weapon != null) OnDetach();
			if (ChangesSpread) ChangeSpread();
		}

		void Update()
		{
			
		}

		public void OnAttach()
		{
			weapon = attachment.curMount.Parent.GetComponent<FVRFireArm>();
			if (ChangesRecoil) RecoilModifier(actionType.attach);
			if (ChangesMagCapacity) MagSizeModifier(actionType.attach);
			if (ChangesBoltSpeed) BoltSpeedModifier(actionType.attach);
			//if (ChangesFireSelector) FireSelectorModifier(actionType.attach);
			if (ChangesGrabPos) ChangeGrabPos(actionType.attach);
		}

		public void OnDetach()
		{
			if (ChangesRecoil) RecoilModifier(actionType.detach);
			if (ChangesMagCapacity) MagSizeModifier(actionType.detach);
			if (ChangesBoltSpeed) BoltSpeedModifier(actionType.detach);
			//if (ChangesFireSelector) FireSelectorModifier(actionType.detach);
			if (ChangesGrabPos) ChangeGrabPos(actionType.detach);
			weapon = null;
		}

		public void ChangeGrabPos(actionType ActType)
		{
			if (ActType == actionType.attach)
			{
				//Set Old Override transform
				oldPoseOverridePos = weapon.PoseOverride.localPosition;
				oldPoseOverrideRot = weapon.PoseOverride.localRotation;
				//Set new Override transform
				weapon.PoseOverride.position = NewPoseOverride.position;
				weapon.PoseOverride.rotation = NewPoseOverride.rotation;
				//Set Old OverrideTouch transform
				oldPoseOverrideTouchPos = weapon.PoseOverride_Touch.localPosition;
				oldPoseOverrideTouchRot = weapon.PoseOverride_Touch.localRotation;
				//Set new OverrideTouch transform
				weapon.PoseOverride_Touch.position = NewPoseOverrideTouch.position;
				weapon.PoseOverride_Touch.rotation = NewPoseOverrideTouch.rotation;
			}
			if (ActType == actionType.detach)
			{
				//Set old Override back
				weapon.PoseOverride.localPosition = oldPoseOverridePos;
				weapon.PoseOverride.localRotation = oldPoseOverrideRot;
				//Set old OverrideTouch back
				weapon.PoseOverride_Touch.localPosition = oldPoseOverrideTouchPos;
				weapon.PoseOverride_Touch.localRotation = oldPoseOverrideTouchRot;
			}
		}

		public void ChangeSpread()
		{
			FVRFireArmRound rnd = null;
			if (weapon is OpenBoltReceiver)
			{
				var wep = weapon as OpenBoltReceiver;
				rnd = wep.Chamber.GetRound();
			}
			else if (weapon is ClosedBoltWeapon)
			{
				var wep = weapon as ClosedBoltWeapon;
				rnd = wep.Chamber.GetRound();
			}
			else if (weapon is Handgun)
			{
				var wep = weapon as Handgun;
				rnd = wep.Chamber.GetRound();
			}
			else if (weapon is TubeFedShotgun)
			{
				var wep = weapon as TubeFedShotgun;
				rnd = wep.Chamber.GetRound();
			}

			if (rnd == null) return;

			if (rnd.ThrowAngMultiplier != 1.1)
			{
				rnd.ThrowAngMultiplier = 1.1f;
				rnd.ProjectileSpread *= spreadmult;
			}
		}

		public void RecoilModifier(actionType ActType)
		{
			if (ActType == actionType.attach)
			{
				originalRecoil = weapon.RecoilProfile;
				weapon.RecoilProfile = modifiedRecoil;
			}
			if (ActType == actionType.detach)
			{
				weapon.RecoilProfile = originalRecoil;
			}
		}

		public void MagSizeModifier(actionType ActType)
		{
			if (weapon.Magazine == null) return;
			if (ActType == actionType.attach)
			{
				if (weapon.Magazine.IsIntegrated == true || applyToNonInternalMags)
				{
					prevCapacity = weapon.Magazine.m_capacity;
					if (CapacityModifierType == CapType.AddTo) weapon.Magazine.m_capacity += setCapacityTo;
					if (CapacityModifierType == CapType.SetTo) weapon.Magazine.m_capacity = setCapacityTo;
					Array.Resize(ref weapon.Magazine.LoadedRounds, weapon.Magazine.m_capacity);
				}
			}
			if (ActType == actionType.detach)
			{
				if (weapon.Magazine.IsIntegrated == true || applyToNonInternalMags) weapon.Magazine.m_capacity = prevCapacity;
			}
		}

		public void BoltSpeedModifier(actionType ActType)
		{
			int wepType = 0;
			OpenBoltReceiver _OpenBoltWep = null;
			ClosedBoltWeapon _ClosedBoltWep = null;
			Handgun _HandgunWep = null;
			TubeFedShotgun _TFWep = null;

			if (weapon is OpenBoltReceiver)
			{
				_OpenBoltWep = weapon as OpenBoltReceiver;
				wepType = 1;
			}
			else if (weapon is ClosedBoltWeapon)
			{
				_ClosedBoltWep = weapon as ClosedBoltWeapon;
				wepType = 2;
			}
			else if (weapon is Handgun)
			{
				_HandgunWep = weapon as Handgun;
				wepType = 3;
			}
			else if (weapon is TubeFedShotgun)
			{
				_TFWep = weapon as TubeFedShotgun;
				wepType = 4;
			}

			float _boltSpeedBack = 0;
			float _boltSpeedForward = 0;
			float _boltSpringStiffness = 0;

			switch (wepType)
			{
				case 1:
					_boltSpeedBack = _OpenBoltWep.Bolt.BoltSpeed_Rearward;
					_boltSpeedForward = _OpenBoltWep.Bolt.BoltSpeed_Forward;
					_boltSpringStiffness = _OpenBoltWep.Bolt.BoltSpringStiffness;
					break;
				case 2:
					_boltSpeedBack = _ClosedBoltWep.Bolt.Speed_Rearward;
					_boltSpeedForward = _ClosedBoltWep.Bolt.Speed_Forward;
					_boltSpringStiffness = _ClosedBoltWep.Bolt.SpringStiffness;
					break;
				case 3:
					_boltSpeedBack = _HandgunWep.Slide.Speed_Rearward;
					_boltSpeedForward = _HandgunWep.Slide.Speed_Forward;
					_boltSpringStiffness = _HandgunWep.Slide.SpringStiffness;
					break;
			}

			if (ActType == actionType.attach)
			{
				prevBoltSpeedBackwards = _boltSpeedBack;
				prevBoltSpeedForward = _boltSpeedForward;
				prevBoltSpringStiffness = _boltSpringStiffness;

				if (BoltSpeedModifierType == CapType.SetTo)
				{
					_boltSpeedBack = BoltSpeedBackwards;
					_boltSpeedForward = BoltSpeedForward;
					_boltSpringStiffness = BoltSpringStiffness;
				}
				if (BoltSpeedModifierType == CapType.AddTo)
				{
					_boltSpeedBack += BoltSpeedBackwards;
					_boltSpeedForward += BoltSpeedForward;
					_boltSpringStiffness += BoltSpringStiffness;
				}
			}
			if (ActType == actionType.detach)
			{
				_boltSpeedBack = prevBoltSpeedBackwards;
				_boltSpeedForward = prevBoltSpeedForward;
				_boltSpringStiffness = prevBoltSpringStiffness;
			}

			if (ChangesBoltSpeedRearward) switch (wepType)
			{
				case 1:
					_OpenBoltWep.Bolt.BoltSpeed_Rearward = _boltSpeedBack;
					break;
				case 2:
					_ClosedBoltWep.Bolt.Speed_Rearward = _boltSpeedBack;
					break;
				case 3:
					_HandgunWep.Slide.Speed_Rearward = _boltSpeedBack;
					break;
			}
			if (ChangesBoltSpeedForward) switch (wepType)
			{
				case 1:
					_OpenBoltWep.Bolt.BoltSpeed_Forward = _boltSpeedForward;
					break;
				case 2:
					_ClosedBoltWep.Bolt.Speed_Forward = _boltSpeedForward;
					break;
				case 3:
					_HandgunWep.Slide.Speed_Forward = _boltSpeedForward;
					break;
			}
			if (ChangesBoltSpeedStiffness) switch (wepType)
			{
				case 1:
					_OpenBoltWep.Bolt.BoltSpringStiffness = _boltSpringStiffness;
					break;
				case 2:
					_ClosedBoltWep.Bolt.SpringStiffness = _boltSpringStiffness;
					break;
				case 3:
					_HandgunWep.Slide.SpringStiffness = _boltSpringStiffness;
					break;
			}
		}
	}
}
