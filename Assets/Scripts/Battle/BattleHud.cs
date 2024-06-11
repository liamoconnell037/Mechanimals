using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class BattleHud : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI levelText;
    [SerializeField] HPBar hpBar;
    [SerializeField] HPBar enBar;
    Mechanimal _ma;
    public void SetData(Mechanimal ma) {
        _ma = ma;
        nameText.text = ma.Base.Name;
        levelText.text = "Lvl " + ma.Level;
        hpBar.SetHp((float)ma.Hp / ma.MaxHp);
    }

    public IEnumerator AnimateHp() {
        yield return StartCoroutine(hpBar.SetSmooth((float)_ma.Hp / _ma.MaxHp));
    }
    public void UpdateEn() {
        enBar.SetHp((float)_ma.En / _ma.MaxEn);
    }
    public void UpdateHp() {
        hpBar.SetHp((float)_ma.Hp / _ma.MaxHp);
    }
}
