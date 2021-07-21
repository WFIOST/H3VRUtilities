using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;

namespace H3VRUtils
{
	public class InvisibleMagOnLoad : MonoBehaviour
	{
		public FVRFireArmMagazine magazine;

		public void Update()
		{
			magazine.Viz.gameObject.SetActive(magazine.FireArm is null);
		}
	}
}
