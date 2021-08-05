using System;
using System.Linq;
using System.Reflection;
using FistVR;
using UnityEngine;
using UnityEngine.UI;

namespace H3VRUtils
{
	public class AmmoCounter : FVRFireArmAttachment
	{
		[Header("Ammo Counter Settings")]
		public Text ammoText;
		public Text ammoTypeText;
		public Text timeText;

		private int AmmoCount
		{
			get
			{
				var count = 0;
				
				FVRPhysicalObject root = GetRootObject();

				if (root == null) return count;

				if (root is FVRFireArm fireArm)
				{
					if (fireArm.Magazine == null) count = 0;
					else count += fireArm.Magazine.m_numRounds;

					if (fireArm.Clip == null) count = 0;
					else count += fireArm.Clip.m_numRounds;

					switch (fireArm)
					{
						case BreakActionWeapon baw:
							count += baw.Barrels.Count(barrel => barrel.Chamber.IsFull && !barrel.Chamber.IsSpent);
							break;
						
						case Derringer derringer:
							count += derringer.Barrels.Count(barrel => barrel.Chamber.IsFull && !barrel.Chamber.IsSpent);
							break;
						   
						case SingleActionRevolver revolver:
							count += revolver.Cylinder.Chambers.Count(chamber => chamber.IsFull && !chamber.IsSpent);
							break;
						   
					}

					{
						FieldInfo chamberField = fireArm.GetType().GetField("Chamber");
						if (chamberField != null)
						{
							var chamber = chamberField.GetValue(fireArm) as FVRFireArmChamber;
							if (chamber.IsFull && !chamber.IsSpent)
								count++;
						}
					}
					
					{
						FieldInfo chamberField = fireArm.GetType().GetField("Chambers");
						if (chamberField != null)
						{
							var chambers = chamberField.GetValue(fireArm) as FVRFireArmChamber[];

							count += chambers.Count(chamber => chamber.IsFull && !chamber.IsSpent);
						}
					}
				}

				return count;
			}
		}

		private string Time => DateTime.Now.ToString("HH:mm");

		private string AmmoType
		{
			get
			{
				FVRPhysicalObject root = GetRootObject();

				if (root is FVRFireArm fireArm)
					return AM.GetFullRoundName(fireArm.RoundType, fireArm.GetChamberRoundList()[0]);

				return String.Empty;
			}
		}

		private void FixedUpdate()
		{
			timeText.text	   = Time;
			ammoText.text	   = AmmoCount.ToString();
			ammoTypeText.text   = AmmoType;
		}
	}
}