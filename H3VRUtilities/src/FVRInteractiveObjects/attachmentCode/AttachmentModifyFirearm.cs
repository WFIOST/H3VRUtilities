using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using UnityEngine.Serialization;

namespace H3VRUtils
{
	public class AttachmentModifyFirearm : MonoBehaviour
	{
		private FVRFireArmAttachment _attachment;
		private FVRFireArm _weapon;

		public enum ActionType
		{
			Attach,
			Detach
		}
		public enum CapType
		{
			AddTo,
			SetTo
		}

		[FormerlySerializedAs("ChangesRecoil")] [Header("Recoil Modifier")]
		public bool changesRecoil;
		private FVRFireArmRecoilProfile _originalRecoil;
		public FVRFireArmRecoilProfile modifiedRecoil;

		[FormerlySerializedAs("ChangesMagCapacity")] [Header("Magazine Modifer")]
		public bool changesMagCapacity;
		private int _prevCapacity;
		[FormerlySerializedAs("CapacityModifierType")] public CapType capacityModifierType;
		[Tooltip("Keep it off unless you're sure it should apply to non-internal mags.")]
		public bool applyToNonInternalMags;
		public int setCapacityTo;

		[FormerlySerializedAs("ChangesBoltSpeed")] [Header("Bolt Speed Modifier")]
		public bool changesBoltSpeed;
		[FormerlySerializedAs("ChangesBoltSpeedForward")] public bool changesBoltSpeedForward;
		[FormerlySerializedAs("ChangesBoltSpeedRearward")] public bool changesBoltSpeedRearward;
		[FormerlySerializedAs("ChangesBoltSpeedStiffness")] public bool changesBoltSpeedStiffness;
		[FormerlySerializedAs("BoltSpeedModifierType")] public CapType boltSpeedModifierType;
		[FormerlySerializedAs("BoltSpeedForward")] public float boltSpeedForward;
		private float _prevBoltSpeedForward;
		[FormerlySerializedAs("BoltSpeedBackwards")] public float boltSpeedBackwards;
		private float _prevBoltSpeedBackwards;
		[FormerlySerializedAs("BoltSpringStiffness")] public float boltSpringStiffness;
		private float _prevBoltSpringStiffness;

//		[Header("Spread Modifier")]
		[FormerlySerializedAs("ChangesSpread")] [HideInInspector]
		public bool changesSpread;
		[HideInInspector]
		public float spreadmult;

		[FormerlySerializedAs("ChangesGrabPos")] [Header("GrabPos Modifier")]
		public bool changesGrabPos;
		[FormerlySerializedAs("NewPoseOverride")] public Transform newPoseOverride;
		private Transform _oldPoseOverride;
		[FormerlySerializedAs("NewPoseOverrideTouch")] public Transform newPoseOverrideTouch;
		private Transform _oldPoseOverrideTouch;


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
			_attachment = GetComponent<FVRFireArmAttachment>();
		}

		void FixedUpdate()
		{
			if (_attachment.curMount != null) { if (_weapon == null) OnAttach(); }
			else if (_weapon != null) OnDetach();
			if (changesSpread) ChangeSpread();
		}

		public void OnAttach()
		{
			_weapon = _attachment.curMount.Parent.GetComponent<FVRFireArm>();
			if (changesRecoil) RecoilModifier(ActionType.Attach);
			if (changesMagCapacity) MagSizeModifier(ActionType.Attach);
			if (changesBoltSpeed) BoltSpeedModifier(ActionType.Attach);
			//if (ChangesFireSelector) FireSelectorModifier(actionType.attach);
			if (changesGrabPos) ChangeGrabPos(ActionType.Attach);
		}

		public void OnDetach()
		{
			if (changesRecoil) RecoilModifier(ActionType.Detach);
			if (changesMagCapacity) MagSizeModifier(ActionType.Detach);
			if (changesBoltSpeed) BoltSpeedModifier(ActionType.Detach);
			//if (ChangesFireSelector) FireSelectorModifier(actionType.detach);
			if (changesGrabPos) ChangeGrabPos(ActionType.Detach);
			_weapon = null;
		}

		public void ChangeGrabPos(ActionType actType)
		{
			if (actType == ActionType.Attach)
			{
				_oldPoseOverride = _weapon.PoseOverride;
				_weapon.PoseOverride = newPoseOverride;
				_oldPoseOverrideTouch = _weapon.PoseOverride_Touch;
				_weapon.PoseOverride_Touch = newPoseOverrideTouch;
			}
			if (actType == ActionType.Detach)
			{
				_weapon.PoseOverride = _oldPoseOverride;
				_weapon.PoseOverride_Touch = _oldPoseOverrideTouch;
			}
		}

		public void ChangeSpread()
		{
			FVRFireArmRound rnd = null;
			if (_weapon is OpenBoltReceiver)
			{
				OpenBoltReceiver wep = _weapon as OpenBoltReceiver;
				rnd = wep.Chamber.GetRound();
			}
			else if (_weapon is ClosedBoltWeapon)
			{
				ClosedBoltWeapon wep = _weapon as ClosedBoltWeapon;
				rnd = wep.Chamber.GetRound();
			}
			else if (_weapon is Handgun)
			{
				Handgun wep = _weapon as Handgun;
				rnd = wep.Chamber.GetRound();
			}
			else if (_weapon is TubeFedShotgun)
			{
				TubeFedShotgun wep = _weapon as TubeFedShotgun;
				rnd = wep.Chamber.GetRound();
			}

			if (rnd == null) return;

			if (rnd.ThrowAngMultiplier != 1.1)
			{
				rnd.ThrowAngMultiplier = 1.1f;
				rnd.ProjectileSpread *= spreadmult;
			}
		}

		public void RecoilModifier(ActionType actType)
		{
			if (actType == ActionType.Attach)
			{
				_originalRecoil = _weapon.RecoilProfile;
				_weapon.RecoilProfile = modifiedRecoil;
			}
			if (actType == ActionType.Detach)
			{
				_weapon.RecoilProfile = _originalRecoil;
			}
		}

		public void MagSizeModifier(ActionType actType)
		{
			if (_weapon.Magazine == null) return;
			if (actType == ActionType.Attach)
			{
				if (_weapon.Magazine.IsIntegrated == true || applyToNonInternalMags)
				{
					_prevCapacity = _weapon.Magazine.m_capacity;
					if (capacityModifierType == CapType.AddTo) _weapon.Magazine.m_capacity += setCapacityTo;
					if (capacityModifierType == CapType.SetTo) _weapon.Magazine.m_capacity = setCapacityTo;
				}
			}
			if (actType == ActionType.Detach)
			{
				if (_weapon.Magazine.IsIntegrated == true || applyToNonInternalMags) _weapon.Magazine.m_capacity = _prevCapacity;
			}
		}

		public void BoltSpeedModifier(ActionType actType)
		{
			int wepType = 0;
			OpenBoltReceiver openBoltWep = null;
			ClosedBoltWeapon closedBoltWep = null;
			Handgun handgunWep = null;
			TubeFedShotgun tfWep = null;

			if (_weapon is OpenBoltReceiver)
			{
				openBoltWep = _weapon as OpenBoltReceiver;
				wepType = 1;
			}
			else if (_weapon is ClosedBoltWeapon)
			{
				closedBoltWep = _weapon as ClosedBoltWeapon;
				wepType = 2;
			}
			else if (_weapon is Handgun)
			{
				handgunWep = _weapon as Handgun;
				wepType = 3;
			}
			else if (_weapon is TubeFedShotgun)
			{
				tfWep = _weapon as TubeFedShotgun;
				wepType = 4;
			}

			float boltSpeedBack = 0;
			float boltSpeedForward = 0;
			float boltSpringStiffness = 0;

			switch (wepType)
			{
				case 1:
					boltSpeedBack = openBoltWep.Bolt.BoltSpeed_Rearward;
					boltSpeedForward = openBoltWep.Bolt.BoltSpeed_Forward;
					boltSpringStiffness = openBoltWep.Bolt.BoltSpringStiffness;
					break;
				case 2:
					boltSpeedBack = closedBoltWep.Bolt.Speed_Rearward;
					boltSpeedForward = closedBoltWep.Bolt.Speed_Forward;
					boltSpringStiffness = closedBoltWep.Bolt.SpringStiffness;
					break;
				case 3:
					boltSpeedBack = handgunWep.Slide.Speed_Forward;
					boltSpeedForward = handgunWep.Slide.Speed_Rearward;
					boltSpringStiffness = handgunWep.Slide.SpringStiffness;
					break;
			}

			if (actType == ActionType.Attach)
			{
				_prevBoltSpeedBackwards = boltSpeedBack;
				_prevBoltSpeedForward = boltSpeedForward;
				_prevBoltSpringStiffness = boltSpringStiffness;

				if (boltSpeedModifierType == CapType.SetTo)
				{
					boltSpeedBack = boltSpeedBackwards;
					boltSpeedForward = boltSpeedForward;
					boltSpringStiffness = boltSpringStiffness;
				}
				if (boltSpeedModifierType == CapType.AddTo)
				{
					boltSpeedBack += boltSpeedBackwards;
					boltSpeedForward += boltSpeedForward;
					boltSpringStiffness += boltSpringStiffness;
				}
			}
			if (actType == ActionType.Detach)
			{
				boltSpeedBack = _prevBoltSpeedBackwards;
				boltSpeedForward = _prevBoltSpeedForward;
				boltSpringStiffness = _prevBoltSpringStiffness;
			}

			if (changesBoltSpeedRearward) switch (wepType)
			{
				case 1:
					openBoltWep.Bolt.BoltSpeed_Rearward = boltSpeedBack;
					break;
				case 2:
					closedBoltWep.Bolt.Speed_Rearward = boltSpeedBack;
					break;
				case 3:
					handgunWep.Slide.Speed_Forward = boltSpeedBack;
					break;
			}
			if (changesBoltSpeedForward) switch (wepType)
			{
				case 1:
					openBoltWep.Bolt.BoltSpeed_Forward = boltSpeedForward;
					break;
				case 2:
					closedBoltWep.Bolt.Speed_Forward = boltSpeedForward;
					break;
				case 3:
					handgunWep.Slide.Speed_Rearward = boltSpeedForward;
					break;
			}
			if (changesBoltSpeedStiffness) switch (wepType)
			{
				case 1:
					openBoltWep.Bolt.BoltSpringStiffness = boltSpringStiffness;
					break;
				case 2:
					closedBoltWep.Bolt.SpringStiffness = boltSpringStiffness;
					break;
				case 3:
					handgunWep.Slide.SpringStiffness = boltSpringStiffness;
					break;
			}

		}

/*		public void FireSelectorModifier(actionType ActType)
		{
			int wepType = 0;
			OpenBoltReceiver _OpenBoltWep = null;
			ClosedBoltWeapon _ClosedBoltWep = null;
			Handgun _HandgunWep = null;
			var OpenBoltModes = new List<OpenBoltReceiver.FireSelectorMode>();
			var ClosedBoltModes = new List<ClosedBoltWeapon.FireSelectorMode>();
			var HandgunModes = new List<Handgun.FireSelectorMode>();
			OpenBoltReceiver.FireSelectorMode[] prevOpenBoltModes;
			ClosedBoltWeapon.FireSelectorMode[] prevClosedBoltModes;
			Handgun.FireSelectorMode[] prevHandgunModes;

			if (weapon is OpenBoltReceiver)
			{
				_OpenBoltWep = weapon.GetComponent<OpenBoltReceiver>();
				OpenBoltModes = new List<OpenBoltReceiver.FireSelectorMode>(_OpenBoltWep.FireSelector_Modes);
				prevOpenBoltModes = _OpenBoltWep.FireSelector_Modes;
				wepType = 1;
			}
			if (weapon is ClosedBoltWeapon)
			{
				_ClosedBoltWep = weapon.GetComponent<ClosedBoltWeapon>();
				ClosedBoltModes = new List<ClosedBoltWeapon.FireSelectorMode>(_ClosedBoltWep.FireSelector_Modes);
				prevClosedBoltModes = _ClosedBoltWep.FireSelector_Modes;
				wepType = 2;
			}
			if (weapon is Handgun)
			{
				_HandgunWep = weapon.GetComponent<Handgun>();
				HandgunModes = new List<Handgun.FireSelectorMode>(_HandgunWep.FireSelectorModes);
				prevHandgunModes = _HandgunWep.FireSelectorModes;
				wepType = 3;
			}

			if (ActType == actionType.attach)
			{
				if (FireSelectorModiferType == CapType.AddTo)
				{
					if (wepType == 1)
					{
						int convertedModeType = (int)AddedModeType;
						switch (AddedModeType)
						{
							case ClosedBoltWeapon.FireSelectorModeType.Burst:
								return;
							case ClosedBoltWeapon.FireSelectorModeType.FullAuto:
								convertedModeType = (int)OpenBoltReceiver.FireSelectorModeType.FullAuto;
								break;
							case ClosedBoltWeapon.FireSelectorModeType.SuperFastBurst:
								convertedModeType = (int)OpenBoltReceiver.FireSelectorModeType.SuperFastBurst;
								break;
						}

						var addMode = new OpenBoltReceiver.FireSelectorMode
						{
							ModeType = (OpenBoltReceiver.FireSelectorModeType)convertedModeType
						};
					}
					if (wepType == 2)
					{

					}
					if (wepType == 3)
					{

					}
				}
				if (FireSelectorModiferType == CapType.SetTo)
				{
					if (wepType == 1)
					{

					}
					if (wepType == 2)
					{

					}
					if (wepType == 3)
					{

					}
				}
			}
			if (ActType == actionType.detach)
			{
				if (wepType == 1)
				{

				}
				if (wepType == 2)
				{

				}
				if (wepType == 3)
				{

				}
			}
		}
	*/}
}
