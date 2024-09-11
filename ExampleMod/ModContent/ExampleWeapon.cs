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
        WeaponStats.ComboCount.SetDefaultBaseStat(1);
        WeaponStats.ProjectileCount.SetDefaultBaseStat(1);
        WeaponStats.DelayBetweenCombo.SetDefaultBaseStat(1.0f);
        WeaponStats.DelayBetweenAttack.SetDefaultBaseStat(0.1f);
        WeaponStats.Damage.SetDefaultBaseStat(20);
        WeaponStats.ProjectilePiercing.SetDefaultBaseStat(6);
        DamageSource = "ExampleWeaponDamageSource";
        _weaponProjectile = Resources.Load<GameObject>("WeaponProjectiles/BoltProjectile");
    }
    public override void OnAttack(PlayerEntity Owner, AttackInformation attackInformation)
    {
        if (EnemyManager.GetEnemiesCount > 0)
        {

            List<DefaultProjectileAI> projectileList = GetHealthyTargeting(GameData.GetConstructor(typeof(DefaultProjectileAI)), Owner);


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
                projectileGO.transform.localScale = Vector3.one * Owner.PlayerStats.ProjectileSize.GetValue();
                DefaultProjectileAI projectile = projectileList[i];
                projectile.Owner = Owner;
                projectile.damageSource = DamageSource;
                projectile.DefencePiercing = Owner.PlayerStats.DefencePiercing.Value;
                DamageMultiplierData DamageMultiplier = Owner.GetDamageMultiplier;
                projectile.DamageValue = Damage * DamageMultiplier.DamageMultiplier;
                projectile.CriticalStack = DamageMultiplier.CriticalStack;
                projectile.Piercing = (int)(Piercing + Owner.PlayerStats.ProjectilePiercing.GetValue()) + 1;
                projectile.Speed = 25 * Owner.PlayerStats.ProjectileSpeed.GetValue() * (0.75f + UnityEngine.Random.value * 0.25f); ;
                projectile.LifeTime = Owner.PlayerStats.ProjectileLifeTime.GetValue();
                projectile.knockback = Owner.PlayerStats.KnockBack.GetValue() * 0.75f;
                foreach(var modifier in WeaponModifiers)
                {
                    modifier.AttachToProjectile(projectile, this);
                }
                projectile.Awake();
            }
        }

    }

    public void OnEnemyHit(IEntity entityHitted, DefaultProjectileAI projectile, DamageInformation damageInformation)
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

        bool killed = entityHitted.TakeDamage(new DamageInformationRef(projectile.Owner, BonusDamage, 0, 0.1f,0, 0, new Vector2(direction.x, direction.z), DamageSource, this));


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
        WeaponStats.Damage.AddBaseStat(2f);

        if (_level >= 3)
        {
            WeaponStats.ProjectileCount.SetDefaultBaseStat(2);
        }
        if (_level >= 5)
        {
            WeaponStats.ProjectileCount.SetDefaultBaseStat(3);
        }
        if (_level == 8)
        {
            WeaponStats.Damage.AddBaseStat(2f);
            WeaponStats.ProjectileCount.SetDefaultBaseStat( 4);
        }
        if (_level == 9)
        {
            WeaponStats.Damage.AddBaseStat(2f);
            WeaponStats.ProjectileCount.SetDefaultBaseStat(5);
            WeaponStats.ComboCount.SetDefaultBaseStat(2);
        }

        switch (_level)
        {
            case 1:
                WeaponStats.DelayBetweenCombo.SetDefaultBaseStat(1.0f);
                break;
            case 2:
                WeaponStats.DelayBetweenCombo.SetDefaultBaseStat(.9f);
                break;
            case 3:
                WeaponStats.DelayBetweenCombo.SetDefaultBaseStat(.8f);
                break;
            case 4:
                WeaponStats.DelayBetweenCombo.SetDefaultBaseStat(.7f);
                break;
            case 5:
                WeaponStats.DelayBetweenCombo.SetDefaultBaseStat(.6f);
                break;
            case 6:
                WeaponStats.DelayBetweenCombo.SetDefaultBaseStat(.5f);
                break;
            case 7:
                WeaponStats.DelayBetweenCombo.SetDefaultBaseStat(.45f);
                break;
            case 8:
                WeaponStats.DelayBetweenCombo.SetDefaultBaseStat(.4f);
                break;
            case 9:
                WeaponStats.DelayBetweenCombo.SetDefaultBaseStat(.3f);
                break;
        }

        WeaponStats.ProjectilePiercing.AddBaseStat(6);
    }
}