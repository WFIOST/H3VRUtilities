using UnityEngine;

namespace H3VRUtils
{
    internal class compressingSpring : MonoBehaviour
    {
        public GameObject compressor;
        public GameObject spring;
        public float fullextend;

        public float fullcompress;
        /*		public enum dirType
                {
                    x = 0,
                    y = 1,
                    z = 2
                }*/

        //		public dirType directionofcompression;
        private void Update()
        {
            var localScale = spring.transform.localScale;
            localScale = new Vector3(localScale.x, localScale.y,
                (compressor.transform.localPosition.z - fullcompress) * (1 / (fullextend - fullcompress)));
            spring.transform.localScale = localScale;
        }
    }
}