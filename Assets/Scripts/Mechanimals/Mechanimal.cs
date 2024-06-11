using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Mechanimal
{
    [SerializeField] MechanimalsBase _base;
    [SerializeField] int level;
    public MechanimalsBase Base {
        get {
            return _base;
        }
    }
    public int Level {
        get {
            return level;
        }
    }
    public int Hp {get; set;}
    public int En {get; set;}
    public List<Move> Moves {get; set;}

    public void init() {
        Hp = MaxHp;
        En = MaxEn;
        Moves = new List<Move>();
        foreach(var move in Base.LearnableMoves) {
            if(level > move.Level)
                Moves.Add(new Move(move.MoveBase));

            if(Moves.Count >= 4)
                break;
        }
    }

    //formulas used by pokemon
    public int Attack {
        get {return Mathf.FloorToInt(Base.Attack * Level / 100f) + 5;} 
    }
    public int MaxHp {
        get {return Mathf.FloorToInt(Base.MaxHp * Level / 100f) + 10;} 
    }
    public int MaxEn {
        get {return Base.MaxEn;}
    }
    public int Defense {
        get {return Mathf.FloorToInt(Base.Defense * Level / 100f) + 5;} 
    }
    public int Speed {
        get {return Mathf.FloorToInt(Base.Speed * Level / 100f) + 5;} 
    }

    public DamageDetails TakeDamage(Move move, Mechanimal attacker) {
        // more pokemon formulas
        float type = TypeChart.GetEffectiveness(move.Base.Type, this.Base.Type);

        var damageDetails = new DamageDetails() {
            TypeEffectiveness = type,
            Fainted = false,
        };

        float modifiers = UnityEngine.Random.Range(0.85f, 1f) * type;
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attacker.Attack / Defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);
        
        Hp -= damage;
        if(Hp <= 0) {
            Hp = 0;
            damageDetails.Fainted = true;
        }
        return damageDetails;
    } 

    public void AddEn(int en) {
        if(En + en >= MaxEn)
            En = MaxEn;
        else
            En += en;
    }
    public Move GetRandomMove() {
        var move = Moves[UnityEngine.Random.Range(0, Moves.Count)];
        if(En >= move.Base.Cost) {
            return move;
        }
        return GetRandomMove();
    }
}


public class DamageDetails {
    public bool Fainted {get; set;}

    public float TypeEffectiveness {get; set;}
}