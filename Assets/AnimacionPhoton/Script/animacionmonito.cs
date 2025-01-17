﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animacionmonito : MonoBehaviour
{
    public int velocidad;
    // Start is called before the first frame update
   
    bool LookRight = true;

    public GameObject jugador;
    public Animator animator;
    int Ataque=0;
    bool vivo = false;

    bool opcion = false;

    public PlayerStats jugadorstats;

    // Update is called once per frame
    void Update()
    {
        #region GetHit
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Hurt"))
        {

            if(jugadorstats.m_HP<=0)
            {
                vivo = true;
                animator.SetBool("Morir",true);
            }
            else
            {
             return;

            }
            
        }
        #endregion
        #region AtaqueMelee
        if (opcion == false)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Ataque++;
                if (Ataque == 1)
                {
                    animator.SetInteger("Ataque", Ataque);
                    animator.SetBool("Arma", opcion);
                }

            }
            else
            {
                Ataque = 0;
                animator.SetInteger("Ataque", Ataque);
                animator.SetBool("Arma", opcion);
            }
        }
       if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            opcion = true;
        }
        if(opcion!=false)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Ataque= 2;
                
                if(Ataque == 2)
                {
                    animator.SetInteger("Ataque", Ataque);
                    animator.SetBool("Arma", opcion);
                }
                   

            }
            else
            {
                Ataque = 3;
                animator.SetInteger("Ataque", Ataque);
                animator.SetBool("Arma", opcion);
            }
        }
        
        
        
        #endregion 



        #region Move
        bool isMoving = false;

        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.back * velocidad * Time.deltaTime);
            isMoving = true;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * velocidad * Time.deltaTime);
            isMoving = true;
        }


        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector3.left * velocidad * Time.deltaTime);
            isMoving = true;
            /*if (LookRight)
            {
                LookRight = false;
                jugador.transform.localScale = new Vector3(jugador.transform.localScale.x * -1, jugador.transform.localScale.y, jugador.transform.localScale.z);
            }*/
        }
        else if (Input.GetKey(KeyCode.D))
        {
            isMoving = true;
            transform.Translate(Vector3.right * velocidad * Time.deltaTime);
            /*if (!LookRight)
            {
                LookRight = true;
                jugador.transform.localScale = new Vector3(jugador.transform.localScale.x * -1, jugador.transform.localScale.y, jugador.transform.localScale.z);
            }*/
        }

        animator.SetBool("run", isMoving);
        #endregion
    }
    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("ENEMIES"))
        {
            animator.SetTrigger("hit");

            Debug.Log(jugadorstats.m_HP);

            animator.SetBool("Morir", vivo);
            jugadorstats.m_HP = jugadorstats.m_HP - 5;
            //Debug.Log(m_HP);
        }
    }
}
