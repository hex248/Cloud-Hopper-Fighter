using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    public int playerCount = 0;

    public List<PlayerController> players = new List<PlayerController>();

    [SerializeField] Material[] playerMaterials;

    public void PlayerSpawned(PlayerController player)
    {
        players.Add(player);
        playerCount++;
        player.model.GetComponentInChildren<Renderer>().material = playerMaterials[player.playerNumber - 1];
    }
}
