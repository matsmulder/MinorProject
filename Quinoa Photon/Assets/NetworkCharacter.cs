using UnityEngine;
using System.Collections;

public class NetworkCharacter : Photon.MonoBehaviour
{
    public float updatePositionTime, updateRotationTime;
    Vector3 realPosition = Vector3.zero;
    Quaternion realRotation = Quaternion.identity;
    WeaponSwitching ws;

    void Awake()
    {
        ws = GetComponentInChildren<WeaponSwitching>();
        Debug.Log(ws);
    }


    void Update()
    {
        if(photonView.isMine)
        {
            //do nothing -- playerMovement is moving us
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, realPosition, updatePositionTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, updateRotationTime);
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);

            for (int i = 0; i < ws.weaponList.Length; i++) //ws.weaponList.Length;
            {
                stream.SendNext(ws.weaponList[i].activeSelf); //send active state of each weapon
                Debug.Log(i + "isWriting");
            }
        }
        else
        {
            // Network player, receive data
            realPosition = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();

            Debug.Log(ws.weaponList.Length + "length");
            for (int i = 0; i < ws.weaponList.Length; i++) //ws.weaponList.Length;
            {
               ws.weaponStates[i] = (bool)stream.ReceiveNext();
                ws.weaponList[i].SetActive(ws.weaponStates[i]);
                Debug.Log(ws.weaponList[i]);
                Debug.Log("test onphotonjeweetzelf");
            }
        }
    }
}