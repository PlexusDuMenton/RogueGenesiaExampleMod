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

public class CustomCardExample : SoulCard
{

    public override void OnTakeDamagePreDefence(PlayerEntity owner, IEntity damageOwner, ref float modifierDamageValue, float damageValue)
    {
        owner.AddBuff(new ExampleCustomCardEffectBuff(BuffIDManager.GetID("ExampleCustomBuff"), owner, owner, BuffStacking.IncreaseLevelByLevel | BuffStacking.IndepentStackDuration, 0.85f, 4+_level, 20f, 1));
    }




}

