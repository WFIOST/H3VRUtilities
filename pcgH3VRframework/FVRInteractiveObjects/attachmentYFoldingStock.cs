using FistVR;
using UnityEngine;
using UnityEngine.Serialization;

namespace H3VRUtils
{
    // Token: 0x020003CD RID: 973
    public class attachmentYFoldingStock : FVRInteractiveObject
    {
        public enum StockPos
        {
            Closed,
            Mid,
            Open
        }

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

        // Token: 0x0400276E RID: 10094
        private float _rotAngle;

        // Token: 0x0600141B RID: 5147 RVA: 0x00089ED0 File Offset: 0x000882D0
        public override void UpdateInteraction(FVRViveHand hand)
        {
            base.UpdateInteraction(hand);
            var vector = hand.transform.position - root.position;
            var up = root.up;
            vector = Vector3.ProjectOnPlane(vector, up).normalized;
            var lhs = -root.transform.forward;
            _rotAngle = Mathf.Atan2(Vector3.Dot(up, Vector3.Cross(lhs, vector)), Vector3.Dot(lhs, vector)) * 57.29578f;
            if (Mathf.Abs(_rotAngle - minRot) < 5f) _rotAngle = minRot;
            if (Mathf.Abs(_rotAngle - maxRot) < 5f) _rotAngle = maxRot;
            if (_rotAngle >= minRot && _rotAngle <= maxRot)
            {
                stock.localEulerAngles = new Vector3(0f, _rotAngle, 0f);
                var num = Mathf.InverseLerp(minRot, maxRot, _rotAngle);
                if (fireArm != null)
                {
                    if (isMinClosed)
                    {
                        if (num < 0.02f)
                        {
                            mCurPos = FVRFoldingStockYAxis.StockPos.Closed;
                            fireArm.HasActiveShoulderStock = false;
                        }
                        else if (num > 0.9f)
                        {
                            mCurPos = FVRFoldingStockYAxis.StockPos.Open;
                            fireArm.HasActiveShoulderStock = true;
                        }
                        else
                        {
                            mCurPos = FVRFoldingStockYAxis.StockPos.Mid;
                            fireArm.HasActiveShoulderStock = false;
                        }
                    }
                    else if (num < 0.1f)
                    {
                        mCurPos = FVRFoldingStockYAxis.StockPos.Open;
                        fireArm.HasActiveShoulderStock = true;
                    }
                    else if (num > 0.98f)
                    {
                        mCurPos = FVRFoldingStockYAxis.StockPos.Closed;
                        fireArm.HasActiveShoulderStock = false;
                    }
                    else
                    {
                        mCurPos = FVRFoldingStockYAxis.StockPos.Mid;
                        fireArm.HasActiveShoulderStock = false;
                    }

                    if (mCurPos == FVRFoldingStockYAxis.StockPos.Open && mLastPos != FVRFoldingStockYAxis.StockPos.Open)
                        fireArm.PlayAudioEvent(FirearmAudioEventType.StockOpen);
                    if (mCurPos == FVRFoldingStockYAxis.StockPos.Closed &&
                        mLastPos != FVRFoldingStockYAxis.StockPos.Closed)
                        fireArm.PlayAudioEvent(FirearmAudioEventType.StockClosed);
                    mLastPos = mCurPos;
                }
            }
        }
    }
}