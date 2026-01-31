using System;
using UnityEngine;

public static class GameState 
{

    public static event Action OnMaskEquipped;
    public static event Action OnMaskOff;

    public static event Action OnPlayerLookingAtEnemy;
    public static event Action OnPlayerLookingAway;

    public static event Action OnPickUpItem;

    public static event Action OnDeath;
}
