using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public enum COLOR
{
    RED,
    BLUE,
    GREEN
}
public class PhotonConnection : PunBehaviour
{
    #region SINGLETON
    private static PhotonConnection _instance;
    public static PhotonConnection GetInstance()
    {
        return _instance;
    }

    private void Awake()
    {
        _instance = this;
    }
    #endregion

    public string gameVersion = "1.0";
    public string myServerName = "NATOSPHERE";

    public COLOR myColor;

    public GameObject ownPlayer;


    public List<Player> playerList;

    //ENEMIES
    public List<GameObject> enemiesList;
    int maxEnemies = 3;
    WaitForSeconds timeBeforeSpawing = new WaitForSeconds(3.0f);
    public List<GameObject> patterPointsList;
    //SPAWNERS
    public GameManager gameManager;

    ///
    public int randomSeed;
    int playerWithLessEnemies = 0;
    void Start()
    {
        playerList = new List<Player>();
        Connect();
    }

    public void Connect()
    {
        PhotonNetwork.gameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings(gameVersion);
    }

    public override void OnConnectedToPhoton()
    {
        Debug.Log("Conectado al Master");
        StartCoroutine(ConnectLobby());
    }

    IEnumerator ConnectLobby()
    {
        yield return new WaitForSeconds(1f);
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Conectado al lobby");
        PhotonNetwork.playerName = myServerName;
        StartCoroutine(ConnectRoom());
    }

    IEnumerator ConnectRoom()
    {
        yield return new WaitForSeconds(2f);
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        for (int i = 0; i < rooms.Length; i++)
        {
            if (rooms[i].IsOpen)
            {
                PhotonNetwork.JoinRoom(rooms[i].Name);
                yield break;//return de la corrutina
            }
        }
        CreateRoom();
    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        CreateRoom();
    }

    void CreateRoom()
    {
        Debug.Log("NO HAY CUARTOS, CREANDO UNO");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 8;
        PhotonNetwork.CreateRoom(myServerName, roomOptions, TypedLobby.Default);

    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Conectado al cuarto:" + PhotonNetwork.room.Name);
        if (PhotonNetwork.isMasterClient)
        {
            randomSeed = Random.Range(0, 9999999);
            //METODO CREAR MAPA
            ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "Dificultad", 0 }, { "SeedMapa", randomSeed } };
            Debug.Log(customRoomProperties["SeedMapa"].ToString());
        }
        else
        {
            ExitGames.Client.Photon.Hashtable customRoomProperties = PhotonNetwork.room.CustomProperties;
            // randomSeed = (int)customRoomProperties["SeedMapa"];

            //METODO CREAR MAPA
            Debug.Log(randomSeed);
            CrearMapa();
            //Debug.Log((int)customRoomProperties["Dificultad"]);

        }



        ownPlayer = PhotonNetwork.Instantiate("PlayerNet", new Vector3(1f, 0.5f, 1f), Quaternion.identity, 0) as GameObject;

        PhotonNetwork.Instantiate("ColorChanger", Vector3.zero, Quaternion.identity, 0);
        patterPointsList = new List<GameObject>();
        enemiesList = new List<GameObject>();


        gameManager.CreateSpawnPoints();
        StartCoroutine(SpawnEnemy());
    }

    IEnumerator SpawnEnemy()
    {

        bool shouldICreateENemy = false;
        shouldICreateENemy = (enemiesList.Count < maxEnemies);
        yield return new WaitForSeconds(3.0f);



        if (shouldICreateENemy)
        {


            int randSpawn = Random.Range(0, gameManager.EnemySpawners.Count);
            enemiesList.Add(PhotonNetwork.Instantiate("Enemy", gameManager.EnemySpawners[randSpawn].transform.position, Quaternion.identity, 0) as GameObject);
            patterPointsList.Add(PhotonNetwork.Instantiate("EnemyPatrollingPoints_1", gameManager.EnemySpawners[randSpawn].transform.position, Quaternion.identity, 0) as GameObject);
            for (int i = 0; i < enemiesList.Count; i++)
            {
                enemiesList[i].GetComponent<EnemyIA>().patternPoint = patterPointsList[i].gameObject.transform;
            }
        }
        else
        {

            //Reciclo un enemigo ya creadoanteriormente


            for (int i = 0; i < enemiesList.Count; i++)
            {

                if (!enemiesList[i].activeSelf)
                {

                    int randSpawn = Random.Range(0, gameManager.EnemySpawners.Count);
                    enemiesList[i].SetActive(true);
                    enemiesList[i].transform.position = gameManager.EnemySpawners[randSpawn].transform.position;
                    randSpawn = Random.Range(0, gameManager.EnemySpawners.Count);
                    patterPointsList[i].transform.position = gameManager.EnemySpawners[randSpawn].transform.position;
                    break;
                }
            }
        }
        StartCoroutine(SpawnEnemy());
        //PhotonView photonView = PhotonView.Get(this);
        //photonView.RPC("CheckPlayersEnemies", PhotonTargets.All,enemiesList,patterPointsList);
    }


    void CrearMapa()
    {
        Random.InitState(randomSeed);
        Debug.Log(Random.Range(0, 1000));
        Debug.Log(Random.Range(0, 1000));
        Debug.Log(Random.Range(0, 1000));
        Debug.Log(Random.Range(0, 1000));
    }





    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log("Nuevo Jugador:" + newPlayer.NickName);

    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        Debug.Log("Jugador se desconectó:" + otherPlayer.NickName);
     
    }


    public override void OnLeftRoom()
    {
        enemiesList.Clear();
        patterPointsList.Clear();
        gameManager.EnemySpawners.Clear();
        StopAllCoroutines();
        base.OnLeftRoom();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
    }

}