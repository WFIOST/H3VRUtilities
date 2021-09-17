using UnityEngine;

namespace H3VRUtilsConfig.QOLPatches
{
	public class StaticOcclusion : MonoBehaviour
	{
		public Collider m_collider;
		public MeshFilter m_mesh;

		void Start()
		{
			m_collider = GetComponent<Collider>();
			m_mesh = GetComponent<MeshFilter>();
			if(m_collider == null) Destroy(GetComponent<StaticOcclusion>());
		}
		void Update()
		{
			if (GeometryUtility.TestPlanesAABB(OcclusionHandler.planes, m_collider.bounds))
			{
				gameObject.SetActive(true);
			}
			else
			{
				gameObject.SetActive(false);
			}
		}
	}
}