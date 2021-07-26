using FistVR;

namespace H3VRUtils
{
    public class AutoSearAttachment : FVRFireArmAttachment
    {
        private bool _isAuto;
        private void Update()
        {
            FVRPhysicalObject obj = curMount.Parent;

            if (obj == null) return;
            
            if (obj is ClosedBoltWeapon fireArm && !_isAuto)
            {
                fireArm.FireSelector_Modes[0].ModeType = ClosedBoltWeapon.FireSelectorModeType.FullAuto;
                _isAuto = true;
            }
        }
    }
}