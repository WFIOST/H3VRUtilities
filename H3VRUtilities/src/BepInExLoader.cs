using BepInEx;
using UnityEngine;

namespace H3VRUtils
{
	[BepInPlugin("dll.wfiost.h3vrutilitieslib", "H3VR Utilities Library", "8.9.3")]
	[BepInDependency("dll.wfiost.h3vrutilities", BepInDependency.DependencyFlags.HardDependency)]
	[BepInProcess("h3vr.exe")]
	public class BepInExLoader_Lib : BaseUnityPlugin
	{
		
	}
}