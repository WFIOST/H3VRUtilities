using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace H3VRUtils.MonoScripts.VisualModifiers
{
	class CycleOnBoltRelease : MonoBehaviour
	{
		public FVRFireArmChamber chamber;
		public GameObject muzzle;
		public List<Transform> Locs;

		private int pointer;
		private bool wasFull;

		public void Update()
		{
			if(wasFull && !chamber.IsFull)
			{
				pointer++;
				if (pointer > Locs.Count) pointer = 0;
				muzzle.transform.position = Locs[pointer].position;
				muzzle.transform.rotation = Locs[pointer].rotation;
			}
			wasFull = chamber.IsFull;
		}
	}
}
