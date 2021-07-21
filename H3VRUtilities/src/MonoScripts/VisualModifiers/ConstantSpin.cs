using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace H3VRUtils.MonoScripts.VisualModifiers
{
	class ConstantSpin : MonoBehaviour
	{
		public GameObject spinnything;
		public float spinrate;
		public CullOnZLoc.DirType directionofspeen;

		public void FixedUpdate()
		{
			Vector3 rot = new Vector3();
			rot[(int)directionofspeen] = spinrate;
			spinnything.transform.Rotate(rot);
		}
	}
}
