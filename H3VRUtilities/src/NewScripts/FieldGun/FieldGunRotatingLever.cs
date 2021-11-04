using H3VRUtils.FVRInteractiveObjects;
using UnityEngine;

namespace H3VRUtils.NewScripts
{
	public class FieldGunRotatingLever : RotatingObject
	{
		public FieldGun fieldGun;
		public enum axis { x,y,z }
		public axis rotationDir;
	}
}