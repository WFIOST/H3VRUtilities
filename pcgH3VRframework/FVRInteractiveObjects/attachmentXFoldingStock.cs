using System;
using FistVR;
using UnityEngine;

namespace H3VRUtils
{
    internal class attachmentXFoldingStock : FVRFoldingStockXAxis
    {
        public FVRFireArmAttachment attachment;

        private float rotAngle;

        public void FixedUpdate()
        {
            if (attachment.curMount != null)
            {
                if (FireArm == null)
                {
                    var _firearm = attachment.curMount.Parent.GetComponent<FVRFireArm>();
                    FireArm = _firearm;
                    Console.WriteLine("attachmentYFoldingStock has connected itself to " + FireArm);
                }
            }
            else if (FireArm != null)
            {
                FireArm = null;
            }
        }

        public override void UpdateInteraction(FVRViveHand hand)
        {
            base.UpdateInteraction(hand);
            var vector = hand.transform.position - Root.position;
            vector = Vector3.ProjectOnPlane(vector, Root.right).normalized;
            var lhs = Stock.forward;
            if (InvertZRoot) lhs = -Stock.forward;
            var num = Mathf.Atan2(Vector3.Dot(Root.right, Vector3.Cross(lhs, vector)), Vector3.Dot(lhs, vector)) *
                      57.29578f;
            num = Mathf.Clamp(num, -10f, 10f);
            rotAngle += num;
            rotAngle = Mathf.Clamp(rotAngle, MinRot, MaxRot);
            if (Mathf.Abs(rotAngle - MinRot) < 5f) rotAngle = MinRot;
            if (Mathf.Abs(rotAngle - MaxRot) < 5f) rotAngle = MaxRot;
            if (rotAngle >= MinRot && rotAngle <= MaxRot)
            {
                Stock.localEulerAngles = new Vector3(rotAngle, 0f, 0f);
                var num2 = Mathf.InverseLerp(MinRot, MaxRot, rotAngle);
                if (EndPiece != null)
                    EndPiece.localEulerAngles = new Vector3(Mathf.Lerp(EndPieceMinRot, EndPieceMaxRot, num2), 0f, 0f);
                if (FireArm != null)
                {
                    if (isMinClosed)
                    {
                        if (num2 < 0.02f)
                        {
                            m_curPos = StockPos.Closed;
                            if (DoesToggleStockPoint) FireArm.HasActiveShoulderStock = false;
                        }
                        else if (num2 > 0.9f)
                        {
                            m_curPos = StockPos.Open;
                            if (DoesToggleStockPoint) FireArm.HasActiveShoulderStock = true;
                        }
                        else
                        {
                            m_curPos = StockPos.Mid;
                            if (DoesToggleStockPoint) FireArm.HasActiveShoulderStock = false;
                        }
                    }
                    else if (num2 < 0.1f)
                    {
                        m_curPos = StockPos.Open;
                        if (DoesToggleStockPoint) FireArm.HasActiveShoulderStock = true;
                    }
                    else if (num2 > 0.98f)
                    {
                        m_curPos = StockPos.Closed;
                        if (DoesToggleStockPoint) FireArm.HasActiveShoulderStock = false;
                    }
                    else
                    {
                        m_curPos = StockPos.Mid;
                        if (DoesToggleStockPoint) FireArm.HasActiveShoulderStock = false;
                    }
                }
            }

            if (m_curPos == StockPos.Open && m_lastPos != StockPos.Open)
                FireArm.PlayAudioEvent(FirearmAudioEventType.StockOpen);
            if (m_curPos == StockPos.Closed && m_lastPos != StockPos.Closed)
                FireArm.PlayAudioEvent(FirearmAudioEventType.StockClosed);
            m_lastPos = m_curPos;
        }
    }
}