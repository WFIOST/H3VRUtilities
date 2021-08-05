using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FistVR;
using UnityEngine;
using UnityEngine.UI;

namespace H3VRUtils
{
	public class AmmoCounter : MonoBehaviour
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
		private int AmmoCount
		{
			get
			{
				int count = 0;
				
				FVRFireArm _firearm = _fa;
				FVRFireArmMagazine mag = _mag;

				if (mag == null) return count;
				count += mag.m_numRounds;

				if (_firearm != null)
				{
					switch (_firearm)
					{
						//get BAW chambers
						case BreakActionWeapon baw:
							count += baw.Barrels.Count(barrel => barrel.Chamber.IsFull && !barrel.Chamber.IsSpent);
							break;
						
						//get derringer chambers
						case Derringer derringer:
							count += derringer.Barrels.Count(barrel =>
								barrel.Chamber.IsFull && !barrel.Chamber.IsSpent);
							break;
						
						//get SAR chambers
						case SingleActionRevolver revolver:
							count += revolver.Cylinder.Chambers.Count(chamber => chamber.IsFull && !chamber.IsSpent);
							break;
					}

					{
						//get field named Chamber
						FieldInfo chamberField = _firearm.GetType().GetField("Chamber");
						if (chamberField != null)
						{
							//cast Chamber field to firearm as FVRFireArmChamber
							var chamber = (FVRFireArmChamber)chamberField.GetValue(_firearm);
							if (chamber.IsFull && !chamber.IsSpent)
								count++;
						} else
						{
							//if Chamber doesn't exist, try to get Chambers field
							chamberField = _firearm.GetType().GetField("Chambers");
							if (chamberField != null)
							{
								//cast chambers field to firearm as an array of chambers
								var chambers = (FVRFireArmChamber[])chamberField.GetValue(_firearm);
								count += chambers.Count(chamber => chamber.IsFull && !chamber.IsSpent);
							}
						}
					}
				}
				return count;
			}
		}
		private int MaxAmmoCount
		{
			get
			{
				if (_mag != null)
					return _mag.m_capacity;
				return 0;
			}
		}
		//private string Time => DateTime.Now.ToString("HH:mm");

		private string AmmoType
		{
			get
			{
				FVRPhysicalObject root = attachment.GetRootObject();

				if (root is FVRFireArm fireArm)
					return AM.GetFullRoundName(fireArm.RoundType, fireArm.GetChamberRoundList()[0]);

				return String.Empty;
			}
		}

		private void Update()
		{
			GetFirearmAndMag();
			var amtAmmo = AmmoCount;
			string amtAmmoString = amtAmmo.ToString();
			if (AddMinCharLength) { //most certainly a faster way but idc
				int lengthneedtoadd = MinCharLength - amtAmmoString.Length;
				for (int i = 0; i < lengthneedtoadd; i++) amtAmmoString = "0" + amtAmmoString;
			}
			UItext.text = amtAmmo.ToString();
			MaxAmmoText.text = MaxAmmoCount.ToString();
			ammoTypeText.text = AmmoType;
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