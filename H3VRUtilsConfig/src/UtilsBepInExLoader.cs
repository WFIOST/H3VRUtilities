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
using System.Reflection;

namespace H3VRUtils
{
	[BepInPlugin("dll.wfiost.h3vrutilities", "H3VR Utilities", "8.9.3")]
	[BepInDependency("nrgill28.Sodalite", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInProcess("h3vr.exe")]
	public class UtilsBepInExLoader : BaseUnityPlugin
	{
		public static ConfigEntry<bool> paddleMagRelease;
		public static ConfigEntry<bool> magDropRequiredRelease;
		public static ConfigEntry<bool> VehicleLockXRot;
		public static ConfigEntry<bool> VehicleLockYRot;
		public static ConfigEntry<bool> VehicleLockZRot;
		public static ConfigEntry<bool> SimpleControls;
		public static ConfigEntry<TouchpadDirTypePT> paddleMagReleaseDir;

		public static bool setToEnablePaddleMagRelease;
		
		public enum TouchpadDirTypePT
		{
			Up,
			Down,
			Left,
			Right,
			Trigger,
			BasedOnWeapon
		}

		public static void EnablePaddleMagRelease()
		{
			try
			{
				paddleMagRelease.Value = true;
				magDropRequiredRelease.Value = true;
			}
			catch
			{
				setToEnablePaddleMagRelease = true;
			}
		}
		
		
		void Awake()
		{
			
			paddleMagRelease = Config.Bind("General Settings", "Enable Paddle Release", false, "Allows custom guns to utilize the feature to require a direction press on the touchpad to release the mag, usually to simulate a paddle release.");
			magDropRequiredRelease = Config.Bind("General Settings", "Enable Mandatory Mag Drop", false, "Allows custom guns to utilize the feature to require the mag to be dropped by your primary hand, even if your other hand is gripping the magazine.");

			paddleMagReleaseDir = Config.Bind("Fine Tuning", "Enhanced Mag Release Direction", TouchpadDirTypePT.BasedOnWeapon, "Based On Weapon is the default direction chosen by the modmaker. Others are overrides.");

			VehicleLockXRot = Config.Bind("Vehicles", "Lock X Rotation", false, "Rotates your X rotation based on the vehicles rotation. Induces VR sickness.");
			VehicleLockYRot = Config.Bind("Vehicles", "Lock Y Rotation", false, "Rotates your Y rotation based on the vehicles rotation. Induces VR sickness.");
			VehicleLockZRot = Config.Bind("Vehicles", "Lock Z Rotation", false, "Rotates your Z rotation based on the vehicles rotation. Induces VR sickness.");
			SimpleControls  = Config.Bind("General Settings", "Enable Simple Controls", false, "Allows simpler controls (e.g enables ModulAR bolt release via up press on touchpad)");
			//Harmony.CreateAndPatchAll(typeof(MagReplacer));

			if (setToEnablePaddleMagRelease)
			{
				setToEnablePaddleMagRelease = false;
				paddleMagRelease.Value = true;
				magDropRequiredRelease.Value = true;
			}
			
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
			WristMenuAPI.Buttons.Add(new WristMenuButton("Spawn Utils Panel", int.MaxValue, SpawnUtilsPanel));
            
			//setup panel
			_UtilsPanel = new LockablePanel();
			_UtilsPanel.Configure += ConfigureUtilsPanel;
			_UtilsPanel.TextureOverride = SodaliteUtils.LoadTextureFromBytes(Assembly.GetExecutingAssembly().GetResource("UtilsPanel.png"));
		}
        
		ButtonWidget paddleMagReleaseButton;
		ButtonWidget MagDropRequiredReleaseButton;
        
		ButtonWidget VehicleLockXRotButton;
		ButtonWidget VehicleLockYRotButton;
		ButtonWidget VehicleLockZRotButton;
		
		ButtonWidget SimpleControls;
		
		
		public static string GetTerm(bool value)
		{
			//what the fuck do this do i'm not smart
			return value ? "Disable" : "Enable";
		}
		
		private void ConfigureUtilsPanel(GameObject panel)
		{
			//there's gotta be a fucking better way man there's no way
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
					button.ButtonText.transform.localRotation = Quaternion.identity;
					});
				
				widget.AddChild((ButtonWidget button) => {
					button.ButtonText.text = GetTerm(UtilsBepInExLoader.magDropRequiredRelease.Value) + " Mag Drop Required Release";
					button.AddButtonListener(ToggleMagRelease);
					MagDropRequiredReleaseButton = button;
					button.ButtonText.transform.localRotation = Quaternion.identity;
				});
				
				/*widget.AddChild((ButtonWidget button) => {
					button.ButtonText.text = "Reload Magazine Release Cache";
					button.AddButtonListener(ReloadVanillaMagRelease);
					MagDropRequiredReleaseButton = button;
					button.RectTransform.localRotation = Quaternion.identity;
				});
				
				//ROW THREE
				widget.AddChild((ButtonWidget button) => {
					button.ButtonText.text = GetTerm(UtilsBepInExLoader.VehicleLockXRot.Value) + " Vehicle X Lock";
					button.AddButtonListener(lockXRot);
					VehicleLockXRotButton = button;
					button.RectTransform.localRotation = Quaternion.identity;
				});
				
				widget.AddChild((ButtonWidget button) => {
					button.ButtonText.text = GetTerm(UtilsBepInExLoader.VehicleLockYRot.Value) + " Vehicle Y Lock";
					button.AddButtonListener(lockYRot);
					VehicleLockYRotButton = button;
					button.RectTransform.localRotation = Quaternion.identity;
				});
				
				widget.AddChild((ButtonWidget button) => {
					button.ButtonText.text = GetTerm(UtilsBepInExLoader.VehicleLockZRot.Value) + " Vehicle Z Lock";
					button.AddButtonListener(lockZRot);
					VehicleLockZRotButton = button;
					button.RectTransform.localRotation = Quaternion.identity;
				});*/
				
				//ROW FOUR (actually due to disabled buttons it'll appear row 2)
				widget.AddChild((ButtonWidget button) => {
					button.ButtonText.text = GetTerm(UtilsBepInExLoader.SimpleControls.Value) + " Simple Controls";
					button.AddButtonListener(ToggleSimpleControls);
					SimpleControls = button;
					button.ButtonText.transform.localRotation = Quaternion.identity;
				});
			});
		}

		private void TogglePaddleRelease(object sender, ButtonClickEventArgs args) {
			UtilsBepInExLoader.paddleMagRelease.Value = !UtilsBepInExLoader.paddleMagRelease.Value;
			paddleMagReleaseButton.ButtonText.text = GetTerm(UtilsBepInExLoader.paddleMagRelease.Value) + " Paddle Release";
		}
		private void ToggleMagRelease(object sender, ButtonClickEventArgs args) {
			UtilsBepInExLoader.magDropRequiredRelease.Value = !UtilsBepInExLoader.magDropRequiredRelease.Value;
			MagDropRequiredReleaseButton.ButtonText.text = GetTerm(UtilsBepInExLoader.magDropRequiredRelease.Value) + " Mag Drop Required Release";
		}
		
		private void lockXRot(object sender, ButtonClickEventArgs args)
		{
			UtilsBepInExLoader.VehicleLockXRot.Value = !UtilsBepInExLoader.paddleMagRelease.Value;
			VehicleLockXRotButton.ButtonText.text = GetTerm(UtilsBepInExLoader.paddleMagRelease.Value) + " Vehicle X Lock";
		}
		private void lockYRot(object sender, ButtonClickEventArgs args)
		{
			UtilsBepInExLoader.VehicleLockYRot.Value = !UtilsBepInExLoader.paddleMagRelease.Value;
			VehicleLockYRotButton.ButtonText.text = GetTerm(UtilsBepInExLoader.paddleMagRelease.Value) + " Vehicle Y Lock";
		}
		private void lockZRot(object sender, ButtonClickEventArgs args)
		{
			UtilsBepInExLoader.VehicleLockZRot.Value = !UtilsBepInExLoader.paddleMagRelease.Value;
			VehicleLockZRotButton.ButtonText.text = GetTerm(UtilsBepInExLoader.paddleMagRelease.Value) + " Vehicle Z Lock";
		}
		
		private void ToggleSimpleControls(object sender, ButtonClickEventArgs args)
		{
			UtilsBepInExLoader.SimpleControls.Value = !UtilsBepInExLoader.SimpleControls.Value;
			SimpleControls.ButtonText.text = GetTerm(UtilsBepInExLoader.SimpleControls.Value) + " Simple Controls";
		}
		
		/*private void ReloadVanillaMagRelease()
		{
			MagReplacerData.GetMagDropData(true);
			MagReplacerData.GetPaddleData(true);
		}*/

		private void SpawnUtilsPanel(object sender, ButtonClickEventArgs args)
		{
			FVRWristMenu wristMenu = WristMenuAPI.Instance;
			if (wristMenu is null || !wristMenu) return;
			GameObject panel = _UtilsPanel.GetOrCreatePanel();
			wristMenu.m_currentHand.RetrieveObject(panel.GetComponent<FVRPhysicalObject>());
		}
	}
}
