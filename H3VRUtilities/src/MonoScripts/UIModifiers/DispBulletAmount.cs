using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using FistVR;
using UnityEngine.Serialization;

namespace H3VRUtils.MonoScripts.UIModifiers
{
	public class DispBulletAmount : MonoBehaviour
	{
		public FVRFireArm firearm;
		public FVRFireArmMagazine magazine;
		[FormerlySerializedAs("UItext")] public Text uItext;
		public string textWhenNoMag;
		[FormerlySerializedAs("KeepLastRoundInfoOnNoMag")] [Tooltip("When there is no mag, the text will remain whatever it was before.")]
		public bool keepLastRoundInfoOnNoMag;
		[FormerlySerializedAs("AddMinCharLength")] public bool addMinCharLength;
		[FormerlySerializedAs("MinCharLength")] public int minCharLength;
		[FormerlySerializedAs("EnabledObjects")] [Header("Alternate Usages")]
		public bool enabledObjects;
		[FormerlySerializedAs("ObjectWhenEmpty")] public GameObject objectWhenEmpty;
		[FormerlySerializedAs("Objects")] public List<GameObject> objects;
		[FormerlySerializedAs("EnableAllUnderAmount")] public bool enableAllUnderAmount;


		private bool _wasFull;
		private bool _wasLoaded;
		private FVRFireArmChamber _chamber;
		private string _txt;
		private int _amt;

		public void Start()
		{
			if (firearm is ClosedBoltWeapon)
			{
				ClosedBoltWeapon wep = firearm as ClosedBoltWeapon;
				_chamber = wep.Chamber;
			}
			if (firearm is OpenBoltReceiver)
			{
				OpenBoltReceiver wep = firearm as OpenBoltReceiver;
				_chamber = wep.Chamber;
			}
			if (firearm is Handgun)
			{
				Handgun wep = firearm as Handgun;
				_chamber = wep.Chamber;
			}
		}

		public void FixedUpdate()
		{
			_txt = "";

			if (firearm == null) //magcode
			{
				_amt = magazine.m_numRounds;
				SetText();
			}
			else

			//guncode
			if (firearm.Magazine != null)
			{
				_amt = firearm.Magazine.m_numRounds;

				if ((_wasFull && !_chamber.IsFull) || !_wasLoaded) //was chamber loaded but no longer, or was not loaded and now is
				{
					if (!_wasLoaded)
					{
						if (_chamber.IsFull)
						{
							_amt++; //check if mag loaded + 1
						}
					}
					SetText();
					_wasLoaded = true;
				}

				_wasFull = _chamber.IsFull;
			}
			else
			{
				if (!keepLastRoundInfoOnNoMag)
				{
					_amt = 0;
					SetText();
				}
				_wasLoaded = false;
			}
		}

		public void SetText()
		{
			_txt = _amt.ToString();
			if (uItext != null)
			{
				if (addMinCharLength)
				{
					int lengthneedtoadd = minCharLength - _txt.Length;
					for (int i = 0; i < lengthneedtoadd; i++) _txt = "0" + _txt;
				}
				uItext.text = _txt;
				if(firearm.Magazine == null)
				{
					uItext.text = textWhenNoMag;
				}
			}

			if (enabledObjects)
			{
				for (int i = 0; i < objects.Count; i++) //set all to false
				{
					objects[i].SetActive(false);
					objectWhenEmpty.SetActive(false);
				}

				if(firearm.Magazine == null && objectWhenEmpty != null) //turn on the no-mag object
				{
					objectWhenEmpty.SetActive(true);
				} else 
				for (int i = 0; i < objects.Count; i++) //now do the actual turn-ons
				{
					if (i < _amt)
					{
						if (enableAllUnderAmount)
						{
							objects[i].SetActive(true);
						}
					}
					else if (i == _amt)
					{
						objects[i].SetActive(true);
					}
				}
			}
		}
	}
}