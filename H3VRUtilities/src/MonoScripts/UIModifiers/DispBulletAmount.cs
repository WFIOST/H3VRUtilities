using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using FistVR;

namespace H3VRUtils.MonoScripts.UIModifiers
{
	public class DispBulletAmount : MonoBehaviour
	{
		public FVRFireArm firearm;
		public FVRFireArmMagazine magazine;
		public Text UItext;
		public string textWhenNoMag;
		[Tooltip("When there is no mag, the text will remain whatever it was before.")]
		public bool KeepLastRoundInfoOnNoMag;
		public bool AddMinCharLength;
		public int MinCharLength;
		[Header("Alternate Usages")]
		public bool EnabledObjects;
		public GameObject ObjectWhenEmpty;
		public List<GameObject> Objects;
		public bool EnableAllUnderAmount;


		private bool WasFull;
		private bool WasLoaded;
		private FVRFireArmChamber chamber;
		private string txt;
		private int amt;

		public void Start()
		{
			if (firearm is ClosedBoltWeapon)
			{
				var wep = firearm as ClosedBoltWeapon;
				chamber = wep.Chamber;
			}
			if (firearm is OpenBoltReceiver)
			{
				var wep = firearm as OpenBoltReceiver;
				chamber = wep.Chamber;
			}
			if (firearm is Handgun)
			{
				var wep = firearm as Handgun;
				chamber = wep.Chamber;
			}
		}

		public void FixedUpdate()
		{
			txt = "";

			if (firearm == null) //magcode
			{
				amt = magazine.m_numRounds;
				SetText();
			}
			else

			//guncode
			if (firearm.Magazine != null)
			{
				amt = firearm.Magazine.m_numRounds;

				if ((WasFull && !chamber.IsFull) || !WasLoaded) //was chamber loaded but no longer, or was not loaded and now is
				{
					if (!WasLoaded)
					{
						if (chamber.IsFull)
						{
							amt++; //check if mag loaded + 1
						}
					}
					SetText();
					WasLoaded = true;
				}

				WasFull = chamber.IsFull;
			}
			else
			{
				if (!KeepLastRoundInfoOnNoMag)
				{
					amt = 0;
					SetText();
				}
				WasLoaded = false;
			}
		}

		public void SetText()
		{
			txt = amt.ToString();
			if (UItext != null)
			{
				if (AddMinCharLength)
				{
					int lengthneedtoadd = MinCharLength - txt.Length;
					for (int i = 0; i < lengthneedtoadd; i++) txt = "0" + txt;
				}
				UItext.text = txt;
				if(firearm.Magazine == null)
				{
					UItext.text = textWhenNoMag;
				}
			}

			if (EnabledObjects)
			{
				for (int i = 0; i < Objects.Count; i++) //set all to false
				{
					Objects[i].SetActive(false);
					ObjectWhenEmpty.SetActive(false);
				}

				if(firearm.Magazine == null && ObjectWhenEmpty != null) //turn on the no-mag object
				{
					ObjectWhenEmpty.SetActive(true);
				} else 
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
		}
	}
}