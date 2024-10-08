﻿using System;
using System.IO;
using System.Reflection;
using ModGenesia;
using System.Collections.Generic;
using RogueGenesia.Data;
using RogueGenesia.Actors.Survival;
using UnityEngine;
using RogueGenesia.GameManager;
using RogueGenesia.Sound;
using RogueGenesia.UI;

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
    public override void OnRegisterModdedContent()
    {
        AddCardStatExample();

        AddCustomCardExample();

        AddCustomWeaponExample();

        AddExampleOption();

        AddCustomAvatar();

        AddCustomPet();

        RegisterModdedChallenge();
        RegisterCustomModdedChallenge();

    }


    public void AddExampleOption()
    {

        LocalizationData englishLoc = new LocalizationData() { Key = "en", Value = "Example Slider" };
        LocalizationDataList localization = new LocalizationDataList("Example Slider") { localization = new List<LocalizationData>() { englishLoc } };

        LocalizationData englishLocTooltip = new LocalizationData() { Key = "en", Value = "This does nothing" };
        LocalizationDataList localizationTooltip = new LocalizationDataList("Example Slider") { localization = new List<LocalizationData>() { englishLocTooltip } };

        GameOptionData scaleSlider = ModOption.MakeSliderDisplayValueOption("projectile_scale",localization, 0f, 100f, 50f, 100, true, localizationTooltip);
        var sliderobj = ModOption.AddModOption(scaleSlider, "Accessibility Options", "Example Mod");

        
    }

    //This function is called when the game is finishing loading all the mods and the vanilla content
    public override void OnAllContentLoaded()
    {
        Debug.Log("Game Finished Loading Mods Step");
    }


    //Example of a simple Stat Card, this don't require any custom class
    public void AddCardStatExample()
    {


        //We create the SoulCardCreationData, which is used by the game to set the soul-card information
        SoulCardCreationData soulCardCreationData = GetSoulCardCreationData("Card\\ExampleCard.json");

        //We use the Game modding api to add the card data to the game, use an unique card name as duplication can lead to issues
        CardAPI.AddCustomStatCard("Stat_CardExample", soulCardCreationData, true);
    }

    //Example of a custom card having more advanced effect
    public void AddCustomCardExample()
    {


        //card creation of soulcardcreation data is the same as the preview example
        SoulCardCreationData soulCardCreationData = GetSoulCardCreationData("Card\\ExampleCustomCard.json");


        //the main difference is that we send the constructor of the class linked to the card, so the game know which class to instantiate when we obtain the card
        // typeof(YOURCLASS).GetConstructor(Type.EmptyTypes)
        CardAPI.AddCustomCard("Custom_CardExample", typeof(CustomCardExample).GetConstructor(Type.EmptyTypes), soulCardCreationData, true);
    }

    //Example of a custom weapons
    public void AddCustomWeaponExample()
    {
        SoulCardCreationData soulCardCreationData = GetSoulCardCreationData("Card\\ExampleWeapon.json");

        //Similarly to the custom card, we also give the constructor of the weapon's class to be instanted by the game.
        CardAPI.AddCustomWeapon("Weapon_Example", typeof(ExampleWeapon).GetConstructor(Type.EmptyTypes), soulCardCreationData, true);

        //Adding the custom damage source
        ContentAPI.AddCustomDamageSource("ExampleWeaponDamageSource", soulCardCreationData.NameOverride, ModGenesia.ModGenesia.LoadSprite(ModFolder + "/ExampleWeapon.png"));

    }

    void AddCustomAvatar()
    {
        AvatarAnimations avatarAnimations = new AvatarAnimations()
        {
            GameOverAnimation = new PixelAnimationData() { Frames = new Vector2Int(1, 1), Texture = ModGenesia.ModGenesia.LoadPNGTexture(ModFolder + "/SerialKillerAvatar/SKA_Death.png") },
            Icon = ModGenesia.ModGenesia.LoadSprite(ModFolder + "/SerialKillerAvatar/SKA_Icon.png"),
            IdleAnimation = new PixelAnimationData() { Frames = new Vector2Int(9, 1), Texture = ModGenesia.ModGenesia.LoadPNGTexture(ModFolder + "/SerialKillerAvatar/SKA_Idle64.png") },
            IdleHDAnimation = new PixelAnimationData() { Frames = new Vector2Int(10, 1), Texture = ModGenesia.ModGenesia.LoadPNGTexture(ModFolder + "/SerialKillerAvatar/SKA_Idle128.png") },
            RunAnimation = new PixelAnimationData() { Frames = new Vector2Int(10, 1), Texture = ModGenesia.ModGenesia.LoadPNGTexture(ModFolder + "/SerialKillerAvatar/SKA_Run.png") },
            VictoryAnimation = new PixelAnimationData() { Frames = new Vector2Int(1, 1), Texture = ModGenesia.ModGenesia.LoadPNGTexture(ModFolder + "/SerialKillerAvatar/SKA_Victory.png") }
        };


        

        LocalizationDataList name = new LocalizationDataList();

        name.localization.Add(new LocalizationData() { Key = "en", Value = "Serial Killer" });



        LocalizationDataList description = new LocalizationDataList();
        description.localization.Add(new LocalizationData() { Key = "en", Value = "The Serial killer count his victim, for each kill you receive one charge."
            
            +"\nYou can press the attack button to use your charges, 10 charges are used, you trigger one attack for each of your weapon" });


        AvatarSkinSO defaultAvatarSkin = AvatarAPI.AddAvatarSkin("Default_SerialKiller", avatarAnimations, "SerialKiller", name, description);

        AvatarAPI.AddCustomAvatar("SerialKiller", typeof(ExampleAvatarData).GetConstructor(Type.EmptyTypes), defaultAvatarSkin, name, description, new Color(1, 0, 0), true);
    }



    void AddCustomPet()
    {
        PetAnimations petAnimation = new PetAnimations()
        {
            Icon = ModGenesia.ModGenesia.LoadSprite(ModFolder + "/MiniRog/MiniRogIcon.png"),
            IdleAnimation = new PixelAnimationData() { Frames = new Vector2Int(9, 1), Texture = Resources.Load<Texture2D>("Textures/Player/RogKnight/Default/Rog_Idle64") },
            RunAnimation = new PixelAnimationData() { Frames = new Vector2Int(10, 1), Texture = Resources.Load<Texture2D>("Textures/Player/RogKnight/Default/Rog_Movement64") },
        };

        LocalizationDataList name = new LocalizationDataList();

        name.localization.Add(new LocalizationData() { Key = "en", Value = "MiniRog" });



        LocalizationDataList description = new LocalizationDataList();
        description.localization.Add(new LocalizationData()
        {
            Key = "en",
            Value = "Touch talk to me or any of my mini-rog again"
        });

        PetScriptableObject pet = PetAPI.AddCustomPet("MiniRog", typeof(ExamplePetData), petAnimation, ERequiredPetDLC.Dog);
        pet.Unlocked = true;
        pet.PetScale = new Vector2(1, 1);
    }


    void RegisterModdedChallenge()
    {

        ChallengeModifier challengeModifier = new ChallengeModifier();
        //We reduce health regen by 95% in this challenge
        challengeModifier.StatsModifier = new StatsModifier();
        challengeModifier.StatsModifier.ModifiersList.Add(
            new StatModifier
            {
                Key = "HealthRegen",
                Value = new SingularModifier() { Value = 0.05f }
            });


        var localizedName = new List<LocalizationData>
            {
                new LocalizationData { Key = "en", Value = "Example Modded Challenge" }
            };

        //We use the "AddCustomChallengeWithoutRefresh()" instead of "AddCustomChallenge()" function as it greatly reduce the CPU usage.
        var moddedChallenge = ChallengeAPI.AddCustomChallengeWithoutRefresh("ExampleModdedChallenge", 0, (int)EDifficulty.D, 2, challengeModifier, false, localizedName);

        moddedChallenge.AllowStacking = true;
        moddedChallenge.CanLevelUp = true;
        moddedChallenge.HardMode = false;
        moddedChallenge.LevelIncreaseType = ChallengeSO.ChallengeLevelingType.StatAndRankIncrease;

    }

    void RegisterCustomModdedChallenge()
    {
        // we first register our custom class ExampleModdedChallengeModifier that is created inside ExampleModdedChallengeModifier.cs
        ChallengeAPI.RegisterCustomChallengeModifier("ExampleModdedCustomChallenge", typeof(ExampleModdedChallengeModifier).GetConstructor(Type.EmptyTypes));

        ChallengeModifier challengeModifier = new ChallengeModifier();

        challengeModifier.CustomChallengeModifiersList.Add("ExampleModdedCustomChallenge");

        challengeModifier.GlobalStatsMultiplier = 0.75f;

        var localizedName = new List<LocalizationData>
            {
                new LocalizationData { Key = "en", Value = "Custom Modded Challenge" }
            };

        // we use the "AddCustomChallengeWithoutRefresh()" instead of "AddCustomChallenge()" function as it greatly reduce the CPU usage 
        var CustomModdedChallenge = ChallengeAPI.AddCustomChallengeWithoutRefresh("ExampleModdedCustomChallenge", 0, (int)EDifficulty.D, 2, challengeModifier, false, localizedName);

        CustomModdedChallenge.AllowStacking = true;
        CustomModdedChallenge.CanLevelUp = true;
        CustomModdedChallenge.HardMode = false;
        CustomModdedChallenge.LevelIncreaseType = ChallengeSO.ChallengeLevelingType.StatAndRankIncrease;
    }

}
