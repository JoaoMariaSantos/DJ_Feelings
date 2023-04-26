using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Artifact
{
    public class Diamond : MonoBehaviour
    {
        public float diamondHeight;
        public float minimumHeight;
        public float maximumHeight;
        private float gapHeight;
        private float timeTracker;
        public LayerMask whatIsGround;

        void Start()
        {
            gapHeight = maximumHeight - minimumHeight;
            gameObject.transform.position = new Vector3(Mathf.Round(gameObject.transform.position.x), gameObject.transform.position.y, Mathf.Round(gameObject.transform.position.z));
        }

        // Update is called once per frame
        void Update()
        {
            Ray downRay = new Ray(transform.position, -Vector3.up);
            RaycastHit toGround;

            bool onGround = Physics.Raycast(downRay, out toGround, Mathf.Infinity, whatIsGround);

            if (onGround)
            {
                timeTracker += Time.deltaTime;
                float SinValue = Mathf.Sin(timeTracker * 1.5f) / 2 + 1;

                float hitPointY = toGround.point.y; //gets y coordinates of intersection of raycast and ground (cube);

                Debug.Log(toGround.transform.position.x);

                float currentHeight = hitPointY + minimumHeight + diamondHeight / 2 + gapHeight * SinValue;

                gameObject.transform.position = new Vector3(toGround.transform.position.x, currentHeight, toGround.transform.position.z);

                gameObject.transform.Rotate(Vector3.up, 3);
            }
            else
            {
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, 50, gameObject.transform.position.z);
            }
        }

        private void Collected()
        {
            //player collects diamond
        }

        public Vector3 GetPos()
        {
            return transform.position;
        }

        public Vector2 GetPosFlat()
        {
            return new Vector2(transform.position.x, transform.position.z);
        }
    }
}