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

public class ExampleAvatarData : AvatarData
{

    public override void BaseStatsInit()
    {
        _basePlayerStats.MaxHealth.SetDefaultBaseStat(10);
    }


    public override void OnAttackButtonPressed(PlayerEntity playerEntity)
    {
        float SerialKillerValue = GameData.GetCustomValue("SerialKillerKill");
        if (SerialKillerValue >= 10)
        {
            int attackProcc = Mathf.FloorToInt(SerialKillerValue/10);

            SerialKillerValue -= (attackProcc*10);

            for (int A = 0; A < attackProcc; A++)
            {
                for (int i = 0; i < _weapon.Count; i++)
                {
                    OnAttack(playerEntity, new AttackInformation(_weapon[i].GetComboCount - _weapon[i].AttackComboLeft, _weapon[i].GetComboCount), i);
                }
            }
            GameData.SetCustomValue("SerialKillerKill", SerialKillerValue);
            PlayerEntity.OverrideBuff(new SerialKillerBuffCounter(PlayerEntity, SerialKillerValue));
        }

    }


    public override void OnKill(IEntity entityKilled)
    {
        float SerialKillerValue = GameData.GetCustomValue("SerialKillerKill");

        if (SerialKillerValue < 200)
        {
            SerialKillerValue ++;

            PlayerEntity.OverrideBuff(new SerialKillerBuffCounter(PlayerEntity, SerialKillerValue));

            GameData.SetCustomValue("SerialKillerKill", SerialKillerValue);
        }

    }

    int SerialKillerBuffID = -1;

    public override void OnUpdate(PlayerEntity playerEntity)
    {


        base.OnUpdate(playerEntity);

        float SerialKillerValue = GameData.GetCustomValue("SerialKillerKill");
        if (SerialKillerValue > 0)
        {
            PlayerEntity.OverrideBuff(new SerialKillerBuffCounter(PlayerEntity, SerialKillerValue));
        }
        else
        {
            if (SerialKillerBuffID == -1)
            {
                SerialKillerBuffID = BuffIDManager.GetID("SerialKillerBuffCounter");
            }
            PlayerEntity.RemoveBuff(SerialKillerBuffID);
        }
        
    }

}

//used to display to the player the number of SerialKillerBuff
public class SerialKillerBuffCounter : Buff
{

    public override void LoadBuffIcon()
    {
        if (_buffIcon == null)
        {
            _buffIcon = ModGenesia.ModGenesia.LoadPNGTexture(ExampleMod.ModFolder + "/SerialKillerAvatar/ChargeIcon.png");
        }
    }

    public override string GetDescription()
    {
        return "Charges of Serial Killer ability you own \nBy using the attack button ,you can consume 10 charges to make all your weapon attack once";
    }

    public override string GetName()
    {
        return "Serial Killer charges";
    }

    public SerialKillerBuffCounter(IEntity owner, float level) : base(BuffIDManager.GetID("SerialKillerBuffCounter"), owner, owner, 1, level, BuffStacking.NoDuration)
    {

    }
}
