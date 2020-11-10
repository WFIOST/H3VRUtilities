using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace pcgH3VRframework
{

	public class cullOnZLoc : MonoBehaviour
	{

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
			objMeshRenderer.enabled = objTarget.transform.localPosition[(int) dir] > loc;
/*
			switch (dir)
			{
				case dirType.x:
					if (objTarget.transform.localPosition.x <= loc)
					{
						objMeshRenderer.enabled = false;
					}
					else if (objTarget.transform.localPosition.x > loc)
					{
						objMeshRenderer.enabled = true;
					}
					break;
				case dirType.y:
					if (objTarget.transform.localPosition.y <= loc)
					{
						objMeshRenderer.enabled = false;
					}
					else if (objTarget.transform.localPosition.y > loc)
					{
						objMeshRenderer.enabled = true;
					}
					break;
				case dirType.z:
					if (objTarget.transform.localPosition.z <= loc)
					{
						objMeshRenderer.enabled = false;
					}
					else if (objTarget.transform.localPosition.z > loc)
					{
						objMeshRenderer.enabled = true;
					}
					break;

			}*/

		}
	}
}
