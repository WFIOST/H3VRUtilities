using UnityEngine;
using FistVR;

namespace H3VRUtils.NonAddedScripts
{
    public class GetTransformPosition : MonoBehaviour
    {
        public TNH_ShatterableCrate[] shatteredCrates = FindObjectsOfType<TNH_ShatterableCrate>();

        public GameObject[] shatteredCratesObjects;
        
        public GameObject[] GetCrates()
        {
            shatteredCratesObjects = new GameObject[shatteredCrates.Length];
            for (int i = 0; i < shatteredCrates.Length; i++)
            {
                shatteredCratesObjects[i] = shatteredCrates[i].gameObject;
            }

            return shatteredCratesObjects;
        }
    }
}