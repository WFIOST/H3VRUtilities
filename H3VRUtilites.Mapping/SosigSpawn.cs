using System.Diagnostics;
using FistVR;
using UnityEngine;

namespace H3VRUtils.Mapping
{
    public class SosigSpawn : MonoBehaviour
    {
        private Transform sosigSpawnLocation;
        [Header("Choose Sosig Type")]
        public SosigEnemyID listOfSosigs;
        
        //private ScriptableObject[] sosigTemplates;
        

        private void Awake()
        {
            sosigSpawnLocation = this.transform;
            
        }
    }
}