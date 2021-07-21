using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace H3VRUtils
{

	public class CullOnZLoc : MonoBehaviour
	{
		public enum CutOffType
		{
			Above,
			Below
		}
		public CutOffType cutoff;
		public double loc;
		public GameObject objTarget;
		public enum DirType
		{
			X = 0,
			Y = 1,
			Z = 2
		}
		public DirType dir;
		private MeshRenderer _objMeshRenderer;


		void Start()
		{
			_objMeshRenderer = gameObject.GetComponent<MeshRenderer>();
		}

		void Update()
		{
			switch (cutoff) {
				case CutOffType.Below:
					_objMeshRenderer.enabled = objTarget.transform.localPosition[(int)dir] > loc;
					break;
				case CutOffType.Above:
					_objMeshRenderer.enabled = objTarget.transform.localPosition[(int)dir] < loc;
					break;
			}
		}
	}
}
