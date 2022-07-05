using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack", menuName = "ScriptableObjects/Attack", order = 1)]
public class Attack : ScriptableObject
{
    public string attackName = "";
    public int damage = 0;
    public float hitDuration = 0.0f;
    public float cooldown = 0.0f;
}
