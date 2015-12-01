using UnityEngine;
using System.Collections;

public class FXmanager : MonoBehaviour {

    //FXmanager: this class only takes care of the visual effects when shooting with a gun; no collision checks are done here

    public AudioClip sniperSound;
    public GameObject sniperBulletPrefab;
    //public AudioClip sniperRicochet;

    void Start()
    {
       // audio = GetComponent<AudioSource>();
    }

    [PunRPC]
    void SniperBulletFX( Vector3 startPos, Vector3 endPos)
    {
        AudioSource.PlayClipAtPoint(sniperSound, startPos);

        GameObject sniperFX = (GameObject)Instantiate(sniperBulletPrefab, startPos, Quaternion.LookRotation(endPos - startPos));
        LineRenderer lr = sniperFX.transform.Find("LineFX").GetComponent<LineRenderer>();
        lr.SetPosition(0, startPos);
        lr.SetPosition(1, endPos);
    }

    [PunRPC]
    void RocketLauncherFX()
    {

    }

    [PunRPC]
    void PlasmaGunFX()
    {

    }
}
