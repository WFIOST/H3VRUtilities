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

		[Header("Ammo Counter Settings")]
		public Text UItext;
		public Text MaxAmmoText;
		public Text ammoTypeText;
		public bool AddMinCharLength;
		public int MinCharLength;

		public bool enableDispLerp;
		[Tooltip("From 0-1. The % amount moved towards its correct amount every 50th of a second.")]
		[Range(0f, 1f)]
		public float DispLerpAmt;

		[Header("Alternate Displays")]
		[Tooltip("Enables enabling/disabling objects based on rounds left in mag.")]
		public bool EnabledObjects;
		[Tooltip("Object enabled when there is no magazine.")]
		public GameObject ObjectWhenEmpty;
		[Tooltip("Element no. corresponds to rounds left. 0 means no rounds, 1 means one round. Enables the 5th object if there are 5 rounds, and so on.")]
		public List<GameObject> Objects;
		[Tooltip("Enables all objects under the round count. Enables the 0nd, 1st, 2nd, 3rd objects if there are 3 rounds left, and so on.")]
		public bool EnableAllUnderAmount;
		
		private FVRFireArm _fa;
		private FVRFireArmMagazine _mag;
		private int bulletamts;

		private void GetFirearmAndMag()
		{
			if (_isFireArmNotNull) {
				_fa = firearm;
				_mag = firearm.Magazine;
			}
			else if (_isMagazineNotNull) {
				_mag = magazine;
				_fa = magazine.FireArm;
			}
			else if (_isAttachmentNotNull) {
				if (attachment.curMount != null) //if attached to a mount
				{
					if (_fa == null) //check first if _fa is not cached
					{
						//if it isn't, cache it
						_fa = attachment.curMount.Parent as FVRFireArm;
					}
				}
				else _fa = null; //if not attached to a mount, there is no firearm
				if (_fa != null) _mag = _fa.Magazine;
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
				if (GetFireArmDeets.GetFireArmProxy(_firearm) != null)
				{
					count++;
				}
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
			
			if (enableDispLerp)
			{
				amtAmmo = Mathf.CeilToInt(Mathf.Lerp(bulletamts, amtAmmo, DispLerpAmt));
			}
			
			bulletamts = amtAmmo;
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
			
			if (_mag == null && ObjectWhenEmpty != null) //turn on the no-mag object
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

		private bool _isFireArmNotNull;
		private bool _isMagazineNotNull;
		private bool _isAttachmentNotNull;
		private void Start()
		{
			_isFireArmNotNull = firearm != null;
			_isMagazineNotNull = magazine != null;
			_isAttachmentNotNull = attachment != null;
			
			//cast all three to int, add
			int i   = (_isFireArmNotNull ? 1 : 0) 
			        + (_isMagazineNotNull ? 1 : 0)
					+ (_isAttachmentNotNull ? 1 : 0);
			if (i > 2)
				Debug.LogWarning("AmmoCounter has more than one field filled out! Is this supposed to be on a firearm, magazine, or attachment? Choose one!");
			else if (i == 0)
				Debug.LogWarning("AmmoCounter doesn't have any field filled out!");
		}
	}
}