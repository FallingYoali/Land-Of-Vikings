using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Items;

public enum PlayerState
{
    NORMAL,
    BLEEDING,
    POISONED
};
public class Player : PunBehaviour
{

    public Weapon melee, ranged;
    public Sprite[] meleeSprites;
    public Sprite[] rangedSprites;
    public PlayerState myState;
    PlayerStats _myPlayerStats;
    List<GameObject> range_attack_Objects = new List<GameObject>();
    public GameObject prefab_range_attack;
    public Rigidbody player_rigidbody;
    WaitForSeconds proyectile_Timer = new WaitForSeconds(2.0f);
    public Vector3 posicionJugador;
    private bool facingRight = true;
    public int ID;
    public Color[] colors;
    public int enemyCount = 0;
    //melee attack HIT BOX
    public GameObject BasicHitBox;

    void Start()
    {
        melee = new Weapon();
        ranged = new Weapon();

        PhotonConnection.GetInstance().playerList.Add(this);
        if (photonView.isMine)
        {
            Camera.main.transform.parent = transform;
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localPosition = new Vector3(0, 8, -25);
        }

        InitBaseWeapons(melee, ranged); // common
        //InitRandomWeapons(melee, ranged); // random
        DebugWeapons(melee, ranged);

        facingRight = false;
        player_rigidbody = GetComponent<Rigidbody>();
        _myPlayerStats = GetComponent<PlayerStats>();
        ID = this.gameObject.GetComponent<PhotonView>().viewID;
        //hit box is deactivated unless the player hits
        BasicHitBox.GetComponent<MeshRenderer>().enabled = false;
        BasicHitBox.GetComponent<Collider>().enabled = false;
    }

    public void ChangeColorByName(COLOR _c)
    {
        Debug.Log("Cambiar color");
       // GetComponent<MeshRenderer>().material.SetColor("_Color", colors[(int)_c]);
    }

    GameObject SpawnRangeAttackObject(GameObject desired_prefab, Vector3 position)
    {
        for (int i = 0; i < range_attack_Objects.Count; i++)
        {
            if (range_attack_Objects[i].activeSelf == false)
            {
                //Si vamos a meter diferente tipos de proyectiles (hachas, flechas, etc), aquí hay que colocar el nombre de los prefabas
                //Hay que copiar todo dentro del IF de "prefab.name == **** " y solo cambiarle el nombre
                if (desired_prefab.name == "Arrow")
                {
                    if (range_attack_Objects[i].gameObject.name == "Arrow(Clone)")
                    {

                        object[] parameters2 = new object[3];

                        parameters2[2] = position;
                        parameters2[1] = true;
                        parameters2[0] = facingRight;
                        // range_attack_Objects[i].GetComponent<Transform>().position = position;
                        //range_attack_Objects[i].SetActive(true);
                        // range_attack_Objects[i].GetComponent<projectile>().moveProjectile(facingRight);

                        range_attack_Objects[i].GetComponent<projectile>().ReactivarFlecha(parameters2);
                        return range_attack_Objects[i];

                    }

                }

            }
        }

        GameObject go = PhotonNetwork.Instantiate(prefab_range_attack.name.ToString(), position, Quaternion.identity, 0);
        //ESPERAR UN MOMENTO PARA PODER HACER ESTO O HACER UN BULLET MANAGER APARTE QUE SE ENCARGUÉ ESPECIFICAMENTE DE ESTO
        object[] parameters = new object[3];
        parameters[2] = ID;
        parameters[1] = facingRight;
        parameters[0] = new Vector3(-90, 90, 0);
        //PhotonNetwork.RPC(go.GetComponent<PhotonView>(), "ArrowStart", PhotonTargets.AllBuffered, false, parameters);
        go.GetComponent<projectile>().PrepareRPC(parameters);
        // go.transform.eulerAngles = new Vector3(-90, 90, 0);
        // go.GetComponent<projectile>().owner = ID;
        // go.GetComponent<projectile>().moveProjectile(facingRight);
        range_attack_Objects.Add(go);
        //StartCoroutine(MoveProyectile(go));
        return go;
    }
    public IEnumerator MoveProyectile(GameObject proyectile)
    {
        //Mover el disparo y luego desactivarlo para volverse a usar en el futuro
        if (!facingRight)
        {
            proyectile.GetComponent<Rigidbody>().AddForce(Vector3.right * 350f);
        }
        else
        {
            proyectile.GetComponent<Rigidbody>().AddForce(Vector3.left * 350f);
        }
        yield return proyectile_Timer;
        proyectile.GetComponent<Rigidbody>().velocity = Vector3.zero;
        proyectile.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        proyectile.SetActive(false);

    }


    void Movement()
    {
        //Checar que lado esta mirando para cambiar su la escala (voltear)
        if (Input.GetAxis("Horizontal") > 0 && facingRight || Input.GetAxis("Horizontal") < 0 && !facingRight)
        {
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;

        }
        float h = _myPlayerStats.m_Speed * Input.GetAxis("Horizontal");
        float v = _myPlayerStats.m_Speed * Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(h, 0.0f, v);
        player_rigidbody.velocity = movement * _myPlayerStats.m_Speed;


    }

    public IEnumerator BasicAttack()
    {

        BasicHitBox.GetComponent<Collider>().enabled = true;
        BasicHitBox.GetComponent<MeshRenderer>().enabled = true;
        yield return new WaitForSeconds(0.5f);
        BasicHitBox.GetComponent<Collider>().enabled = false;   //will go back to waiting if another object is hit after detecting one with space. Will need counter for animation
        BasicHitBox.GetComponent<MeshRenderer>().enabled = false;
    }



    void AttackInput()
    {
        //Primary Attack
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(BasicAttack());
        }

        //Secondary attack
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (_myPlayerStats.m_ShootingSpeed >= 1)
            {
                _myPlayerStats.m_ShootingSpeed = 0f;
                SpawnRangeAttackObject(prefab_range_attack, transform.position);
            }

        }
    }

    void UpdateVariables()
    {
        //Debug.Log(timeBeforeAnotherShoot);
        //Todas las variables de las estádisticas se deben actualizar aquí
        posicionJugador = transform.position;
        //Debug.Log(transform.position);


        //Timer para disparar proyectiles
        if (_myPlayerStats.m_ShootingSpeed <= 1f)
        {
            _myPlayerStats.m_ShootingSpeed += Time.deltaTime;
        }

    }

    void OnTriggerEnter(Collider objeto)
    {

    }

    void OnTriggerStay(Collider objeto)
    {
        if (objeto.CompareTag("Melee") || objeto.CompareTag("Rango"))
            if (Input.GetKeyDown(KeyCode.E))
            {
                WeaponPickup weapon = objeto.GetComponent<WeaponPickup>();
                ChangeWeapon(weapon.type, weapon.rarity, PhotonConnection.GetInstance().randomSeed);
            }
        
    }

    void InitRandomWeapons(Weapon melee, Weapon ranged)
    {
        melee.rarity = (Items.WeaponRarity)Random.Range(1, 5);
        melee.sprite = meleeSprites[(int)melee.rarity];
        melee.stats = WeaponStats.SetStats(melee.stats, PhotonConnection.GetInstance().randomSeed, melee.type, melee.rarity);

        ranged.rarity = (Items.WeaponRarity)Random.Range(1, 5);
        ranged.sprite = rangedSprites[(int)ranged.rarity];
        ranged.stats = WeaponStats.SetStats(ranged.stats, PhotonConnection.GetInstance().randomSeed, ranged.type, ranged.rarity);

        if ((int)melee.rarity > 1)
        {
            WeaponStats.SetMeleeModifier(ref melee.stats, melee.stats.mod1);
            if (melee.rarity != Items.WeaponRarity.RARE)
            {
                WeaponStats.SetMeleeModifier(ref melee.stats, melee.stats.mod2);
            }
        }

        if ((int)ranged.rarity > 1)
        {
            WeaponStats.SetMeleeModifier(ref ranged.stats, ranged.stats.mod1);
            if (ranged.rarity != Items.WeaponRarity.RARE)
            {
                WeaponStats.SetMeleeModifier(ref ranged.stats, ranged.stats.mod2);
            }
        }
    }

    void InitBaseWeapons(Weapon melee, Weapon ranged)
    {
        melee.type = Items.WeaponType.MELEE;
        melee.rarity = Items.WeaponRarity.COMMON;
        melee.sprite = meleeSprites[(int)melee.rarity];
        melee.stats = WeaponStats.SetStats(melee.stats, PhotonConnection.GetInstance().randomSeed, melee.type, melee.rarity);

        ranged.type = Items.WeaponType.RANGED;
        ranged.rarity = Items.WeaponRarity.COMMON;
        ranged.sprite = meleeSprites[(int)ranged.rarity];
        ranged.stats = WeaponStats.SetStats(ranged.stats, PhotonConnection.GetInstance().randomSeed, ranged.type, ranged.rarity);
    }

    void DebugWeapons(Weapon melee, Weapon ranged)
    {
        Debug.Log("Melee = DMG: " + melee.stats.damage + " | RoF: " + melee.stats.rOF + " | ArmourPen: " + melee.stats.armourPen + " | Crit%: " + melee.stats.critChance + " | wear: " + melee.stats.wear);
        Debug.Log("Modifier 1: " + melee.stats.mod1);
        Debug.Log("Modifier 2: " + melee.stats.mod2);

        Debug.Log("Ranged = DMG: " + ranged.stats.damage + " | RoF: " + ranged.stats.rOF + " | ArmourPen: " + ranged.stats.armourPen + " | Crit%: " + ranged.stats.critChance + " | wear: " + ranged.stats.wear);
        Debug.Log("Modifier 1: " + ranged.stats.mod1);
        Debug.Log("Modifier 2: " + ranged.stats.mod2);
        Debug.Log(ranged.stats);
    }

    #region IPunObservable

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {

            if (_myPlayerStats == null)
            {
                Debug.Log("stats vacio writing");
            }
            else
            {
                //stream.SendNext(transform.position);
                stream.SendNext(_myPlayerStats.m_Speed);
                stream.SendNext(_myPlayerStats.m_Shield);
                stream.SendNext(_myPlayerStats.m_HP);
                stream.SendNext(_myPlayerStats.m_ShootingSpeed);
                stream.SendNext(_myPlayerStats.m_MeeleSpeed);
                stream.SendNext(_myPlayerStats.m_DamageRange);
                stream.SendNext(_myPlayerStats.m_DamageMelee);
            }

        }
        else
        {
            if (_myPlayerStats == null)
            {
                Debug.Log("stats vacio");
            }
            else
            {
                //transform.position = (Vector3)stream.ReceiveNext();
                _myPlayerStats.m_Speed = (float)stream.ReceiveNext();
                _myPlayerStats.m_Shield = (float)stream.ReceiveNext();
                _myPlayerStats.m_HP = (float)stream.ReceiveNext();
                _myPlayerStats.m_ShootingSpeed = (float)stream.ReceiveNext();
                _myPlayerStats.m_MeeleSpeed = (float)stream.ReceiveNext();
                _myPlayerStats.m_DamageRange = (float)stream.ReceiveNext();
                _myPlayerStats.m_DamageMelee = (float)stream.ReceiveNext();


            }

        }
    }
    #endregion
    // Update is called once per frame

    public void ChangeWeapon(WeaponType type, WeaponRarity rarity, int seed)
    {
        object[] objects = new object[4];

        objects[0] = photonView.viewID;
        objects[1] = seed;
        objects[2] = type;
        objects[3] = rarity;

        if (type == WeaponType.MELEE)
            PhotonNetwork.RPC(photonView, "GetMeleeWeapon", PhotonTargets.All, false, objects);
        else
            PhotonNetwork.RPC(photonView, "GetRangedWeapon", PhotonTargets.All, false, objects);
    }


    [PunRPC]
    public void GetMeleeWeapon(object[] objects)
    {
        int playerID = (int)objects[0];
        if (playerID == photonView.viewID)
        {
            melee = new Weapon();
            melee.stats = WeaponStats.SetStats(melee.stats, (int)objects[1], (WeaponType)objects[2], (WeaponRarity)objects[3]);

        }
    }

    [PunRPC]
    public void GetRangedWeapon(object[] objects)
    {
        int playerID = (int)objects[0];
        if (playerID == photonView.viewID)
        {
            ranged = new Weapon();
            ranged.stats = WeaponStats.SetStats(ranged.stats, (int)objects[1], (WeaponType)objects[2], (WeaponRarity)objects[3]);
        }
    }



    void Update()
    {
        if (photonView.isMine)
        {
            Movement();
            UpdateVariables();
            AttackInput();
        }

    }
}
