using UnityEngine;
using System.Collections;

public class GameRoom : MonoBehaviour {
    // this class GameRoom has the properties we want to compare and use with restrictions. The RoomInfo of the PhotonNetwork is not enough for us.
    public string roomName;        // Name of the roomInfo instance of the PhotonNetwork
    private int amountPlayers;      // Total Amount of players in the currentgame
    private int amountFF;           // Amount of fastfood players in the game
    private int amountSF;           // Amount of superfood players in the game

    public GameRoom(string name)
    {
        roomName = name;
    }

    public void incrementAmountFF(){
        amountFF++;
    }

    public void incrementAmountSF(){
        amountSF++;
    }

    public void decrementAmountFF()
    {
        amountFF--;
    }

    public void decrementAmountSF()
    {
        amountSF--;
    }

    public int getAmountFF(){
        return amountFF;
    }

    public int getAmountSF(){
        return amountSF;
    }

    public string getRoomName(){
        return roomName;
    }
}
