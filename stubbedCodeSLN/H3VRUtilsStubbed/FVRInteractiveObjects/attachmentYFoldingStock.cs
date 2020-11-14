using System;
using UnityEngine;
using FistVR;
using UnityEngine.Serialization;

namespace H3VRUtils
{
	// Token: 0x020003CD RID: 973
	public class attachmentYFoldingStock : FVRInteractiveObject
	{
		[FormerlySerializedAs("Root")] public Transform root;
		[FormerlySerializedAs("Stock")] public Transform stock;
		private float _rotAngle;
		public float minRot;
		public float maxRot;
		[FormerlySerializedAs("m_curPos")] public FVRFoldingStockYAxis.StockPos mCurPos;
		[FormerlySerializedAs("m_lastPos")] public FVRFoldingStockYAxis.StockPos mLastPos;
		public bool isMinClosed = true;
		[FormerlySerializedAs("FireArm")] public FVRFireArm fireArm;
		public FVRFireArmAttachment attachment;
		public enum StockPos
		{
			Closed,
			Mid,
			Open
		}
	}
}

