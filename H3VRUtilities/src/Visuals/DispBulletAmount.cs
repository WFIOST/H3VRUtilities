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
		[Header("Only fill out one of these.")]
		public FVRFireArm firearm;
		public FVRFireArmMagazine magazine;
		public FVRFireArmAttachment attachment;
		
		[Header("Ammo Counter Settings")]
		[Tooltip("Text to display ammo left in the gun.")]
		public Text UItext;
		[Tooltip("Text to display maximum ammo.")]
		public Text MaxAmmoText;
		[Tooltip("Text to display the type of ammo.")]
		public Text ammoTypeText;
		[Tooltip("Adds a minimum character count. See tooltip for MinCharLength for more.")]
		public bool AddMinCharLength;
		[Tooltip("i.e if MinCharLength is 2 and the amount of rounds left is 5, it will display 05 instead of 5.")]
		public int MinCharLength;
		
		[Tooltip("Will not instantly be the correct amount of rounds, but will tick up/down until it is.")]
		public bool enableDispLerp;
		[Tooltip("MaxAmmo will also lerp according to the DispLerpAmt")]
		public bool enableLerpForMaxAmmo;
		[Tooltip("EnabledObjects will also lerp according to the DispLerpAmt")]
		public bool enableLerpForEnabledObjects;
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
		[Tooltip("Enables all objects under the round count. Enables the 0th, 1st, 2nd, 3rd objects if there are 3 rounds left, and so on.")]
		public bool EnableAllUnderAmount;
		[Tooltip("Overrides Objects. #0 means Objects' #0 displays from 0% to #0%, #1 means Objects' #1 displays from #0% to #1%, etc- written normally, not mathmatically (e.g 57.32)")]
		public bool EnableBasedOnPercentage;
		public List<float> ObjectPercentages;
		
		private FVRFireArm _fa;
		private FVRFireArmMagazine _mag;
		private FVRFirearmMovingProxyRound[] proxies;
		private FVRFireArmChamber[] chambers;
		private int lastAmountOfBullets;
		private int lastAmountOfMaxBullets;
		private int lastAmountOfBulletsForEnableObjects; //this is now a game to get the longest var name
		
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
			//get proxy/chamber deets
			if (_fa == null)
			{
				chambers = null;
				proxies = null;
			}
			else
			{
				if (chambers == null) chambers = GetFireArmDeets.GetFireArmChamber(_fa);
				proxies = GetFireArmDeets.GetFireArmProxySwitch(_fa);
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
				count += (int)chambers?.Count(chamber => chamber.IsFull && !chamber.IsSpent);
				foreach (var proxy in proxies) if (proxy.IsFull) count++; //if proxy aint empty
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
			GetFirearmAndMag(); //update firearm and mag if needed
			
			if (_isUItextNotNull) //set ammo text
			{
				string amountOfAmmoString = CalculateAmmoCounterAmount(GetAmmoCount(), ref lastAmountOfBullets,
					enableDispLerp, DispLerpAmt, AddMinCharLength, MinCharLength);
				UItext.text = amountOfAmmoString; //set uitext
			}
			if (_isMaxAmmoTextNotNull) //set max ammo text
			{
				string amountOfAmmoString = CalculateAmmoCounterAmount(GetMaxAmmoCount(), ref lastAmountOfMaxBullets,
					enableLerpForMaxAmmo, DispLerpAmt, AddMinCharLength, MinCharLength);
				MaxAmmoText.text = amountOfAmmoString; //set maxammo
			}
			if(_isammoTypeTextNotNull) ammoTypeText.text = GetAmmoType();
			if (EnabledObjects)
			{
				int amountOfAmmo = int.Parse(CalculateAmmoCounterAmount(GetAmmoCount(), ref lastAmountOfBulletsForEnableObjects,
					enableLerpForEnabledObjects, DispLerpAmt));
				if(EnableBasedOnPercentage) SetEnabledObjectsPercentage(amountOfAmmo);
				else SetEnabledObjects(amountOfAmmo);
			}
		}

		#region Function Shortcuts

		public static string CalculateAmmoCounterAmount(
			int currentammo, ref int lastammo,
			bool lerp = false, float lerpAmt = 0, bool pad = false, int padAmt = 0)
		{
			//lerp ammo amount
			if(lerp) currentammo = LerpInt(lastammo, currentammo, lerpAmt);
			//make string of amtofammo, pad it
			string amountOfAmmoString = currentammo.ToString();
			if (pad) amountOfAmmoString = PadStringNumberToAmt(amountOfAmmoString, padAmt);
			lastammo = currentammo;
			return amountOfAmmoString;
		}
		
		public static string PadStringNumberToAmt(string str, int minCharLength)
		{
			//most certainly a faster way but idc
			int lengthneedtoadd = minCharLength - str.Length; //calc 0s needed to add for string
			for (int i = 0; i < lengthneedtoadd; i++) str = "0" + str;
			return str;
		}

		public static int LerpInt(int a, int b, float lerp)
		{
			//CeilToIntOverride is here because otherwise the lerp would never result in a 0
			//(only if lerp was 1 would CeilToInt return 0 normally)
			bool COIoverride = a == 0;
			a = Mathf.CeilToInt(Mathf.Lerp(a, b, lerp));
			if (a == 1 && COIoverride) a = 0;
			return a;
		}
		#endregion

		private void SetEnabledObjects(int amt)
		{ //yoinked from old bit. TODO: rewrite this plz
			
			amt = Mathf.Clamp(amt, 0, Objects.Count - 1);
			
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

		private void SetEnabledObjectsPercentage(int amt)
		{ //yoinked from old bit. TODO: rewrite this plz
			
			amt = Mathf.Clamp(amt, 0, Objects.Count - 1);
			int maxamt = GetMaxAmmoCount();
			float per = (float)amt / (float)maxamt;
			
			for (int i = 0; i < Objects.Count; i++) //set all to false
			{
				Objects[i].SetActive(false);
				ObjectWhenEmpty.SetActive(false);
			}
			
			if (_mag == null && ObjectWhenEmpty != null) //turn on the no-mag object
			{
				ObjectWhenEmpty.SetActive(true);
			}
			else for (int i = 0; i < Objects.Count; i++)
			{
				//if anyone complains abt var names im naming em a/b/c. you've been warned.
				float peropp = ObjectPercentages[i] / 100;
				float peropb = 0;
				if(i > 0){ peropb = ObjectPercentages[i - 1] / 100; }

				if (per > peropp)
					if (per > peropb)
					{
						if (EnableAllUnderAmount) Objects[i].SetActive(true);
					} else Objects[i].SetActive(true);
			}
		}
		#region Start stuff
		private void Start()
		{
			NullCheck();
			SafetyCheck();
		}
		
		private bool _isFireArmNotNull;
		private bool _isMagazineNotNull;
		private bool _isAttachmentNotNull;
		private bool _isUItextNotNull;
		private bool _isMaxAmmoTextNotNull;
		private bool _isammoTypeTextNotNull;

		public void NullCheck() //if for some reason some foreign script wants to modify a null param just call this
		{
			_isammoTypeTextNotNull = ammoTypeText != null;
			_isMaxAmmoTextNotNull = MaxAmmoText != null;
			_isUItextNotNull = UItext != null;
			_isFireArmNotNull = firearm != null;
			_isMagazineNotNull = magazine != null;
			_isAttachmentNotNull = attachment != null;
		}

		private void SafetyCheck()
		{
			//cast all three to int, add. also this is dumb lol
			int i   = (_isFireArmNotNull ? 1 : 0) 
			          + (_isMagazineNotNull ? 1 : 0)
			          + (_isAttachmentNotNull ? 1 : 0);
			if (i > 1)
				Debug.LogWarning("AmmoCounter has more than one field filled out! Is this supposed to be on a firearm, magazine, or attachment? Choose one!");
			else if (i == 0)
				Debug.LogWarning("AmmoCounter doesn't have any field filled out!");
		}
		#endregion
	}
}