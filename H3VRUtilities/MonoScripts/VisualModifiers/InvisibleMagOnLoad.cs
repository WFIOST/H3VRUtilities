using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;

namespace H3VRUtils
{
	class InvisibleMagOnLoad : MonoBehaviour
	{
		public FVRFireArmMagazine magazine;

		public void Update()
		{
			if(magazine.FireArm != null)
			{
				magazine.Viz.gameObject.SetActive(false);
			}
			else
			{
				magazine.Viz.gameObject.SetActive(true);
			}
		}
	}
}
