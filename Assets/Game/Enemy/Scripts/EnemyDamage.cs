using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int life;
    public int cool_down_attack;


    public EnemyIA IA;
    public GameObject HitBoxLeft;
    public GameObject HitBoxRight;
    public Transform player;
    public int attack_distance;
    

    //Attack Right
    bool can_attack_right = true;
    bool activate_routine_right = false;
    
    //Attack Left
    bool can_attack_left = true;
    bool activate_routine_left = false;

    private void Start()
    {
        HitBoxLeft.SetActive(false);
        HitBoxRight.SetActive(false);
    }

    private void Update()
    {
        //Enemy Attacks
        if(Vector3.Distance(player.position, transform.position) <= IA.stoppingDistance)  //if the enemy is close enough, it will attack
        {
            if(this.transform.position.x > player.transform.position.x  && can_attack_left)   //means the player is left
            {
                HitBoxRight.SetActive(true);
                //HitBoxRight.GetComponent<MeshRenderer>().enabled = true;   //change to actually see the trigger object on screen
                StartCoroutine(CoolDownLeft());
                can_attack_left = false;
                Debug.Log("Enemy Attack Enters left!");
            }
            if (this.transform.position.x < player.transform.position.x && can_attack_right)   //means the player is right
            {
                HitBoxLeft.SetActive(true);
               // HitBoxLeft.GetComponent<MeshRenderer>().enabled = true;   //change to actually see the trigger object on screen
                can_attack_right = false;
                Debug.Log("Enemy Attack Enters right!");
                StartCoroutine(CoolDownRight());
            }
        }
    }

    //Wait until another attack can be made (CoolDowns)
    IEnumerator CoolDownRight()
    {
        yield return new WaitForSeconds(cool_down_attack);
        HitBoxRight.SetActive(false);   //why this not work right??
        can_attack_right = true;
    }
    IEnumerator CoolDownLeft()
    {
        yield return new WaitForSeconds(cool_down_attack);
        HitBoxLeft.SetActive(false);   //why this not work right??
        can_attack_left = true;

    } 
    
    
    //Every time an enemy comes in contacts with the tag "Proyectile." its life will decrease by one.
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Proyectile")  && col.gameObject.CompareTag("Melee"))
        {
            if(life >= 1)
            {
                life--;
            }

            if (life <= 0)
            {
                Death();
            }
        }
    }
    void Death()
    {
        this.gameObject.SetActive(false);
    }

}


