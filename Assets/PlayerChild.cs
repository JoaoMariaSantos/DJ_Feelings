using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChild : MonoBehaviour
{
    public Transform player;
    void Update()
    {
        transform.position = player.position;
    }
}
