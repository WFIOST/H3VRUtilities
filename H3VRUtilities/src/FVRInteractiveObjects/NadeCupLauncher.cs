using FistVR;
using UnityEngine;

namespace H3VRUtils.Weapons.NadeCup
{
    public class NadeCupLauncher : MonoBehaviour
    {
        public FVRFireArmChamber mainChamber;
        public FVRFireArmChamber nadeCup;
        public bool alreadyFired;

        private void FixedUpdate()
        {
            if (mainChamber.IsSpent && alreadyFired == false)
            {
                nadeCup.Fire();
                alreadyFired = true;
            }
            else if (mainChamber.IsSpent == false && alreadyFired)
            {
                alreadyFired = false;
            }
        }
    }
}