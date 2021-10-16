namespace H3VRUtils
{
	public class SimpleControls_ClosedBolt_SelectorSwitch : SimpleControls_ClosedBolt
	{
		public void FixedUpdate() { 
			cbw.HasFireSelectorButton = UtilsBepInExLoader.SimpleControls.Value;
		}
	}
}