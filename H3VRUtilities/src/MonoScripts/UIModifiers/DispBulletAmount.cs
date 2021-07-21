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
		private string txt;
		private int _amt;

		public void Start()
		{
			switch (firearm)
			{
				case ClosedBoltWeapon weapon:
				{
					_chamber = weapon.Chamber;
					break;
				}
				case OpenBoltReceiver receiver:
				{
					_chamber = receiver.Chamber;
					break;
				}
				case Handgun handgun:
				{
					_chamber = handgun.Chamber;
					break;
				}
			}
		}

		public void FixedUpdate()
		{
			txt = "";

			if (firearm is null) //magcode
			{
				_amt = magazine.m_numRounds;
				SetText();
			}
			else

			//guncode
			if (firearm.Magazine is not null)
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
			txt = _amt.ToString();
			if (uItext is not null)
			{
				if (addMinCharLength)
				{
					int lengthneedtoadd = minCharLength - txt.Length;
					for (var i = 0; i < lengthneedtoadd; i++) txt = "0" + txt;
				}
				uItext.text = txt;
				if(firearm.Magazine is null)
				{
					uItext.text = textWhenNoMag;
				}
			}

			if (enabledObjects)
			{
				foreach (GameObject obj in objects)
				{
					obj.SetActive(false);
					objectWhenEmpty.SetActive(false);
				}

				if(firearm.Magazine is null && objectWhenEmpty is not null) //turn on the no-mag object
				{
					objectWhenEmpty.SetActive(true);
				} 
				else
				{
					for (var i = 0; i < objects.Count; i++) //now do the actual turn-ons
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
}