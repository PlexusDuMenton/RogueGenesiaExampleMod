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

public class ExampleMod : RogueGenesiaMod
{
    //so we can track the mod folder to load texture from
    static public string ModFolder;

    //This happen before the game load any contents
    public override void OnModLoaded(ModData modData)
    {

        ModFolder = modData.ModDirectory.FullName;

        Debug.Log("Example Mod Loaded");
    }


    //We add content at this stage, as it happen after game registered the vanilla content, but before it initiated all the IDs
    //Adding stuff before can work but you won't be able to add requirement from vanilal content
    //Adding stuff after won't work because their ID will be conflicting with other vanilla content
    public override void GameRegisterationStep()
    {
        AddCardStatExample();

        AddCustomCardExample();

        AddCustomWeaponExample();

        Debug.Log("Example Card added");
    }

    //This function is called when the game is finishing loading all the mods and the vanilla content
    public override void OnGameFinishedLoading()
    {
        Debug.Log("Game Finished Loading Mods Step");
    }


    //Example of a simple Stat Card, this don't require any custom class
    public void AddCardStatExample()
    {


        //We create the SoulCardCreationData, which is used by the game to set the soul-card information
        SoulCardCreationData soulCardCreationData = new SoulCardCreationData();

        //Tags of the card, a card can have multiple tags, it can be used in multiple way in the game
        soulCardCreationData.Tags = CardTag.Fire;

        
        //Set the max level of the card (note that level of the card can go up to max-level+2
        soulCardCreationData.MaxLevel = 2;
        //Sprite of the card, we are loading the sprite from the modFolder
        soulCardCreationData.Texture = ModGenesia.ModGenesia.LoadSprite(ModFolder + "/ExampleCard.png");

        //He we set the different names in differents language, in this example, only the english language is set
        //You can get all language supported by the game using ModGenesia.ModGenesia.GetLocales():
        soulCardCreationData.NameOverride = new List<LocalizationData>();
        soulCardCreationData.NameOverride.Add(new LocalizationData());
        soulCardCreationData.NameOverride[0].Key = "en";
        soulCardCreationData.NameOverride[0].Value = "Stat CardExample";

        //Custom description ins multiple language
        //Dynamic description is not yet supported for mods
        soulCardCreationData.DescriptionOverride = new List<LocalizationData>();
        soulCardCreationData.DescriptionOverride.Add(new LocalizationData());
        soulCardCreationData.DescriptionOverride[0].Key = "en";
        soulCardCreationData.DescriptionOverride[0].Value = "Stat CardExample Description";

        //Here we set the stats modifier of the card
        soulCardCreationData.ModifyPlayerStat = true;
        soulCardCreationData.StatsModifier = new StatsModifier();

        //we add a new stats modifier, modifying Damage stats, it is additional type, so it add to the base value
        soulCardCreationData.StatsModifier.ModifiersList.Add(new StatModifier());
        soulCardCreationData.StatsModifier.ModifiersList[0].Key = nameof(StatsType.Damage);
        soulCardCreationData.StatsModifier.ModifiersList[0].Value = new SingularModifier(0);
        soulCardCreationData.StatsModifier.ModifiersList[0].Value.ModifierType = ModifierType.Additional;
        soulCardCreationData.StatsModifier.ModifiersList[0].Value.Value = 10;

        soulCardCreationData.StatsModifier.ModifiersList.Add(new StatModifier());
        soulCardCreationData.StatsModifier.ModifiersList[1].Key = nameof(StatsType.MaxHealth);
        soulCardCreationData.StatsModifier.ModifiersList[1].Value = new SingularModifier(0);
        soulCardCreationData.StatsModifier.ModifiersList[1].Value.ModifierType = ModifierType.Compound;
        soulCardCreationData.StatsModifier.ModifiersList[1].Value.Value = 0.25f;

        soulCardCreationData.StatsModifier.ModifiersList.Add(new StatModifier());
        soulCardCreationData.StatsModifier.ModifiersList[2].Key = nameof(StatsType.AttackCoolDown);
        soulCardCreationData.StatsModifier.ModifiersList[2].Value = new SingularModifier(0);
        soulCardCreationData.StatsModifier.ModifiersList[2].Value.ModifierType = ModifierType.Compound;
        soulCardCreationData.StatsModifier.ModifiersList[2].Value.Value = .75f;

        soulCardCreationData.StatsModifier.ModifiersList.Add(new StatModifier());
        soulCardCreationData.StatsModifier.ModifiersList[3].Key = nameof(StatsType.AttackDelay);
        soulCardCreationData.StatsModifier.ModifiersList[3].Value = new SingularModifier(0);
        soulCardCreationData.StatsModifier.ModifiersList[3].Value.ModifierType = ModifierType.Compound;
        soulCardCreationData.StatsModifier.ModifiersList[3].Value.Value = .75f;

        //we set the weight from droping (when the card is level 0) and then the weight once we already own it
        soulCardCreationData.DropWeight = 0.01f;
        soulCardCreationData.LevelUpWeight = 0.05f;

        //we set the card rarity, note that PLACEHOLDER_X, are just slot in case future rarity are added into the game and are not supported for now
        soulCardCreationData.Rarity = CardRarity.Ascended;
        //set from which mod the card is from, if it's not set, it'll simply be written "modded"
        soulCardCreationData.ModSource = "CardExampleMod";

        //We use the Game modding api to add the card data to the game, use an unique card name as duplication can lead to issues
        CardAPI.AddCustomStatCard("Stat_CardExample", soulCardCreationData, true);
    }

    //Example of a custom card having more advanced effect
    public void AddCustomCardExample()
    {


        //card creation of soulcardcreation data is the same as the preview example
        SoulCardCreationData soulCardCreationData = new SoulCardCreationData();


        soulCardCreationData.Tags = CardTag.Fire | CardTag.Moon;

        soulCardCreationData.MaxLevel = 1;
        soulCardCreationData.Texture = ModGenesia.ModGenesia.LoadSprite(ModFolder + "/ExampleCustomCard.png");

        soulCardCreationData.NameOverride = new List<LocalizationData>();
        soulCardCreationData.NameOverride.Add(new LocalizationData());
        soulCardCreationData.NameOverride[0].Key = "en";
        soulCardCreationData.NameOverride[0].Value = "Custom Card example";

        soulCardCreationData.DescriptionOverride = new List<LocalizationData>();
        soulCardCreationData.DescriptionOverride.Add(new LocalizationData());
        soulCardCreationData.DescriptionOverride[0].Key = "en";
        soulCardCreationData.DescriptionOverride[0].Value = "When you take damage, it Increase your attack speed by 15% up to 5 stack (+1 per level)";

        soulCardCreationData.DropWeight = 0.1f;
        soulCardCreationData.LevelUpWeight = 0.25f;


        soulCardCreationData.Rarity = CardRarity.Heroic;
        soulCardCreationData.ModSource = "CardExampleMod";

        //the main difference is that we send the constructor of the class linked to the card, so the game know which class to instantiate when we obtain the card
        // typeof(YOURCLASS).GetConstructor(Type.EmptyTypes)
        CardAPI.AddCustomCard("Custom_CardExample", typeof(CustomCardExample).GetConstructor(Type.EmptyTypes), soulCardCreationData, true);
    }

    //Example of a custom weapons
    public void AddCustomWeaponExample()
    {
        SoulCardCreationData soulCardCreationData = new SoulCardCreationData();

        //Weapon don't use tags, but use weapon tags, they are used as card requirements for some cards
        soulCardCreationData.WeaponTags = WeaponTag.Distance | WeaponTag.Physical | WeaponTag.Projectile | WeaponTag.Critical;

        soulCardCreationData.MaxLevel = 7;
        soulCardCreationData.Texture = ModGenesia.ModGenesia.LoadSprite(ModFolder + "/ExampleWeapon.png");

        soulCardCreationData.NameOverride = new List<LocalizationData>();
        soulCardCreationData.NameOverride.Add(new LocalizationData());
        soulCardCreationData.NameOverride[0].Key = "en";
        soulCardCreationData.NameOverride[0].Value = "Custom Weapon Example";

        soulCardCreationData.DescriptionOverride = new List<LocalizationData>();
        soulCardCreationData.DescriptionOverride.Add(new LocalizationData());
        soulCardCreationData.DescriptionOverride[0].Key = "en";
        soulCardCreationData.DescriptionOverride[0].Value = "Shot bolt to the strongest ennemy near you, enemies hit, get teleported in a random position and lose a part of their max health";

        soulCardCreationData.Rarity = CardRarity.Evolution;
        soulCardCreationData.ModSource = "CardExampleMod";

        soulCardCreationData.DropWeight = 0.1f;
        soulCardCreationData.LevelUpWeight = 0.15f;

        soulCardCreationData.cardAvatarLimitation = CardAvatarLimitation.BlackList;
        soulCardCreationData.avatarLimitationList = new List<string>(){ "GunSlinger" };


        //Modded Card also supported requirements, 
        //we tell the game we need card of name cross bow at level 7 minimum and Card of name Rift at level 0 minimum to unloack this weapon
        ModCardRequirement cardForEvolutionWeapon = new ModCardRequirement();
        cardForEvolutionWeapon.cardName = "CrossBow";
        cardForEvolutionWeapon.requiredLevel = 7;

        ModCardRequirement cardForEvolutionPassive = new ModCardRequirement();
        cardForEvolutionPassive.cardName = "Rift";
        cardForEvolutionPassive.requiredLevel = 0;

        //Hard requirements meant all the requirements must be fullfilled, where normal requirement simply required one of the requirements de be fullfiled

        soulCardCreationData.CardHardRequirement = CardAPI.MakeCardRequirement(new ModCardRequirement[2] { cardForEvolutionWeapon, cardForEvolutionPassive }, new StatRequirement[0]);

        //Similarly to the custom card, we also give the constructor of the weapon's class to be instanted by the game.
        CardAPI.AddCustomWeapon("Weapon_Example", typeof(ExampleWeapon).GetConstructor(Type.EmptyTypes), soulCardCreationData, true);
    }


}
