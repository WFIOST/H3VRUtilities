﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace H3VRUtils.stub
{
	class compressingSpring : MonoBehaviour
	{
		public GameObject compressor;
		public GameObject spring;
		public float fullextend;
		public float fullcompress;
		/*		public enum dirType
				{
					x = 0,
					y = 1,
					z = 2
				}*/

		//		public dirType directionofcompression;
		void Update()
		{
			spring.transform.localScale = new Vector3(spring.transform.localScale.x, spring.transform.localScale.y, (compressor.transform.localPosition.z - fullcompress) * (1 / (fullextend - fullcompress)));
		}
	}
}
