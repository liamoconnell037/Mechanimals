using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Mechanimals", menuName = "Mechanimals/Create new mechaninal")]
public class MechanimalsBase : ScriptableObject
{
    [SerializeField] string _name;

    [TextArea] 
    [SerializeField] string description;
    
    [SerializeField] Sprite frontSprite;
    [SerializeField] Sprite backSprite;

    [SerializeField] MechanimalType type;

    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int speed;
    [SerializeField] List<LearnableMove> learnableMoves;
    [SerializeField] int maxEn;
    // KILL ME NOWWWW
    public string Name {
        get {return _name;}
    }

    public int MaxEn {get { return maxEn; }}
    public string Description {
        get {return description;}
    }
    public Sprite FrontSprite {
        get {return frontSprite;}
    }
    public Sprite BackSprite {
        get {return backSprite;}
    }
    public MechanimalType Type {
        get {return type;}
    }
    public int MaxHp {
        get {return maxHp;}
    }
    public int Attack {
        get {return attack;}
    }
    public int Defense {
        get {return defense;}
    }
    public int Speed {
        get {return speed;}
    }

    public List<LearnableMove> LearnableMoves {
        get {return learnableMoves; }
    }
}

[System.Serializable]
public class LearnableMove {
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;
    
    public MoveBase MoveBase {
        get {return moveBase; }
    }
    public int Level { get { return level; }}
}


public enum MechanimalType {
    Malware,
    Software,
    Hardware,
    Firmware //basically normal type
}

public class TypeChart {
    static float[][] chart = {
        //                          MAL  SOFT  HARD  FIRM
        /* malware */ new float[] {0.5f, 2f,   0.5f, 1f},
        /* software */new float[] {0.5f, 0.5f, 2f,   1f},
        /* hardware */new float[] {2f,   0.5f, 0.5f, 1f},
        /* firmware */new float[] {1f,   1f,   1f,   1f},
    };

    public static float GetEffectiveness(MechanimalType attackType, MechanimalType defenseType) {
        return chart[(int)attackType][(int)defenseType];
    }
}