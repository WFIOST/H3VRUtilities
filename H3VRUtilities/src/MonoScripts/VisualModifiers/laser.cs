using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace H3VRUtils.MonoScripts.VisualModifiers
{
    class Laser : MonoBehaviour
    {
        public GameObject endpoint;

        private LineRenderer _lr;
        void Start()
        {
            _lr = GetComponent<LineRenderer>();
        }

        void Update()
        {
            _lr.SetPosition(0, transform.position);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit))
            {
                if (hit.collider)
                {
                    _lr.SetPosition(1, hit.point);
                }
            }
            else
            {
                _lr.SetPosition(1, endpoint.transform.position);
            }
        }
    }
}