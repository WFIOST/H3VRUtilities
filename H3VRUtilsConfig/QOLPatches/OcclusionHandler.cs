using FistVR;
using UnityEngine;

namespace H3VRUtilsConfig.QOLPatches
{
	public class OcclusionHandler : MonoBehaviour
	{
		public void Start()
		{
			Camera1 = ManagerSingleton<GM>.Instance.m_currentPlayerBody.EyeCam;
			var objs = Object.FindObjectsOfType<GameObject>();
			foreach (var obj in objs)
			{
				if (obj.isStatic) obj.AddComponent<StaticOcclusion>();
			}
		}

		public void Update()
		{
			planes = GeometryUtility.CalculateFrustumPlanes(Camera1);
		}

		public static Plane[] planes;
		public static Camera Camera1;
		//public static Camera Camera2;
	}
}