﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;
using UnityEngine.Serialization;

namespace H3VRUtils.stub
{
	class attachmentYFoldingStock : FVRInteractiveObject
	{
		[FormerlySerializedAs("Root")] public Transform root;


		[FormerlySerializedAs("Stock")] public Transform stock;

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
