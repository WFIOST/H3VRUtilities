using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace H3VRUtils.MonoScripts.VisualModifiers
{
	class laser : MonoBehaviour
	{
		public float maxrange = 1000;

		private LineRenderer lr;
		void Start()
		{
			lr = GetComponent<LineRenderer>();
		}

		void Update()
		{
			lr.SetPosition(0, transform.position);
			RaycastHit hit;
			if (Physics.Raycast(transform.position, transform.forward, out hit))
			{
				if (hit.collider)
				{
					lr.SetPosition(1, hit.point);
				}
			}
			else lr.SetPosition(1, transform.forward * maxrange);
		}
	}
}
