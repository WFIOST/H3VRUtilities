using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FistVR;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace H3VRUtils
{
	public class CustomRdShaderHandler : FVRFireArmAttachment
	{
		public bool enableMagnificationSettings;
		[FormerlySerializedAs("Reticle")] public MeshRenderer reticle;
		[FormerlySerializedAs("Magnifier")] public MeshRenderer magnifier;
		[FormerlySerializedAs("SettingsTextCanvas")] public Canvas settingsTextCanvas;
		[FormerlySerializedAs("ReticleYOffsetText")] public Text reticleYOffsetText;
		[FormerlySerializedAs("ReticleXOffsetText")] public Text reticleXOffsetText;
		[FormerlySerializedAs("MagnifierMagnificationText")] public Text magnifierMagnificationText;
		[FormerlySerializedAs("HighlightPosOffsetX")] public Transform highlightPosOffsetX;
		[FormerlySerializedAs("HighlightPosOffsetY")] public Transform highlightPosOffsetY;
		[FormerlySerializedAs("HighlightPosMagnification")] public Transform highlightPosMagnification;
		[FormerlySerializedAs("OffsetXNums")] public List<float> offsetXNums;
		[FormerlySerializedAs("OffsetXNames")] public List<string> offsetXNames;
		[FormerlySerializedAs("OffsetYNums")] public List<float> offsetYNums;
		[FormerlySerializedAs("OffsetYNames")] public List<string> offsetYNames;
		[FormerlySerializedAs("Magnification")] public List<float> magnification;
		[FormerlySerializedAs("MagnificationNames")] public List<string> magnificationNames;
		private Material _matReticle;
		private Material _matMagnifier;
		private Shader _scopeShader;
		private Shader _redDotShader;
		private int _selectedtxt;

		public void Start()
		{
			_redDotShader = Shader.Find("RedDot(Unlit)");
			_scopeShader = Shader.Find("Magnification");
			_matReticle = reticle.material;
			_matMagnifier = magnifier.material;
			_matReticle.SetFloat("_OffsetX", 4f);
		}

		public static bool IfPressedInDir(FVRViveHand hand, Vector2 dir)
		{
			if (Vector2.Angle(hand.Input.TouchpadAxes, dir) <= 45f && hand.Input.TouchpadDown && hand.Input.TouchpadAxes.magnitude > 0.2f) return true;
			return false;
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			settingsTextCanvas.enabled = true;
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);

			if (IfPressedInDir(hand, Vector2.up))
			{
				_selectedtxt++;
			}
			if (IfPressedInDir(hand, Vector2.left))
			{

			}
			int val = 1;
			if (enableMagnificationSettings) val++;
			if (_selectedtxt > val) _selectedtxt = 0;
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			settingsTextCanvas.enabled = false;
		}

	}
}
