using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogueBox dialogue;
    BattleState state;
    public event Action<bool> OnBattleOver;
    int currentAction, currentMove;

    private MechanimalParty playerParty;
    private Mechanimal wild;
    public void StartBattle(MechanimalParty player, Mechanimal wild) {
        playerParty = player;
        this.wild = wild;
        StartCoroutine(SetupBattle());
    }

    public void resetEN() {
        playerUnit.Mechanimal.En = playerUnit.Mechanimal.MaxEn;
        playerHud.UpdateEn();
    }

    public IEnumerator SetupBattle() {
        state = BattleState.Busy;
        currentAction = 0;
        currentMove = 0;
        dialogue.resetActionSelection();
        dialogue.resetMoveSelection();
        playerUnit.Setup(playerParty.GetHealthyMechanimal());
        playerHud.SetData(playerUnit.Mechanimal);
        playerHud.UpdateEn();
        playerHud.UpdateHp();
        enemyUnit.Setup(wild);
        enemyHud.SetData(enemyUnit.Mechanimal);
        enemyHud.UpdateEn();
        enemyHud.UpdateHp();
        dialogue.SetMoveNames(playerUnit.Mechanimal.Moves);
        
        yield return StartCoroutine(dialogue.TypeDialogue($"A wild {enemyUnit.Mechanimal.Base.Name} appeared!"));
        yield return new WaitForSeconds(0.5f); //wait one second
        
        dialogue.UpdateActionSelection(currentAction);
        dialogue.UpdateMoveSelection(currentMove, playerUnit.Mechanimal.Moves[currentMove]);
        StartCoroutine(PlayerAction());
    }
    private IEnumerator PlayerAction() {
        dialogue.EnableActionSelector(true);
        dialogue.EnableDialogueText(true);
        dialogue.EnableMoveSelector(false);
        yield return dialogue.TypeDialogue("Choose an action.");
        state = BattleState.PlayerAction;
        
    }

    void OpenPartyScreen() {
        //todo: AHHHHHHHHHHHHHHHHHHH
    }
    private void PlayerMove() {
        state = BattleState.PlayerMove;
        dialogue.EnableActionSelector(false);
        dialogue.EnableDialogueText(false);
        dialogue.EnableMoveSelector(true);
    }
    public void HandleUpdate() {
        switch(state) {
            case BattleState.PlayerAction:
                StartCoroutine(HandleActionSelection());
                break;
            case BattleState.PlayerMove:
                HandleMoveSelection();
                break;
        }
    }

    void HandleMoveSelection() {
        if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow)) {
            if((currentMove&1)==0) { // left side
                if(currentMove < playerUnit.Mechanimal.Moves.Count - 1)
                    currentMove++;
            } else { // right side
                currentMove--;
            }
            dialogue.UpdateMoveSelection(currentMove, playerUnit.Mechanimal.Moves[currentMove]);
        } else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow)) {
            if(currentMove < 2) { // top row
                if(currentMove < playerUnit.Mechanimal.Moves.Count - 2)
                    currentMove += 2;
            } else { // bottom row
                currentMove -= 2;
            }
            dialogue.UpdateMoveSelection(currentMove, playerUnit.Mechanimal.Moves[currentMove]);
        } else if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.X)) { // go back one layer
            StartCoroutine(PlayerAction());
        } else if(Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return)) {
           dialogue.EnableMoveSelector(false);
           dialogue.EnableDialogueText(true);
           StartCoroutine(PerformPlayerMove());
        }
    }

    IEnumerator PerformPlayerMove() {
        state = BattleState.Busy;
        var move = playerUnit.Mechanimal.Moves[currentMove];
        if(move.Base.Cost > playerUnit.Mechanimal.En) {
            yield return dialogue.TypeDialogue($"{playerUnit.Mechanimal.Base.Name} does not have enough EN to use {move.Base.Name}!");
            yield return new WaitForSeconds(0.5f);
            dialogue.EnableMoveSelector(true);
            dialogue.EnableDialogueText(false);
            StartCoroutine(PlayerAction());           
        }
        else {
            yield return dialogue.TypeDialogue($"{playerUnit.Mechanimal.Base.Name} used {move.Base.Name}");
            playerUnit.AttackAnimation();
            yield return new WaitForSeconds(0.5f);
            var damageDetails = enemyUnit.Mechanimal.TakeDamage(move, playerUnit.Mechanimal);
            playerUnit.Mechanimal.En -= move.Base.Cost;
            playerHud.UpdateEn();
            enemyUnit.DamageAnimation(damageDetails.TypeEffectiveness);
            yield return enemyHud.AnimateHp();
            yield return ShowDamageDetails(damageDetails);
            if(damageDetails.Fainted) {
                enemyUnit.FaintAnimation();
                yield return dialogue.TypeDialogue($"{enemyUnit.Mechanimal.Base.Name} broke down :)");
                yield return new WaitForSeconds(0.75f);
                OnBattleOver(true);
            }
            else {
                StartCoroutine(PlayerAction());
            }
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails) {
        if(damageDetails.TypeEffectiveness < 1f) {
            yield return dialogue.TypeDialogue($"It's not very effective...");
        } else if(damageDetails.TypeEffectiveness > 1f) {
            yield return dialogue.TypeDialogue($"It's super effective!");
        }
        yield return new WaitForSeconds(0.75f);
    }

    IEnumerator PerformEnemyMove() {
        state = BattleState.EnemyMove;
        dialogue.EnableActionSelector(false);
        bool canMove = false;
        foreach(var m in enemyUnit.Mechanimal.Moves) {
            if(m.Base.Cost <= enemyUnit.Mechanimal.En) {
                canMove = true;
                break;
            }
        }
        if(canMove) {
            var move = enemyUnit.Mechanimal.GetRandomMove();
            yield return dialogue.TypeDialogue($"{enemyUnit.Mechanimal.Base.Name} used {move.Base.Name}");
            enemyUnit.AttackAnimation();
            yield return new WaitForSeconds(0.5f);
            var damageDetails = playerUnit.Mechanimal.TakeDamage(move, enemyUnit.Mechanimal);
            enemyUnit.Mechanimal.En -= move.Base.Cost;
            enemyHud.UpdateEn();
            playerUnit.DamageAnimation(damageDetails.TypeEffectiveness);
            yield return playerHud.AnimateHp();
            yield return ShowDamageDetails(damageDetails);
            if(damageDetails.Fainted) {
                yield return dialogue.TypeDialogue($"{playerUnit.Mechanimal.Base.Name} broke down :(");
                playerUnit.FaintAnimation();
                yield return new WaitForSeconds(0.75f);

                var next = playerParty.GetHealthyMechanimal();
                if(next != null) {
                    playerUnit.Setup(next);
                    playerHud.SetData(next);
                    playerHud.UpdateEn();
                    playerHud.UpdateHp();
                    dialogue.SetMoveNames(next.Moves);
                    yield return dialogue.TypeDialogue($"GO {next.Base.Name}!");
                    yield return new WaitForSeconds(0.5f);

                    dialogue.UpdateActionSelection(currentAction);
                    dialogue.UpdateMoveSelection(currentMove, next.Moves[currentMove]);
                    StartCoroutine(PlayerAction());
                } else {
                    yield return dialogue.TypeDialogue($"Womp womp...");
                    yield return new WaitForSeconds(0.5f);
                    OnBattleOver(false);
                }
            }
            else {
                yield return new WaitForSeconds(0.5f);
                StartCoroutine(PerformEnemyMove());
            }
        }
        else {
            yield return dialogue.TypeDialogue($"{enemyUnit.Mechanimal.Base.Name} ends its turn!");
            yield return new WaitForSeconds(0.5f);
            playerUnit.Mechanimal.AddEn(1);
            playerHud.UpdateEn();
            StartCoroutine(PlayerAction());
        }
        
    }

    IEnumerator HandleActionSelection() {
        if(Input.GetKeyDown(KeyCode.RightArrow)) {
            if(currentAction < 2)
                currentAction++;
            else // loop back
                currentAction = 0;
            dialogue.UpdateActionSelection(currentAction);
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow)) {
            if(currentAction > 0)
                currentAction--;
            else
                currentAction = 2;
            dialogue.UpdateActionSelection(currentAction);
        }

        if(Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return)) {
            if(currentAction == 0) { // fight
                PlayerMove();
            } else if(currentAction == 1) { // end turn
                enemyUnit.Mechanimal.AddEn(1);
                enemyHud.UpdateEn();
                StartCoroutine(PerformEnemyMove());
            // } else if(currentAction == 2) { // switch
            //     OpenPartyScreen();
            // } else if(currentAction == 3) { // bag

            } else { // run
                if(UnityEngine.Random.Range(1, 3) <= 1) {
                    yield return dialogue.TypeDialogue("You ran away...");
                    yield return new WaitForSeconds(0.5f);
                    OnBattleOver(false);
                }
                else {
                    yield return dialogue.TypeDialogue("You failed to run away...");
                    yield return new WaitForSeconds(0.5f);
                    enemyUnit.Mechanimal.AddEn(1);
                    enemyHud.UpdateEn();
                    StartCoroutine(PerformEnemyMove());
                }   
            }
            
        }
    }
}
public enum BattleState {
    Start,
    PlayerAction,
    PlayerMove,
    EnemyMove,
    Busy
}