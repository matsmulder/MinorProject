using UnityEngine;
using System.Collections;

public class FXmanager : MonoBehaviour {

    public AudioClip sniperSound;
    //public AudioClip sniperRicochet;

    void Start()
    {
       // audio = GetComponent<AudioSource>();
    }

    [PunRPC]
    void SniperBulletFX( Vector3 startPos, Vector3 endPos)
    {
        AudioSource.PlayClipAtPoint(sniperSound, startPos);
    }




}
