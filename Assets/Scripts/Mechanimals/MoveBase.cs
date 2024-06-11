using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Mechanimals/Create move")]
public class MoveBase : ScriptableObject
{
   [SerializeField] string _name;
   
   [TextArea]
   [SerializeField] string description;

   [SerializeField] MechanimalType type;
   [SerializeField] int power;
   [SerializeField] int accuracy;
   [SerializeField] int cost;
   public string Name {
        get {return _name;}
    }
    public string Description {
        get {return description;}
    }
    public MechanimalType Type {
        get {return type;}
    }
    public int Power {
        get {return power;}
    }
    public int Accuracy {
        get {return accuracy;}
    }
    public int Cost {
        get {return cost;}
    }
}
