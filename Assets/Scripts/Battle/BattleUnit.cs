using UnityEngine;
using DG.Tweening;
public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isPlayer;

    public Mechanimal Mechanimal {get; set;}

    UnityEngine.UI.Image image;
    Vector3 originalPos;
    Color originalColor;
    private void Awake() {
        image = GetComponent<UnityEngine.UI.Image>();
        originalPos = image.transform.localPosition;
        originalColor = image.color;
    }
    public void Setup(Mechanimal ma) {
        Mechanimal = ma;
        if(isPlayer) 
            image.sprite = Mechanimal.Base.BackSprite;
        else
            image.sprite = Mechanimal.Base.FrontSprite;
        image.color = originalColor;
        EnterAnimation();
    }

    public void EnterAnimation() {
        if(isPlayer) {
            image.transform.localPosition = new Vector3(originalPos.x - 350f, originalPos.y);
        } else {
            image.transform.localPosition = new Vector3(originalPos.x + 350f, originalPos.y);
        }

        image.transform.DOLocalMoveX(originalPos.x, 1f, false); // sliiiiick animation
    }

    public void AttackAnimation() {
        var sequence = DOTween.Sequence();
        if(isPlayer) {
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 25f, 0.25f));
        }
        else
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 25f, 0.25f));
        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.25f));
    } 
    public void DamageAnimation(float typeEffectiveness) {
        var sequence = DOTween.Sequence();
        if(typeEffectiveness < 1f) {
            sequence.Append(image.DOColor(Color.blue, 0.1f));
            sequence.Append(image.DOColor(originalColor, 0.1f));
        } else if (typeEffectiveness > 1f) {
            sequence.Append(image.DOColor(Color.red, 0.1f));
            if(isPlayer)
                sequence.Join(image.transform.DOLocalMoveX(originalPos.x -25f, 0.15f));
            else
                sequence.Join(image.transform.DOLocalMoveX(originalPos.x + 25f, 0.15f));
            sequence.Append(image.DOColor(originalColor, 0.1f));
            sequence.Join(image.transform.DOLocalMoveX(originalPos.x, 0.15f));
        } else {
            sequence.Append(image.DOColor(Color.gray, 0.15f));
            sequence.Append(image.DOColor(originalColor, 0.15f));
        }
    }

    public void FaintAnimation() {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }
}
