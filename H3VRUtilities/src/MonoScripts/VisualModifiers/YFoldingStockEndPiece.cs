using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace H3VRUtils.FVRInteractiveObjects
{
	class YFoldingStockEndPiece : MonoBehaviour
	{
		public cullOnZLoc.dirType MainPieceDir;
		public GameObject MainPiece;
		public float MainPieceMinRot;
		public float MainPieceMaxRot;
		public cullOnZLoc.dirType EndPieceDir;
		public GameObject EndPiece;
		public float EndPieceMinRot;
		public float EndPieceMaxRot;



		public void Update()
		{
			var localRot = EndPiece.transform.localEulerAngles;
			float[] dir = new float[3];

			dir[0] = localRot.x;
			dir[1] = localRot.y;
			dir[2] = localRot.z;
			float invlerp = Mathf.InverseLerp(MainPieceMinRot, MainPieceMaxRot, MainPiece.transform.localRotation[(int)MainPieceDir] * 180);
			dir[(int)EndPieceDir] = Mathf.Lerp(EndPieceMinRot, EndPieceMaxRot, invlerp);


			EndPiece.transform.localEulerAngles = new Vector3(dir[0], dir[1], dir[2]);
		}
	}
}
