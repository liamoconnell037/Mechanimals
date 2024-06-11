using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Mechanimal> wildMechanimals;

    public Mechanimal GetWildMechanimal() {
        Mechanimal ma = wildMechanimals[UnityEngine.Random.Range(0, wildMechanimals.Count)];
        ma.init();
        return ma;
    }
    
}
