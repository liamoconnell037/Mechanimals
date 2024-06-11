using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    GameState state;
    [SerializeField] PlayerController playerController;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;
    [SerializeField] MapArea mapArea;
    private void Update() {
        if(state == GameState.FreeRoam) {
            playerController.HandleUpdate();
        } else if (state == GameState.Battle) {
            battleSystem.HandleUpdate();
        }
    }
    
    private void Start() {
        playerController.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += LeaveBattle;
    }
    void LeaveBattle(bool win) {
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
        battleSystem.resetEN();
    }
    void StartBattle() {
        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);
        battleSystem.StartBattle(playerController.GetComponent<MechanimalParty>(), mapArea.GetWildMechanimal());
    }

}

public enum GameState {
    FreeRoam,
    Battle
}
