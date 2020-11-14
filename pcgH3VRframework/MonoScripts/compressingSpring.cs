using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace H3VRUtils
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
			var localScale = spring.transform.localScale;
			localScale = new Vector3(localScale.x, localScale.y, (compressor.transform.localPosition.z - fullcompress) * (1 / (fullextend - fullcompress)));
			spring.transform.localScale = localScale;
		}
	}
}
