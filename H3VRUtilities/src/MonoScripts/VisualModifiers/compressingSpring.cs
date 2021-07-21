using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace H3VRUtils
{
	class CompressingSpring : MonoBehaviour
	{
		public GameObject compressor;
		public GameObject spring;
		public enum DirType
		{
			X = 0,
			Y = 1,
			Z = 2
		}

		public DirType directionOfCompression = DirType.Z;

		public DirType directionOfCompressor = DirType.Z;

		[Tooltip("The directionOfCompression position where the scale will be 1.")]
		public float fullextend;
		[Tooltip("The directionOfCompression position where the scale will be 0.")]
		public float fullcompress;
		void Update()
		{
			Vector3 localScale = spring.transform.localScale;
			float[] dir = new float[3];

			dir[0] = localScale.x;
			dir[1] = localScale.y;
			dir[2] = localScale.z;


			//dir[(int)directionOfCompression] = Mathf.InverseLerp(fullcompress, fullextend, compressor.transform.localPosition[(int)directionOfCompression]);
			dir[(int)directionOfCompression] = (compressor.transform.localPosition[(int)directionOfCompressor] - fullcompress) * (1 / (fullextend - fullcompress));

			localScale = new Vector3(dir[0], dir[1], dir[2]);
			spring.transform.localScale = localScale;
		}
	}
}
