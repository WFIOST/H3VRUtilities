using FistVR;
using UnityEngine;

namespace H3VRUtils.Proxy
{
    //Proxies are deprecated, please use the stubbed code.
    public class attachmentYFoldingStockProxy : FVRInteractableObjectProxy
    {
        public enum StockPos
        {
            Closed,
            Mid,
            Open
        }

        public Transform Root;
        public Transform Stock;
        public float MinRot;
        public float MaxRot;
        public FVRFoldingStockYAxis.StockPos m_curPos;
        public FVRFoldingStockYAxis.StockPos m_lastPos;
        public bool isMinClosed = true;
        public FVRFireArm FireArm;
        public FVRFireArmAttachment attachment;

        public void byeworld()
        {
            Destroy(this);
        }
    }
}