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


public class ExampleWeapon : Weapon
{
    public ExampleWeapon()
    {
        ShotSound = Resources.Load<AudioClip>("SFX/Gameplay/Throw_Sound");
        ComboCount = 1;
        Projectile = 1;
        DelayBetweenCombo = 1.0f;
        DelayBetweenAttack = 0.1f;
        Damage = 20;
        Piercing = 6;
        DamageSource = "ExampleWeaponDamageSource";
        _weaponProjectile = Resources.Load<GameObject>("WeaponProjectiles/BoltProjectile");
    }
    public override void OnAttack(PlayerEntity Owner, AttackInformation attackInformation)
    {
        if (EnemyManager.GetEnemiesCount > 0)
        {

            List<DefaultProjectilAI> projectileList = GetHealthyTargeting(GameData.GetConstructor(typeof(DefaultProjectilAI)), Owner);


            if (projectileList.Count > 0)
            {
                SoundEffect SFX = new SoundEffect(ShotSound);
                SFX.volume = 0.25f;
                SFX.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                SoundManager.PlaySoundEffect(SFX);
            }
            for (int i = 0; i < projectileList.Count; i++)
            {
                GameObject projectileGO = projectileList[i].gameObject;
                projectileGO.transform.localScale = Vector3.one * Owner.GetPlayerStats.ProjectileSize.GetValue();
                DefaultProjectilAI projectile = projectileList[i];
                projectile.Owner = Owner;
                projectile.damageSource = DamageSource;
                projectile.DefencePiercing = Owner.GetPlayerStats.DefencePiercing.Value;
                DamageMultiplierData DamageMultiplier = Owner.GetDamageMultiplier;
                projectile.DamageValue = Damage * DamageMultiplier.DamageMultiplier;
                projectile.Critical = DamageMultiplier.Critical;
                projectile.Piercing = (int)(Piercing + Owner.GetPlayerStats.ProjectilePiercing.GetValue()) + 1;
                projectile.Speed = 25 * Owner.GetPlayerStats.ProjectileSpeed.GetValue() * (0.75f + UnityEngine.Random.value * 0.25f); ;
                projectile.LifeTime = Owner.GetPlayerStats.ProjectileLifeTime.GetValue();
                projectile.knockback = Owner.GetPlayerStats.KnockBack.GetValue() * 0.75f;
                projectile.modifierLevel = Owner.ClonePlayerModifier;
                projectile.Awake();
            }
        }

    }

    public void OnEnemyHit(IEntity entityHitted, DefaultProjectilAI projectile, DamageInformation damageInformation)
    {
        Monster monster = entityHitted as Monster;

        GameObject enemy = entityHitted.GetLinkedGameObject();
        Vector3 enemyPos = enemy.transform.position;
        Vector3 direction = enemyPos - projectile.Owner.GetLinkedGameObject().transform.position;

        float BonusDamage = 0;
        //we don't want cactuses to be teleported
        if (monster != null)
        {
            if (monster.CurrentHealth <= 0)
                return;
            BonusDamage = monster.CurrentHealth * (0.01f + 0.001f * _level);
        }

        bool killed = entityHitted.TakeDamage(new DamageInformation(projectile.Owner, BonusDamage, 0, false, 0, new Vector2(direction.x, direction.z), DamageSource));


        //reduce monster health by 5% +0.5% per level

        if (!killed)
        {
            float distance = 2.5f;
            float Angle = UnityEngine.Random.value * 2 * Mathf.PI;
            Vector3 newEnemyPos = new Vector3(Mathf.Sin(Angle), 0, Mathf.Cos(Angle)) * distance + enemyPos;
            enemy.transform.position = newEnemyPos;
            if (FXManager.instance)
            {
                Vector3 heighOffset = new Vector3(0, 1.5f, 0);
                FXManager.instance.AddRiftFX(enemyPos + heighOffset, newEnemyPos + heighOffset);
            }
        }
    }

    public override void LevelUp()
    {
        base.LevelUp();
        Damage += 2f;

        if (_level >= 3)
        {
            Projectile = 2;
        }
        if (_level >= 5)
        {
            Projectile = 3;
        }
        if (_level == 8)
        {
            Damage += 2f;
            Projectile = 4;
        }
        if (_level == 9)
        {
            Damage += 2f;
            Projectile = 5;
            ComboCount = 2;
        }

        switch (_level)
        {
            case 1:
                DelayBetweenCombo = 1.0f;
                break;
            case 2:
                DelayBetweenCombo = .9f;
                break;
            case 3:
                DelayBetweenCombo = .8f;
                break;
            case 4:
                DelayBetweenCombo = .7f;
                break;
            case 5:
                DelayBetweenCombo = .6f;
                break;
            case 6:
                DelayBetweenCombo = .5f;
                break;
            case 7:
                DelayBetweenCombo = .45f;
                break;
            case 8:
                DelayBetweenCombo = .4f;
                break;
            case 9:
                DelayBetweenCombo = .3f;
                break;
        }

        Piercing += 6;
    }
}