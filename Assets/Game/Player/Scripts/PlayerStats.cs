using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float base_HP;
    public float base_Shield = 50;
    public float base_DamageMeele = 10;   
    public float base_DamageRange = 5;
    public float base_MeleeSpeed = 2f;
    public float base_speed = 2f;    
    public float base_ShootingSpeed = 2f;

    //Modifiers
    public float m_Speed;
    public float m_Shield;
    public float m_HP;
    public float m_ShootingSpeed;
    public float m_MeeleSpeed;
    public float m_DamageRange;
    public float m_DamageMelee;


    void Start()
    {
        m_Speed = base_speed;
        m_HP = base_HP;
        m_Shield = base_Shield;
        m_DamageMelee = base_DamageRange;
        m_DamageRange = base_DamageRange;
        m_MeeleSpeed = base_MeleeSpeed;
        m_ShootingSpeed = base_ShootingSpeed;


        //Getting info from other scripts to use in modifiers
        //m_ShootingSpeed = speed;
        //speed = gameObject.GetComponent<Player>().timeBeforeAnotherShoot;
        //shield = gameObject.GetComponent<pLayerHealth>().escudito;  cannot access script 
        //HP = gameObject.GetComponent<pLayerHealth>().currentHealth;  cannot access script 
    }

    public static void UpdateMods()
    {

    }
    

}
