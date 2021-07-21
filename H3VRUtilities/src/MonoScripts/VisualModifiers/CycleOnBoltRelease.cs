using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using UnityEngine.Serialization;

namespace H3VRUtils.MonoScripts.VisualModifiers
{
	class CycleOnBoltRelease : MonoBehaviour
	{
		public FVRFireArmChamber chamber;
		public GameObject muzzle;
		[FormerlySerializedAs("Locs")] public List<Transform> locs;

		private int _pointer;
		private bool _wasFull;

		public void Update()
		{
			if(_wasFull && !chamber.IsFull)
			{
				_pointer++;
				if (_pointer > locs.Count) _pointer = 0;
				muzzle.transform.position = locs[_pointer].position;
				muzzle.transform.rotation = locs[_pointer].rotation;
			}
			_wasFull = chamber.IsFull;
		}
	}
}
