using UnityEngine;

namespace H3VRUtils
{
    public class cullOnZLoc : MonoBehaviour
    {
        public enum cutOffType
        {
            above,
            below
        }

        public enum dirType
        {
            x = 0,
            y = 1,
            z = 2
        }

        public cutOffType cutoff;
        public double loc;
        public GameObject objTarget;
        public dirType dir;
        private MeshRenderer objMeshRenderer;


        private void Start()
        {
            objMeshRenderer = gameObject.GetComponent<MeshRenderer>();
        }

        private void Update()
        {
            switch (cutoff)
            {
                case cutOffType.below:
                    objMeshRenderer.enabled = objTarget.transform.localPosition[(int) dir] > loc;
                    break;
                case cutOffType.above:
                    objMeshRenderer.enabled = objTarget.transform.localPosition[(int) dir] < loc;
                    break;
            }


/*
			switch (dir)
			{
				case dirType.x:
					if (objTarget.transform.localPosition.x <= loc)
					{
						objMeshRenderer.enabled = false;
					}
					else if (objTarget.transform.localPosition.x > loc)
					{
						objMeshRenderer.enabled = true;
					}
					break;
				case dirType.y:
					if (objTarget.transform.localPosition.y <= loc)
					{
						objMeshRenderer.enabled = false;
					}
					else if (objTarget.transform.localPosition.y > loc)
					{
						objMeshRenderer.enabled = true;
					}
					break;
				case dirType.z:
					if (objTarget.transform.localPosition.z <= loc)
					{
						objMeshRenderer.enabled = false;
					}
					else if (objTarget.transform.localPosition.z > loc)
					{
						objMeshRenderer.enabled = true;
					}
					break;

			}*/
        }
    }
}