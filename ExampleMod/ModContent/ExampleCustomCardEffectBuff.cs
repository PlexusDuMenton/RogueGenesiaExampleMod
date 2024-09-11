using System;
using System.IO;
using System.Reflection;
using ModGenesia;
using System.Collections.Generic;
using RogueGenesia.Data;
using RogueGenesia.Actors.Survival;
using UnityEngine;
using RogueGenesia.GameManager;
using RogueGenesia.Sound;


public class ExampleCustomCardEffectBuff : Buff
{
    protected float SpeedBuff = .85f;
    protected float AttackSpeedStackCap = 5;
    public ExampleCustomCardEffectBuff(int ID, IEntity owner, IEntity origin, BuffStacking buffStacking, float speedBuff, float attackSpeedStackCap, float duration = 2, float level = 1)
        : base(ID, owner, origin, duration, level, buffStacking)
    {
        SpeedBuff = speedBuff;
        AttackSpeedStackCap = attackSpeedStackCap;
    }

    public override void LoadBuffIcon()
    {
        if (_buffIcon == null)
        {
            _buffIcon = ModGenesia.ModGenesia.LoadPNGTexture(ExampleMod.ModFolder + "/ExampleCustomCard.png");
        }
    }

    public override void UpdateStats(float newStackLevel)
    {
        if ((PlayerEntity)_entity)
        {
            ((PlayerEntity)_entity).PlayerStats.AttackCoolDown.SetDynamicMultiplyValue("ExampleCustomBuff", Mathf.Pow(SpeedBuff, Mathf.Min(AttackSpeedStackCap, _buffLevel)));
        }
    }

    public override void AddBuff(Buff buff)
    {
        SpeedBuff = ((ExampleCustomCardEffectBuff)buff).SpeedBuff;
        AttackSpeedStackCap = ((ExampleCustomCardEffectBuff)buff).AttackSpeedStackCap;

        base.AddBuff(buff);
    }
}
