using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    [SerializeField] PlayerController player;

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player Attack")
        {
            PlayerController otherPlayer = other.GetComponent<AttackBox>().player;
            if (otherPlayer.playerNumber == player.playerNumber) return;
            if (otherPlayer.attacking == true && !otherPlayer.hitPlayer)
            {
                // take hit
                otherPlayer.hitPlayer = true;
                player.TakeHit(otherPlayer.currentAttack, otherPlayer);
            }
        }
    }
}
