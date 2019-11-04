using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class ColorChanger : PunBehaviour
{
    public COLOR myColor;
    public int id;

    private void Start()
    {
        StartCoroutine(SendColor());
    }
    IEnumerator SendColor()
    {
        yield return new WaitForSeconds(1);
        object[] parameters = new object[2];
        parameters[1] = PhotonConnection.GetInstance().myColor;
        parameters[0] = PhotonConnection.GetInstance().ownPlayer.GetComponent<PhotonView>().ownerId;
        PhotonNetwork.RPC(GetComponent<PhotonView>(), "ChangeColor", PhotonTargets.AllBuffered, false, parameters);
    }

    [PunRPC]
    public void ChangeColor(object[] _parameters)
    {
        COLOR c = (COLOR)_parameters[1];
        int id = (int)_parameters[0];
        for (int i = 0; i < PhotonConnection.GetInstance().playerList.Count; i++)
        {
            if (PhotonConnection.GetInstance().playerList[i] != null)
            {
                if (PhotonConnection.GetInstance().playerList[i].photonView.ownerId == id)
                {
                    PhotonConnection.GetInstance().playerList[i].ChangeColorByName(c);
                }
            }
        }
    }
}
