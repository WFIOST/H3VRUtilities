using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using FistVR;
using H3VRUtils;

namespace H3VRUtils.MonoScripts.UIModifiers
{
	//thanks frityet for the rewrite!
	public class DispBulletAmount : MonoBehaviour
	{
		public FVRFireArm firearm;
		public FVRFireArmMagazine magazine;
		public FVRFireArmAttachment attachment;
		
		[Header("Alternate Displays")]
		public bool EnabledObjects;
		public GameObject ObjectWhenEmpty;
		public List<GameObject> Objects;
		public bool EnableAllUnderAmount;
		
		[Header("Ammo Counter Settings")]
		public Text UItext;
		public Text MaxAmmoText;
		public Text ammoTypeText;
		public bool AddMinCharLength;
		public int MinCharLength;
		
		private FVRFireArm _fa;
		private FVRFireArmMagazine _mag;

		private void GetFirearmAndMag()
		{
			if (_isFireArmNull) {
				_fa = firearm;
				_mag = firearm.Magazine;
			}
			else if (_isMagazineNull) {
				_mag = magazine;
				_fa = magazine.FireArm;
			}
			else if (_isAttachmentNull) {
				_fa = attachment.GetRootObject() as FVRFireArm;
				if(_fa != null) _mag = firearm.Magazine;
			}
		}

		private int GetAmmoCount()
		{
			int count = 0;

			FVRFireArm _firearm = _fa;
			FVRFireArmMagazine mag = _mag;

			if (mag == null) return count;
			count += mag.m_numRounds;

			if (_firearm != null)
			{
				count += (int)GetFireArmDeets.GetFireArmChamber(_firearm)?.Count(chamber => chamber.IsFull && !chamber.IsSpent);
			}
			return count;
		}
		private int GetMaxAmmoCount()
		{
			if (_mag != null)
				return _mag.m_capacity;
			return 0;
		}
		//private string Time => DateTime.Now.ToString("HH:mm");

		private string GetAmmoType()
		{
			if (_fa != null) {
				return AM.GetFullRoundName(_fa.RoundType, _fa.GetChamberRoundList()[0]);
			}

			return String.Empty;
		}

		private void Update()
		{
			GetFirearmAndMag();
			var amtAmmo = GetAmmoCount();
			string amtAmmoString = amtAmmo.ToString();
			if (AddMinCharLength) { //most certainly a faster way but idc
				int lengthneedtoadd = MinCharLength - amtAmmoString.Length;
				for (int i = 0; i < lengthneedtoadd; i++) amtAmmoString = "0" + amtAmmoString;
			}
			if(UItext != null)
				UItext.text = amtAmmo.ToString();
			if(MaxAmmoText != null)
				MaxAmmoText.text = GetMaxAmmoCount().ToString();
			if(ammoTypeText != null)
				ammoTypeText.text = GetAmmoType();
			if(EnabledObjects) SetEnabledObjects(amtAmmo);
		}

		private void SetEnabledObjects(int amt)
		{ //yoinked from old bit. TODO: rewrite this plz
			for (int i = 0; i < Objects.Count; i++) //set all to false
			{
				Objects[i].SetActive(false);
				ObjectWhenEmpty.SetActive(false);
			}

			if (firearm.Magazine == null && ObjectWhenEmpty != null) //turn on the no-mag object
			{
				ObjectWhenEmpty.SetActive(true);
			}
			else
				for (int i = 0; i < Objects.Count; i++) //now do the actual turn-ons
				{
					if (i < amt)
					{
						if (EnableAllUnderAmount)
						{
							Objects[i].SetActive(true);
						}
					}
					else if (i == amt)
					{
						Objects[i].SetActive(true);
					}
				}
		}

		private bool _isFireArmNull;
		private bool _isMagazineNull;
		private bool _isAttachmentNull;
		private void Start()
		{
			_isFireArmNull = firearm != null;
			_isMagazineNull = magazine != null;
			_isAttachmentNull = attachment != null;
			
			//cast all three to int, add
			int i   = (_isFireArmNull ? 1 : 0) 
			        + (_isMagazineNull ? 1 : 0)
					+ (_isAttachmentNull ? 1 : 0);
			if (i > 2)
				Debug.LogWarning("AmmoCounter has more than one field filled out! Is this supposed to be on a firearm, magazine, or attachment? Choose one!");
			else if (i == 0)
				Debug.LogWarning("AmmoCounter doesn't have any field filled out!");
		}
	}
}