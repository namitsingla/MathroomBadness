using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //keeps track of player
    #region Singleton

    public static PlayerManager instance;

    void Awake () 
    {
        instance = this;
    }

    #endregion

    public GameObject player;
    

}
