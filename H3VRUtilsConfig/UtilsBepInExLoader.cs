using System.Resources;
using FistVR;
using UnityEngine;
using UnityEngine.UI;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using Sodalite;
using Sodalite.Api;
using Sodalite.UiWidgets;
using Sodalite.Utilities;
using HarmonyLib;

namespace H3VRUtils
{
	[BepInPlugin("dll.wfiost.h3vrutilities", "H3VR Utilities", "8.0.0")]
	[BepInDependency("nrgill28.Sodalite", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInProcess("h3vr.exe")]
	public class UtilsBepInExLoader : BaseUnityPlugin
	{
		public static ConfigEntry<bool> paddleMagRelease;
		public static ConfigEntry<bool> magDropRequiredRelease;
		public static ConfigEntry<TouchpadDirTypePT> paddleMagReleaseDir;

		public enum TouchpadDirTypePT
		{
			Up,
			Down,
			Left,
			Right,
			Trigger,
			BasedOnWeapon
		}


		void Start()
		{
			paddleMagRelease = Config.Bind("General Settings", "Enable Paddle Release", false, "Allows custom guns to utilize the feature to require a direction press on the touchpad to release the mag, usually to simulate a paddle release.");
			magDropRequiredRelease = Config.Bind("General Settings", "Enable Mandatory Mag Drop", false, "Allows custom guns to utilize the feature to require the mag to be dropped by your primary hand, even if your other hand is gripping the magazine.");

			paddleMagReleaseDir = Config.Bind("Fine Tuning", "Enhanced Mag Release Direction", TouchpadDirTypePT.BasedOnWeapon, "Based On Weapon is the default direction chosen by the modmaker. Others are overrides.");

			Harmony.CreateAndPatchAll(typeof(MagReplacer));


			//sodalite check
			try
			{
				UtilsOptionsPanel uop = new UtilsOptionsPanel(); // dont do this
			} catch
			{
				Logger.LogWarning("Error when initializing panel! Is Sodalite not installed?");
			}
		}
	}

	
	public class UtilsOptionsPanel : MonoBehaviour
	{
		private LockablePanel _UtilsPanel;

		public UtilsOptionsPanel()
		{
			WristMenuAPI.Buttons.Add(new WristMenuButton("Spawn Utils Panel", () => {
				SpawnUtilsPanel();
			}));

			//setup panel
			_UtilsPanel = new LockablePanel();
			_UtilsPanel.Configure += ConfigureUtilsPanel;
		}

		ButtonWidget paddleMagReleaseButton;
		ButtonWidget MagDropRequiredReleaseButton;


		public static string GetTerm(bool value)
		{
			return value ? "Disable" : "Enable";
		}

		private void ConfigureUtilsPanel(GameObject panel)
		{
			
			GameObject canvas = panel.transform.Find("OptionsCanvas_0_Main/Canvas").gameObject;
			UiWidget.CreateAndConfigureWidget(canvas, (GridLayoutWidget widget) =>
			{
				// Fill our parent and set pivot to top middle
				widget.RectTransform.localScale = new Vector3(0.07f, 0.07f, 0.07f);
				widget.RectTransform.localPosition = Vector3.zero;
				widget.RectTransform.anchoredPosition = Vector2.zero;
				widget.RectTransform.sizeDelta = new Vector2(37f / 0.07f, 24f / 0.07f);
				widget.RectTransform.pivot = new Vector2(0.5f, 1f);
				widget.RectTransform.localRotation = Quaternion.identity;
				// Adjust our grid settings
				widget.LayoutGroup.cellSize = new Vector2(171, 50);
				widget.LayoutGroup.spacing = Vector2.one * 4;
				widget.LayoutGroup.startCorner = GridLayoutGroup.Corner.UpperLeft;
				widget.LayoutGroup.startAxis = GridLayoutGroup.Axis.Horizontal;
				widget.LayoutGroup.childAlignment = TextAnchor.UpperCenter;
				widget.LayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
				widget.LayoutGroup.constraintCount = 3;

				//ROW ONE

				widget.AddChild((TextWidget text) => {
					text.Text.text = "";
					text.RectTransform.localRotation = Quaternion.identity;
				});

				widget.AddChild((TextWidget text) => {
					text.Text.text = "H3VR Utilities Settings";
					text.Text.alignment = TextAnchor.MiddleCenter;
					text.Text.fontSize += 5;
					text.RectTransform.localRotation = Quaternion.identity;
				});

				widget.AddChild((TextWidget text) => {
					text.Text.text = "";
					text.RectTransform.localRotation = Quaternion.identity;
				});

				//ROW TWO

				widget.AddChild((ButtonWidget button) => {
					button.ButtonText.text = GetTerm(UtilsBepInExLoader.paddleMagRelease.Value) + " Paddle Release";
					button.AddButtonListener(TogglePaddleRelease);
					paddleMagReleaseButton = button;
					button.RectTransform.localRotation = Quaternion.identity;
					});

				widget.AddChild((ButtonWidget button) => {
					button.ButtonText.text = GetTerm(UtilsBepInExLoader.magDropRequiredRelease.Value) + " Mag Drop Required Release";
					button.AddButtonListener(ToggleMagRelease);
					MagDropRequiredReleaseButton = button;
					button.RectTransform.localRotation = Quaternion.identity;
				});

				widget.AddChild((ButtonWidget button) => {
					button.ButtonText.text = "Reload Magazine Release Cache";
					button.AddButtonListener(ReloadVanillaMagRelease);
					MagDropRequiredReleaseButton = button;
					button.RectTransform.localRotation = Quaternion.identity;
				});
			});
		}


		private void TogglePaddleRelease() {
			UtilsBepInExLoader.paddleMagRelease.Value = !UtilsBepInExLoader.paddleMagRelease.Value;
			paddleMagReleaseButton.ButtonText.text = GetTerm(UtilsBepInExLoader.paddleMagRelease.Value) + " Paddle Release";
		}
		private void ToggleMagRelease() {
			UtilsBepInExLoader.magDropRequiredRelease.Value = !UtilsBepInExLoader.magDropRequiredRelease.Value;
			MagDropRequiredReleaseButton.ButtonText.text = GetTerm(UtilsBepInExLoader.magDropRequiredRelease.Value) + " Mag Drop Required Release";
		}

		private void ReloadVanillaMagRelease()
		{
			MagReplacerData.GetMagDropData(true);
			MagReplacerData.GetPaddleData(true);
		}

		private void SpawnUtilsPanel()
		{
			FVRWristMenu wristMenu = WristMenuAPI.Instance;
			if (wristMenu is null || !wristMenu) return;
			GameObject panel = _UtilsPanel.GetOrCreatePanel();
			wristMenu.m_currentHand.RetrieveObject(panel.GetComponent<FVRPhysicalObject>());
		}
	}
}
