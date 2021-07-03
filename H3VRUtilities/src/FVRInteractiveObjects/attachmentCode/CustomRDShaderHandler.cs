using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;
using UnityEngine.UI;

namespace H3VRUtils
{
	class CustomRDShaderHandler : FVRFireArmAttachment
	{
		public bool enableMagnificationSettings;
		public MeshRenderer Reticle;
		public MeshRenderer Magnifier;
		public Canvas SettingsTextCanvas;
		public Text ReticleYOffsetText;
		public Text ReticleXOffsetText;
		public Text MagnifierMagnificationText;
		public Transform HighlightPosOffsetX;
		public Transform HighlightPosOffsetY;
		public Transform HighlightPosMagnification;
		public List<float> OffsetXNums;
		public List<string> OffsetXNames;
		public List<float> OffsetYNums;
		public List<string> OffsetYNames;
		public List<float> Magnification;
		public List<string> MagnificationNames;
		private Material matReticle;
		private Material matMagnifier;
		private Shader scopeShader;
		private Shader redDotShader;
		private int selectedtxt;

		public void Start()
		{
			redDotShader = Shader.Find("RedDot(Unlit)");
			scopeShader = Shader.Find("Magnification");
			matReticle = Reticle.material;
			matMagnifier = Magnifier.material;
			matReticle.SetFloat("_OffsetX", 4f);
		}

		public static bool IfPressedInDir(FVRViveHand hand, Vector2 dir)
		{
			if (Vector2.Angle(hand.Input.TouchpadAxes, dir) <= 45f && hand.Input.TouchpadDown && hand.Input.TouchpadAxes.magnitude > 0.2f) return true;
			return false;
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			SettingsTextCanvas.enabled = true;
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);

			if (IfPressedInDir(hand, Vector2.up))
			{
				selectedtxt++;
			}
			if (IfPressedInDir(hand, Vector2.left))
			{

			}
			int val = 1;
			if (enableMagnificationSettings) val++;
			if (selectedtxt > val) selectedtxt = 0;
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			SettingsTextCanvas.enabled = false;
		}

	}
}
