using RogueGenesia.Actors.Survival;
using RogueGenesia.Data;
using RogueGenesia.GameManager;
using RogueGenesia.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ExampleModdedChallengeDescription : CustomChallengeDescription
{
    public override string GetLine()
    {

        var link = GameData.SoulCardDictionary["Pyromaniac"].GetLinkName()
                   + ", "
                   + GameData.SoulCardDictionary["Revolution"].GetLinkName()
                   + ", "
                   + GameData.SoulCardDictionary["LoveAndHate"].GetLinkName()
                   + ", "
                   + GameData.SoulCardDictionary["BloodSpirit"].GetLinkName()
            ;

        List<LocalizationValueContainer> KeyList = new List<LocalizationValueContainer>()
            {
                new LocalizationValueContainer("other", link)
            };

        return LH.GetCompleteLocalized(LH.GetToolTip, "ArmorBan_CardBan", KeyList) + "\nFood slowly heal you over 10 sec";

    }
}

public class ExampleModdedChallengeModifier : CustomChallengeModifier
{
    public override CustomChallengeDescription GetModifierDescription()
    {
        return new ExampleModdedChallengeDescription();

    }

    public override void OnEndRun()
    {

    }
    public override void OnQuitRun()
    {

    }

    public override void OnLoadRun()
    {
        OnStartOrLoadRun();
    }


    public override void OnStartRun()
    {
        GameData.BanishCard(GameData.SoulCardDictionary["Pyromaniac"]);
        GameData.BanishCard(GameData.SoulCardDictionary["Revolution"]);
        GameData.BanishCard(GameData.SoulCardDictionary["LoveAndHate"]);
        GameData.BanishCard(GameData.SoulCardDictionary["BloodSpirit"]);

        OnStartOrLoadRun();
    }

    void OnStartOrLoadRun()
    {
        GameEventManager.OnEatFood.AddListener(OnEatFood, 999);
    }

    private void OnEatFood(PlayerEntity playerEntity, FloatValue HealValue)
    {

        playerEntity.AddBuff(
            new LustBuff(
                BuffIDManager.GetID("Lust"),
                playerEntity,
                playerEntity,
                BuffStacking.IncreaseLevelByLevel | BuffStacking.IndepentStackDuration,
                10,
                HealValue.Value * 0.1f
            )
        );

        HealValue.Value = 0;


    }
}

public class FoodHealOverTimeBuff : Buff
{

    public FoodHealOverTimeBuff(
        int ID,
        IEntity owner,
        IEntity origin,
        BuffStacking buffStacking,
        float duration = 2,
        float level = 1
    )
        : base(ID, owner, origin, duration, level, buffStacking)
    {

    }

    public override string GetName()
    {
        return "HealOverTime";
    }

    public override string GetDescription()
    {
        return "Slowly heal you overtime from food eaten";
    }

    public override void LoadBuffIcon()
    {
        if (_buffIcon == null)
        {
            _buffIcon = Resources.Load<Texture2D>("UI/Buff/Lust");
        }
    }

    public override void UpdateStats(float newStackLevel)
    {

        if ((PlayerEntity)_entity)
        {
            ((PlayerEntity)_entity).PlayerStats.HealthRegen.SetDynamicPostValue(
                "FoodHealOverTimeBuff",
                newStackLevel
            );
        }
    }
}
