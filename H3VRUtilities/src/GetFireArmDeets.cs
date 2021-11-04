using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FistVR;

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
					return revolver.Cylinder.Chambers.ToArray();
			}

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

		public static FVRFirearmMovingProxyRound[] GetFireArmProxy(FVRFireArm firearm)
		{
			List<FVRFirearmMovingProxyRound> ProxyRound = null;
			
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