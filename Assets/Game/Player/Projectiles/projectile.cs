using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
public class projectile : PunBehaviour
{

    void OnTriggerEnter(Collider other)
    {
       if (other.gameObject.CompareTag("Player"))
       {
            if(!photonView.isMine)
                {
                    other.gameObject.GetComponent<PlayerStats>().m_HP -= 10;
                    StartCoroutine(DeathTime());
               
                    Debug.Log(other.gameObject.GetComponent<PlayerStats>().m_HP);
                }
              
       }

        if (other.gameObject.CompareTag("ENEMIES"))
        {
            //DAÑO AÑ ENEMIGO
            StartCoroutine(DeathTime());
            other.gameObject.GetComponent<EnemyIA>().HP -= 50;
        }
    }


    IEnumerator DeathTime()
    {
        yield return new WaitForEndOfFrame();
        this.gameObject.SetActive(false);
    }
}