using FistVR;
using UnityEngine;

namespace H3VRUtils.Mapping
{
    public class SosigSpawn : MonoBehaviour
    {
        [Header("Choose Sosig Type")] public SosigEnemyID listOfSosigs;

        private Transform sosigSpawnLocation;

        //private ScriptableObject[] sosigTemplates;


        private void Awake()
        {
            sosigSpawnLocation = transform;
        }
    }
}