using System;
using UnityEngine;

public static class GameEvents
{
    //Mask
    public static event Action OnMaskEquipped;
    public static event Action OnMaskOff;
    public static void InvokeMaskEquipped() => OnMaskEquipped?.Invoke();
    public static void InvokeMaskOff() => OnMaskOff?.Invoke();

    //Enemy
    public static event Action OnPlayerLookingAtEnemy;
    public static event Action OnPlayerLookingAway;
    public static event Action OnEnemyMoveHead;
    public static void InvokePlayerLookingAtEnemy() => OnPlayerLookingAtEnemy?.Invoke();
    public static void InvokePlayerLookingAway() => OnPlayerLookingAway?.Invoke();
    public static void InvokeEnemyMoveHead() => OnEnemyMoveHead?.Invoke();

    //Item
    public static event Action OnPickUpItem;
    public static void InvokePickUpItem() => OnPickUpItem?.Invoke();
    public static event Action<int, LootType> OnLootCollectedWithData;
    public static void InvokeLootCollected(int value, LootType lootType)
    {
        OnLootCollectedWithData?.Invoke(value, lootType);
    }

    //GameStates
    public static event Action OnGameLost;
    public static event Action OnGameWon;
    public static event Action OnInGame;
    public static event Action OnDoorUnlocked;
    public static void InvokeGameLost() => OnGameLost?.Invoke();
    public static void InvokeGameWon() => OnGameWon?.Invoke();
    public static void InvokeInGame() => OnInGame?.Invoke();

    public static void InvokeDoorUnlocked() => OnDoorUnlocked?.Invoke();

    //Player
    public static event Action OnPlayerWalking;
    public static event Action OnPlayerNotWalking;
    public static void InvokePlayerWalking() => OnPlayerWalking?.Invoke();        
    public static void InvokePlayerNotWalking() => OnPlayerNotWalking?.Invoke();  

    //Environment
    public static int CurrentMoney { get; set; }
    public static event Action onLootCollected;
    public static event Action OnDoorOpen;
    public static event Action OnUIClick;
    public static void InvokeLootCollected() => onLootCollected?.Invoke();  
    public static void InvokeDoorOpen() => OnDoorOpen?.Invoke();            
    public static void InvokeUIClick() => OnUIClick?.Invoke();              
}