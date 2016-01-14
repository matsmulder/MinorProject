using UnityEngine;
using System.Collections;

public class TeamMember : MonoBehaviour {

    public int _teamID=0;

    public int teamID
    {
        get { return _teamID; }
    }

    public void SetTeamIDoffline(int id)
    {
        _teamID = id;
    }

    [PunRPC]
    void SetTeamID(int id)
    {
        _teamID = id;
        MeshRenderer skinColour = this.transform.GetComponentInChildren<MeshRenderer>();
        if (skinColour == null)
        {
            Debug.Log("no mesh renderer found");
        }

        if (teamID == 1) // team fastfood
        {
            skinColour.material.color = Color.red;
            gameObject.transform.FindChild("hipster").gameObject.SetActive(false);
        }
        if (teamID == 2) //team superfood
        {
            skinColour.material.color = Color.green;
            gameObject.transform.FindChild("human").gameObject.SetActive(false);
        }
        if (teamID == 0) //no team
        {
            skinColour.material.color = Color.clear;
        }
    }
}
