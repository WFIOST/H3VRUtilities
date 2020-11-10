using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FistVR;
using System;

namespace pcgH3VRframework
{
	public class attachmentYFoldingStockProxy : FVRInteractiveObject
	{

		public Transform Root;
		public Transform Stock;
		public float MinRot;
		public float MaxRot;
		public FVRFoldingStockYAxis.StockPos m_curPos;
		public FVRFoldingStockYAxis.StockPos m_lastPos;
		public bool isMinClosed = true;
		public FVRFireArm FireArm;
		public FVRFireArmAttachment attachment;
		public enum StockPos
		{
			Closed,
			Mid,
			Open
		}

	}
}
