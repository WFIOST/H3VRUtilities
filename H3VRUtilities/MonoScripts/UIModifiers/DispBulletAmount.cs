using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using FistVR;

namespace H3VRUtils.MonoScripts.UIModifiers
{
    public class DispBulletAmount : MonoBehaviour
    {
        public FVRFireArm firearm;
        public Text UItext;
        public string textWhenEmpty;
        [Tooltip("When there is no mag, the text will remain whatever it was before.")]
        public bool KeepLastRoundInfoOnEmpty;
        public void FixedUpdate()
        {
            if (firearm.Magazine != null)
            {
                UItext.text = firearm.Magazine.m_numRounds.ToString();
            }
            else
            {
                UItext.text = textWhenEmpty;
            }
        }
    }
}