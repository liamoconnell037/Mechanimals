using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class BattleDialogueBox : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] int lettersPerSecond;
    [SerializeField] Color highlightedColor;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;
    [SerializeField] List<TextMeshProUGUI> actionTexts;
    [SerializeField] List<TextMeshProUGUI> moveTexts;
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] TextMeshProUGUI typeText;

    public void SetDialogue(string text) {
        dialogueText.text = text;
    }

    public IEnumerator TypeDialogue(string text) {
        dialogueText.text = "";
        foreach(var letter in text) {
            dialogueText.text += letter;
            yield return new WaitForSeconds(1f/lettersPerSecond);
        }
    }

    public void EnableDialogueText(bool enabled) {
        dialogueText.enabled = enabled;
    }
    public void EnableActionSelector(bool enabled) {
        actionSelector.SetActive(enabled);
    }
    public void EnableMoveSelector(bool enabled) {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

    public void UpdateActionSelection(int currentAction) {
        for(int i = 0; i < actionTexts.Count;i++) {
            if(i == currentAction)
                actionTexts[i].color = highlightedColor;
            else
                actionTexts[i].color = Color.white;
        }
    }

    public void resetActionSelection() {
        for(int i = 0; i < actionTexts.Count;i++) {
            actionTexts[i].color = Color.white;
        }
    }
    public void resetMoveSelection() {
        for(int i = 0; i < moveTexts.Count;i++) {
            moveTexts[i].color = Color.white;
        }
    }
    public void UpdateMoveSelection(int currentMove, Move move) {
        for(int i = 0; i < moveTexts.Count;i++) {
            if(i == currentMove)
                moveTexts[i].color = highlightedColor;
            else
                moveTexts[i].color = Color.white;
        }

        costText.text = $"Cost: {move.Base.Cost}";
        typeText.text = $"TYPE {move.Base.Type.ToString()}";
    }
    public void SetMoveNames(List<Move> moves) {
        for(int i = 0; i < moveTexts.Count;i++) {
            if(i < moves.Count) {
                moveTexts[i].text = moves[i].Base.Name;
            } else
                moveTexts[i].text = "-";
        }
    }
}
