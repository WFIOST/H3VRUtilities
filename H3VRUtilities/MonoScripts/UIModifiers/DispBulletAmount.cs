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
		public bool IncludeChamberRound;
		public int MinCharLength;
		private bool WasFull;
		private FVRFireArmChamber chamber;

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
			string txt = "";

			if (firearm == null)
			{
				txt = magazine.m_numRounds.ToString();
				if (AddMinCharLength)
				{
					int lengthneedtoadd = MinCharLength - txt.Length;
					for (int i = 0; i < lengthneedtoadd; i++) txt = "0" + txt;
				}

				UItext.text = txt;
			}
			else
			if (firearm.Magazine != null)
			{
				int amt = firearm.Magazine.m_numRounds;

				if (IncludeChamberRound)
				{
					if (WasFull && !chamber.IsFull)
					{
						
					}
					else
					{
						WasFull = chamber.IsFull;
						return;
					}

					WasFull = chamber.IsFull;
				}

				txt = firearm.Magazine.m_numRounds.ToString();
				if (AddMinCharLength)
				{
					int lengthneedtoadd = MinCharLength - txt.Length;
					for (int i = 0; i < lengthneedtoadd; i++) txt = "0" + txt;
				}

				UItext.text = txt;
			}
			else
			{
				if (!KeepLastRoundInfoOnNoMag)
				{
					UItext.text = textWhenNoMag;
				}
			}
		}
	}
}