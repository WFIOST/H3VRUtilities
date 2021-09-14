using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using FistVR;
using HarmonyLib;
using UnityEngine;

namespace H3VRUtilsConfig.patchers
{
	public class patch_ClosedBoltWeapon
	{
		/*//static FieldInfo f_closedboltwepUIAA = AccessTools.Field(typeof(ClosedBoltWeapon), "UpdateInputAndAnimate");
		//private static MethodInfo m_extraMethod = SymbolExtensions.GetMethodInfo(() => );
			
		[HarmonyPatch(typeof(ClosedBoltWeapon), "UpdateInputAndAnimate")]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> patch_ClosedBolt_FixHasFireSelector(IEnumerable<CodeInstruction> instructions)
		{
			var codes = new List<CodeInstruction>(instructions);
			for (var i = 0; i < codes.Count; i++)
			{
				var strOperand = codes[i].ToString();
				Debug.Log(i + ", " + strOperand);
				if (strOperand == "call static UnityEngine.Vector2 UnityEngine.Vector2::get_left()")
				{
					if (codes[i + 1].ToString() == "call static float UnityEngine.Vector2::Angle(UnityEngine.Vector2 from, UnityEngine.Vector2 to)")
					{
						
					}
				}
			}
			return codes.AsEnumerable();
		}*/
	}
}