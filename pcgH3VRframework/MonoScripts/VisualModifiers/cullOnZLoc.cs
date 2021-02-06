using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace H3VRUtils
{

	public class cullOnZLoc : MonoBehaviour
	{
		public enum cutOffType
		{
			above,
			below
		}
		public cutOffType cutoff;
		public double loc;
		public GameObject objTarget;
		public enum dirType
		{
			x = 0,
			y = 1,
			z = 2
		}
		public dirType dir;
		private MeshRenderer objMeshRenderer;


		void Start()
		{
			objMeshRenderer = gameObject.GetComponent<MeshRenderer>();
		}

		void Update()
		{
			switch (cutoff) {
				case cutOffType.below:
					objMeshRenderer.enabled = objTarget.transform.localPosition[(int)dir] > loc;
					break;
				case cutOffType.above:
					objMeshRenderer.enabled = objTarget.transform.localPosition[(int)dir] < loc;
					break;
			}
		}
	}
}
