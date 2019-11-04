using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeeleAttack : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ENEMIES"))
        {
            Debug.Log("You have hit something!");
        }
    }
}
