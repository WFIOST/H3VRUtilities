using System;
using FistVR;
using UnityEngine;

namespace H3VRUtils
{
    public class AttachmentXFoldingStock : FVRFoldingStockXAxis
    {
        public FVRFireArmAttachment attachment;

        private float _rotAngle;

        public void FixedUpdate()
        {
            if (attachment.curMount != null)
            {
                if (FireArm == null)
                {
                    FVRFireArm firearm = attachment.curMount.Parent.GetComponent<FVRFireArm>();
                    FireArm = firearm;
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