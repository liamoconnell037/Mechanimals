using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class MechanimalParty : MonoBehaviour
{
    [SerializeField] List<Mechanimal> mechanimals;
    private void Start() {
        foreach(var ma in mechanimals) {
            ma.init();
        }
    }

    public Mechanimal GetHealthyMechanimal() { //gets first non-fainted mechanimal
        return mechanimals.Where(x => x.Hp > 0).FirstOrDefault(); // this is nasty
    }
}
