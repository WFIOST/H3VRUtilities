using System;
using UnityEngine;
using FistVR;
using UnityEngine.Serialization;

namespace H3VRUtils
{
	// Token: 0x020003CD RID: 973
	public class attachmentYFoldingStock : FVRInteractiveObject
	{
		// Token: 0x0400276C RID: 10092
		[FormerlySerializedAs("Root")] public Transform root;

		// Token: 0x0400276D RID: 10093
		[FormerlySerializedAs("Stock")] public Transform stock;

		// Token: 0x0400276F RID: 10095
		public float minRot;

		// Token: 0x04002770 RID: 10096
		public float maxRot;

		// Token: 0x04002771 RID: 10097
		[FormerlySerializedAs("m_curPos")] public FVRFoldingStockYAxis.StockPos mCurPos;

		// Token: 0x04002772 RID: 10098
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
