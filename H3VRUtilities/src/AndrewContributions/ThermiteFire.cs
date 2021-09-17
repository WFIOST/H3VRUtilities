using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;
using Unity;

namespace Plugin.src
{
    class thermiteFire : MonoBehaviour
    {
        public Vector2 angleX;
        public Vector2 angleY;
        public Vector2 velocity;
        public Vector2 durration;
        public Vector2 delay;
        public BallisticProjectile projToFire;
        //public GameObject objToDestroy;

        private int fixedDurration;
        private int fixedDelay;
        private int fixedAngleX;
        private int fixedAngleY;
        private int fixedVelocity;
       
       

        void Start()
        {
           // Debug.Log("start");
            //converting all seconds to units of .02 seconds at 1unit/.02sec or 50 units a second
            fixedDurration = (int)UnityEngine.Random.Range(durration.x, durration.y) * 50;
            fixedDelay = (int)UnityEngine.Random.Range(delay.x, delay.y) * 50;

            fixedAngleX = (int)(UnityEngine.Random.Range(angleX.x, angleX.y)*0.01745);
            fixedAngleY = (int)(UnityEngine.Random.Range(angleY.x, angleY.y)*0.01745);

            fixedVelocity = (int)UnityEngine.Random.Range(velocity.x, velocity.y);


            //projToFire.transform.localEulerAngles = new Vector3(UnityEngine.Random.Range(angleX.x, angleX.y), 0, UnityEngine.Random.Range(angleY.x, angleY.y));
            //projToFire.m_velocity = new Vector3(UnityEngine.Random.Range(velocity.x, velocity.y), 0, UnityEngine.Random.Range(velocity.x, velocity.y));    //I dont think I need this?
        }

        void FixedUpdate() 
        {
            //Debug.Log("start of update cycle");

            if (fixedDurration <= 0) //Checks the remaining durration, and if not 0, subtracts one checktime from it so its ready for the next time
            {
                //Debug.Log("destroy at end of durration");
                Destroy(gameObject);

            }
            else
            {
                
                fixedDurration-- ;//remove 1 time from durration

                if (fixedDelay >= 0)//if time done then do the pew
                {
                    fixedDelay-- ;
                    //Debug.Log("delay not 0 yet");
                }
                else
                {

                    Vector3 sphericalCoord = new Vector3(1, fixedAngleY, fixedAngleX);
                    Vector3 cartesianCoord = new Vector3(Mathf.Sin(fixedAngleX) * Mathf.Cos(fixedAngleY), Math.Abs(Mathf.Sin(fixedAngleX) * Mathf.Sin(fixedAngleY)), Mathf.Cos(fixedAngleX));

                    //Debug.Log("spawning particle");
                    BallisticProjectile firedProjectile = Instantiate(projToFire, this.transform.position, new Quaternion(fixedAngleX,fixedAngleY,0,0));

                    //firedProjectile.m_velocity = new Vector3(fixedVelocity, 0, 0);
                    firedProjectile.SetSource_IFF(GM.CurrentPlayerBody.GetPlayerIFF());
                    firedProjectile.Fire(fixedVelocity, cartesianCoord, null );

                    fixedDelay = (int)UnityEngine.Random.Range(delay.x, delay.y); //makes new value for next run

                    fixedAngleX = (int)UnityEngine.Random.Range(angleX.x, angleX.y);
                    fixedAngleY = (int)UnityEngine.Random.Range(angleY.x, angleY.y);

                    fixedVelocity = (int)UnityEngine.Random.Range(velocity.x, velocity.y);
                }
            }
        }
    }
}
