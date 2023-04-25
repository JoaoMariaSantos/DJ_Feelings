using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : MonoBehaviour
{
    public float minimumDistanceToPlayer;
    public GameObject player;
    public float diamondheight;
    public float minimumHeight;
    public float maximumHeight;
    private float gapHeight;
    private float timeTracker;
    public LayerMask whatIsGround;

    void Start()
    {
        float gapHeight = maximumHeight - minimumHeight;
    }

    // Update is called once per frame
    void Update()
    {
        timeTracker += Time.deltaTime;
        float SinValue = Mathf.Sin(timeTracker);

        Ray downRay = new Ray(transform.position, -Vector3.up);
        RaycastHit toGround;

        bool onGround = Physics.Raycast(downRay, out toGround, Mathf.Infinity, whatIsGround);

        if(onGround){
            float hitPointY = toGround.point.y; //gets y coordinates of intersection of raycast and ground (cube);

            float currentHeight = hitPointY + minimumHeight + diamondheight/2 + gapHeight * SinValue;

            gameObject.transform.position = new Vector3(gameObject.transform.position.x, currentHeight, gameObject.transform.position.z);
        }
    }

    private void Collected()
    {
        //player collects diamond
    }
}
