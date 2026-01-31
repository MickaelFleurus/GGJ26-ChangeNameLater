using System;
using UnityEngine;

/// <summary>
/// This class hold the events that tells the game what is happening
/// If you want to subscribe to this
/// In the onEnable function write for example
/// GameEvents.OnPickUpItem += PickUpItemLogic;
/// GameEvents.OnPickUpItem is an event, you can choose other events.
/// PickUpItemLogic is the function you want to be run when this event happens.
/// In that function you could have like moneygathered, pickupsound, add item to ui etc
/// ask me
/// </summary>
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

    //Player
    public static event Action OnPlayerWalking;
    public static event Action OnPlayerNotWalking;

    //LootCollectionEvents
    public static event Action onLootCollected;
    public static event Action OnDoorOpen;

    public static event Action OnUIClick;


}
