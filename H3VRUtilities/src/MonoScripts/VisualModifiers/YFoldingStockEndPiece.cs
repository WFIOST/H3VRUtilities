using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;

namespace H3VRUtils.FVRInteractiveObjects
{
	class YFoldingStockEndPiece : MonoBehaviour
	{
		[FormerlySerializedAs("MainPieceDir")] public CullOnZLoc.DirType mainPieceDir;
		[FormerlySerializedAs("MainPiece")] public GameObject mainPiece;
		[FormerlySerializedAs("MainPieceMinRot")] public float mainPieceMinRot;
		[FormerlySerializedAs("MainPieceMaxRot")] public float mainPieceMaxRot;
		[FormerlySerializedAs("EndPieceDir")] public CullOnZLoc.DirType endPieceDir;
		[FormerlySerializedAs("EndPiece")] public GameObject endPiece;
		[FormerlySerializedAs("EndPieceMinRot")] public float endPieceMinRot;
		[FormerlySerializedAs("EndPieceMaxRot")] public float endPieceMaxRot;



		public void Update()
		{
			Vector3 localRot = endPiece.transform.localEulerAngles;
			float[] dir = new float[3];

			dir[0] = localRot.x;
			dir[1] = localRot.y;
			dir[2] = localRot.z;
			float invlerp = Mathf.InverseLerp(mainPieceMinRot, mainPieceMaxRot, mainPiece.transform.localRotation[(int)mainPieceDir] * 180);
			dir[(int)endPieceDir] = Mathf.Lerp(endPieceMinRot, endPieceMaxRot, invlerp);


			endPiece.transform.localEulerAngles = new Vector3(dir[0], dir[1], dir[2]);
		}
	}
}
