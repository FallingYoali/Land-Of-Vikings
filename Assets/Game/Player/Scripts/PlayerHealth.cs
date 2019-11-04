/* Written by Kaz Crowe */
/* PlayerHealth.cs */
using UnityEngine;
using System.Collections;

namespace SimpleHealthBar_SpaceshipExample
{
	public class PlayerHealth : MonoBehaviour
	{
		static PlayerHealth instance;
		public static PlayerHealth Instance { get { return instance; } }
	
        //VIDA VARIABLES
		public int maxHealth = 100;
		float currentHealth = 0;
		
        //ESCUDO VARIABLES 
		float Escudito = 0;
		public int maxEscudito = 25;
		float regenShieldTimer = 0.0f;
		public float regenShieldTimerMax = 1.0f;

        
		public SimpleHealthBar healthBar;
		public SimpleHealthBar shieldBar;

	
		void Awake ()
		{
			// If the instance variable is already assigned, then there are multiple player health scripts in the scene. Inform the user.
			if( instance != null )
				//Debug.LogError( "Agregale primero la vida baboso en los prefabs" );
			
			
			instance = GetComponent<PlayerHealth>();
		}

		void Start ()
		{
			// PARA ESTABLECER LA VIDA MAXIMA Y EL ESCUDO
			currentHealth = maxHealth;
			Escudito = maxEscudito;

			// SE van actualizando la vida y los escudos
			healthBar.UpdateBar( currentHealth, maxHealth );
			shieldBar.UpdateBar( Escudito, maxEscudito );
		}

		void Update ()
		{
			
            //si el escudo es menor al maximo, y el cooldown del regenShield no esta aplicandose entonces : 
			if( Escudito < maxEscudito && regenShieldTimer <= 0 )
			{
				//Incrementa el escudo
				Escudito += Time.deltaTime * 5;

				// ACTUALIZA LOS VALORES DEL ESCUDO
				shieldBar.UpdateBar( Escudito, maxEscudito );
			}
            //AQUI TOMA EL DAÑO QUE SE REFLEJARA EN LA BARRA DE VIDA Y ESCUDO 
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                TakeDamage(10);
            }

            
			
            //SI EL REGENERAR ESCUDO ES MAYOR DE 0 ENTONCES DECREMENTA EL TIEMPO
			if( regenShieldTimer > 0 )
				regenShieldTimer -= Time.deltaTime;
		}
        
		public void HealPlayer ()
		{
			// incrementar la vida por un 25%
			currentHealth += ( maxHealth / 4 );

			
            //SI LA VIDA ACTUAL ES MAYOR QUE LA MAX, ENTONCES SE ACTUALIZA A QUE ESA SERA LA MAX, EN CASO DE CURARSE CLARO
			if( currentHealth > maxHealth )
				currentHealth = maxHealth;

			//SE ACTUALIZA LA BARRA
			healthBar.UpdateBar( currentHealth, maxHealth );
		}
        //FUNCION PARA HACER DAÑO 
		public void TakeDamage ( int damage )
		{
			
			// SI EL ESCUDO ES MAYOR A 0 
			if( Escudito > 0 )
			{

				// REDUCE EL ESCUDO DEL DAÑO HECHO
				Escudito -= damage;

		
                //SI EL ESCUDO ES MENOR A 0 
				if( Escudito < 0 )
				{
					
                    //REDUCE LA VIDA POR CUANTO DAÑO HAYA PASADO DEL ESCUDO 
					currentHealth -= Escudito * -1;

					// ESCUDO A 0
					Escudito = 0;
				}
			}
			// SI NO HAY ESCUDO ENTONCES HAZLE DAÑO
			else
				currentHealth -= damage;

			// SI LA VIDA ES MENOR A 0 O IGUAL
			if( currentHealth <= 0 )
			{
				// SIMPLEMENTE PARA ESTETICA Y QUE NO QUEDE EL -1 O NUMEROS NEGATIVOS
				currentHealth = 0;

				// CORRE LA FUNCION DE MORIR
				Death();
			}
			
		

			// UPDATE DE EL ESCUDO Y LA VIDA BARRAS
			healthBar.UpdateBar( currentHealth, maxHealth );
			shieldBar.UpdateBar( Escudito, maxEscudito );

			//RESETEAMOS EL REGENSHIELD
			regenShieldTimer = regenShieldTimerMax;
		}

		public void Death ()
		{
			//AQUI ENSEÑARIA LA ESCENA DE DEATH
			
			Destroy( gameObject );
		}

		
	}
}