using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newLookForPlayerData", menuName = "Data/State Data/Look For Player State Data")]
public class D_LookForPlayer : ScriptableObject
{
    public int amountOfTurns = 2;

    public float timeBetweenTurns = 0.75f;
}
