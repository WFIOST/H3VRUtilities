using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FistVR;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace H3VRUtils
{
	public class GetFireArmDeets
	{
		public static FVRFireArmChamber[] GetFireArmChamber(FVRFireArm firearm)
		{
			List<FVRFireArmChamber> Chambers = new List<FVRFireArmChamber>();
			switch (firearm)
			{
				//get BAW chambers
				case BreakActionWeapon baw:
					foreach(var barrel in baw.Barrels) Chambers.Add(barrel.Chamber);
					return Chambers.ToArray();
				
				//get derringer chambers
				case Derringer derringer:
					foreach(var barrel in derringer.Barrels) Chambers.Add(barrel.Chamber);
					return Chambers.ToArray();
				//get SAR chambers
				case SingleActionRevolver revolver:
					return revolver.Cylinder.Chambers;
				case Revolver  revolver:
					return revolver.Chambers;
			}

			//reflection time!!!!
			//get field named Chamber
			FieldInfo chamberField = firearm.GetType().GetField("Chamber");
			if (chamberField != null)
			{
				//cast Chamber field to firearm as FVRFireArmChamber
				Chambers.Add((FVRFireArmChamber)chamberField.GetValue(firearm));
				return Chambers.ToArray();
			}

			//if Chamber doesn't exist, try to get Chambers field
			chamberField = firearm.GetType().GetField("Chambers");
			if (chamberField != null)
			{
				//cast chambers field to firearm as an array of chambers
				Chambers = (List<FVRFireArmChamber>)chamberField.GetValue(firearm);
				return Chambers.ToArray();
			}
			return null;
		}

		public static FVRFirearmMovingProxyRound[] GetFireArmProxySwitch(FVRFireArm firearm, bool tryReflection = true)
		{
			List<FVRFirearmMovingProxyRound> ProxyRound = new List<FVRFirearmMovingProxyRound>();

			switch (firearm)
			{
				case Handgun f:
					ProxyRound.Add(f.m_proxy);
					break;
				case ClosedBoltWeapon f:
					ProxyRound.Add(f.m_proxy);
					break;
				case OpenBoltReceiver f:
					ProxyRound.Add(f.m_proxy);
					break;
				case LeverActionFirearm f:
					ProxyRound.Add(f.m_proxy);
					ProxyRound.Add(f.m_proxy2);
					break;
				case BoltActionRifle f:
					ProxyRound.Add(f.m_proxy);
					break;
				case TubeFedShotgun f:
					ProxyRound.Add(f.m_proxy);
					break;
				case Airgun f:
					break;
				default:
					if(tryReflection)
						ProxyRound = GetFireArmProxyReflection(firearm).ToList();
					break;
			}
			
			ProxyRound.RemoveAll(item => item == null);
			return ProxyRound.ToArray();
		}
		
		//don't call this. reflection is rather expensive; use switch. if switch doesnt work itll auto reflect
		public static FVRFirearmMovingProxyRound[] GetFireArmProxyReflection(FVRFireArm firearm)
		{
			if (firearm == null)
			{
				Debug.Log("GetFirearmDeets.GetFireArmProxy: Firearm is null!");
				return null;
			}
			List<FVRFirearmMovingProxyRound> ProxyRound = new List<FVRFirearmMovingProxyRound>();
			
			FieldInfo chamberField = firearm.GetType().GetField("m_proxy");
			FieldInfo chamberField2 = firearm.GetType().GetField("m_proxy2");
			if (chamberField != null)
			{
				var obj = chamberField.GetValue(firearm);
				//cast Chamber field to firearm as FVRFireArmChamber
				if (obj != null) ProxyRound.Add((FVRFirearmMovingProxyRound) obj);
			}
			if (chamberField2 != null)
			{
				var obj = chamberField2.GetValue(firearm);
				//cast Chamber field to firearm as FVRFireArmChamber
				if (obj != null) ProxyRound.Add((FVRFirearmMovingProxyRound) obj);
			}
			
			return ProxyRound.ToArray();
		}
	}
}