using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerManager : MonoBehaviour
{
    [Header("Players")]
    public int playerCount = 0;

    public List<PlayerController> players = new List<PlayerController>();

    [Header("Rendering")]
    [SerializeField] Material[] playerMaterials;
    [SerializeField] RenderTexture[] normalRenderTextures;
    [SerializeField] RenderTexture[] tallRenderTextures;
    ScreenManager screenManager;

    [Header("Attacks")]
    public List<Attack> attacks = new List<Attack>();

    private void Start()
    {
        screenManager = FindObjectOfType<ScreenManager>();
    }

    public void PlayerSpawned(PlayerController player)
    {
        players.Add(player);
        playerCount++;
        StartCoroutine(screenManager.PlayerJoined(playerCount));
        player.model.GetComponentInChildren<Renderer>().material = playerMaterials[player.playerNumber - 1];

        switch (playerCount)
        {
            case 1:
                players[0].camera.targetTexture = normalRenderTextures[0];
                break;
            case 2:
                players[0].camera.targetTexture = tallRenderTextures[0];
                players[1].camera.targetTexture = tallRenderTextures[1];
                break;
            case 3:
                players[0].camera.targetTexture = normalRenderTextures[0];
                players[1].camera.targetTexture = tallRenderTextures[1];
                players[2].camera.targetTexture = normalRenderTextures[2];
                break;
            case 4:
                players[0].camera.targetTexture = normalRenderTextures[0];
                players[1].camera.targetTexture = normalRenderTextures[1];
                players[2].camera.targetTexture = normalRenderTextures[2];
                players[3].camera.targetTexture = normalRenderTextures[3];
                break;
        }
    }

    public void KillPlayer(int playerNumber)
    {
        // show player death screen

        Destroy(players[playerNumber - 1].gameObject);
        players.Remove(players[playerNumber - 1]);
        playerCount--;
        // update screens
        screenManager.UpdateSplitScreen();
    }
}
