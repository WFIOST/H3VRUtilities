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
			try
			{
				base.UpdateInteraction(hand);
			}
			catch
			{

			}
        }
    }
}