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
using UnityEngine.VFX;

public class ExamplePetData : PetData
{

    public override string GetName()
    {
        return "Mini-Rog";
    }

    public override string GetDescription()
    {
        return "Mini-Rog will help you using his bow, and picking-up soul-gems";
    }

    public override List<PetBehaviour> GetPetBehaviours()
    {
		return new List<PetBehaviour>
			{
				new BowAttackBehaviour(),
				new MoveToADifferentPositionPetBehaviour(),
				new BowAttackAndMoveBehaviour(),
				new PickExperienceBehaviour(),
			};
	}

}


public class BowAttackAndMoveBehaviour : MoveToADifferentPositionPetBehaviour
{

	float attackTimer = 0;
	GameObject _weaponProjectile;

    public override void OnPickBehaviour(Pet pet)
    {
        base.OnPickBehaviour(pet);
		_weaponProjectile = Resources.Load<GameObject>("WeaponProjectiles/ArrowProjectile");
	}

    public override string GetBehaviourName()
    {
        return "ExampleMod_MoveAndAttack";
    }

    public override void Update(Pet pet)
    {
		base.Update(pet);
		attackTimer -= Time.deltaTime;

		if (attackTimer < 0)
		{


			if (EnemyManager.GetEnemiesCount > 0)
			{
				attackTimer = 5.0f * pet.Player.GetPlayerStats.AttackDelay.Value;



				List<DefaultProjectilAI> projectilesList = new List<DefaultProjectilAI>();

				float projectilecount = 1 + pet.Player.GetPlayerStats.AdditionalProjectile.Value;

				GameObject[] Destructibles = GameObject.FindGameObjectsWithTag("Destructible");

				List<GameObject> TargetCandidate = new List<GameObject>();

				for (int j = 0; j < EnemyManager.GetEnemiesCount; j++)
				{
					if (EnemyManager.GetEnemy(j).dontTargetMe)
						continue;

					TargetCandidate.Add(EnemyManager.GetEnemy(j).gameObject);
				}


				float mindistance = 99999999;
				List<GameObject> TargetList = new List<GameObject>();
				GameObject Target = null;
				GameObject tempTarget = null;
				int selectedID = -1;
				for (int i = 0; i < (projectilecount); i++)
				{
					mindistance = 99999999;
					Target = null;
					for (int j = TargetCandidate.Count - 1; j >= 0; j--)
					{
						tempTarget = TargetCandidate[j];
						float distance = (pet.transform.position - tempTarget.transform.position).sqrMagnitude;
						if (distance < mindistance)
						{
							Target = tempTarget;
							mindistance = distance;
							selectedID = j;
						}
					}

					if (Target == null)
						break;

					TargetCandidate.RemoveAt(selectedID);
					TargetList.Add(Target);

				}

				if (TargetList.Count < projectilecount)
				{
					for (int i = 0; i < Destructibles.Length; i++)
					{
						/*if (!Destructibles[i].activeSelf)
							continue;*/
						if (Vector3.Distance(Destructibles[i].transform.position, pet.transform.position) <= 20)
						{
							TargetList.Add(Destructibles[i]);
							break;
						}
					}
				}

				for (int i = 0; i < TargetList.Count; i++)
				{
					if (TargetList[i] != null)
					{
						Vector3 ProjectileDirection = (TargetList[i].transform.position - pet.transform.position).normalized;
						DefaultProjectilAI poolable = new DefaultProjectilAI();
						poolable.PoolPrefabAssociated = _weaponProjectile;

						projectilesList.Add(poolable);
						ProjectileManager.SpawnProjectile(poolable, pet.transform.position + new Vector3(0, 1.4f + UnityEngine.Random.Range(-0.1f, 0.1f), 0), Quaternion.LookRotation(ProjectileDirection));
					}
				}


				for (int i = 0; i < projectilesList.Count; i++)
				{
					GameObject projectileGO = projectilesList[i].gameObject;
					projectileGO.transform.localScale = Vector3.one * pet.Player.GetPlayerStats.ProjectileSize.GetValue();
					DefaultProjectilAI projectile = projectilesList[i];
					projectile.Owner = pet.Player;
					projectile.damageSource = "MiniRog";
					DamageMultiplierData DamageMultiplier = pet.Player.GetDamageMultiplier;
					projectile.DamageValue = 2 * DamageMultiplier.DamageMultiplier;
					projectile.CriticalStack = DamageMultiplier.CriticalStack;
					projectile.DefencePiercing = pet.Player.GetPlayerStats.DefencePiercing.Value;
					projectile.Piercing = (int)(2 + pet.Player.GetPlayerStats.ProjectilePiercing.GetValue()) + 1;
					projectile.Speed = 15 * pet.Player.GetPlayerStats.ProjectileSpeed.GetValue() * (0.75f + UnityEngine.Random.value * 0.25f);
					projectile.LifeTime = pet.Player.GetPlayerStats.ProjectileLifeTime.GetValue();
					projectile.knockback = pet.Player.GetPlayerStats.KnockBack.GetValue() * 0.75f;

					projectile.modifierLevel = pet.Player.ClonePlayerModifier;
					projectile.Awake();
				}
			}

			

		}
		
	}
}


	public class PickExperienceBehaviour : PetBehaviour
{

	bool _collectedGem = false;

	Collectible _collectible;


	public override float GetPickWeight(Pet pet)
	{
		if (CollectibleManager.instance.CollectibleList.ContainsKey("SoulGem") && CollectibleManager.instance.CollectibleList["SoulGem"].Count > 0)
		{
			return 3f;
		}

		return 0f;
	}

    public override bool CanChangebehaviour(Pet pet)
    {
        return _collectedGem || _collectible == null;
    }
	float _movespeed = 5;

	public override void OnPickBehaviour(Pet pet)
    {
		_collectible = CollectibleManager.instance.CollectibleList["SoulGem"][UnityEngine.Random.Range(0, CollectibleManager.instance.CollectibleList["SoulGem"].Count - 1)];
		_movespeed = pet.Player.GetPlayerStats.MoveSpeed.Value * 1.25f;

	}

    public override string GetBehaviourName()
    {
        return "Example_PickExperienceBehaviour";
    }

	Vector3 GetCurrentTargetPosition(PlayerEntity playerEntity)
	{
		return _collectible.transform.position;
	}

	public override void Update(Pet pet)
    {


		

		if (_collectible == null)
        {
			return;
			PixelAnimationData idleAnimation = GameData.SelectedPet.petAnimations.IdleAnimation;
			pet.Sprite.GetComponent<Renderer>().material.SetTexture("SpriteSheet", idleAnimation.Texture);
			pet.Sprite.GetComponent<Renderer>().material.SetVector("SpriteSheetSize", idleAnimation.FrameCount);
			pet.Sprite.GetComponent<Renderer>().material.SetVector("SpriteSheetAnimation", idleAnimation.FrameCount);
		}
			

		Vector3 targetPos = GetCurrentTargetPosition(pet.Player);

		Vector3 offset = targetPos - pet.transform.position;

		Vector3 direction = Vector3.zero;

		float distance = Mathf.Max(0, offset.magnitude);


		if (distance < 1)
        {
			_collectible.VoidBonus = true;
			_collectedGem = true;

			PixelAnimationData idleAnimation = GameData.SelectedPet.petAnimations.IdleAnimation;
			pet.Sprite.GetComponent<Renderer>().material.SetTexture("SpriteSheet", idleAnimation.Texture);
			pet.Sprite.GetComponent<Renderer>().material.SetVector("SpriteSheetSize", idleAnimation.FrameCount);
			pet.Sprite.GetComponent<Renderer>().material.SetVector("SpriteSheetAnimation", idleAnimation.FrameCount);
			return;
        }

		PixelAnimationData animation = GameData.SelectedPet.petAnimations.RunAnimation;

		if (distance > 0.1f || pet.Player.ActualMoveSpeed > 0)
		{
			pet.AnachronisticFX.SetBool("Idle", false);
			direction = offset / distance;


			Vector3 screenSpaceMovement = Quaternion.Euler(0, -45, 0) * direction;


			if (Mathf.Abs(screenSpaceMovement.x) < 0.01f)
			{

				if (screenSpaceMovement.z > 0)
				{
					pet.Sprite.transform.localScale = new Vector3(1, 1, 1);
					pet.AnachronisticFX.SetBool("LookLeft", false);
				}
				else
				{
					pet.Sprite.transform.localScale = new Vector3(-1, 1, 1);
					pet.AnachronisticFX.SetBool("LookLeft", true);
				}

			}
			else
			{
				if ((Quaternion.Euler(0, -45, 0) * direction).x > 0)
				{
					pet.Sprite.transform.localScale = new Vector3(1, 1, 1);
					pet.AnachronisticFX.SetBool("LookLeft", false);
				}
				else
				{
					pet.Sprite.transform.localScale = new Vector3(-1, 1, 1);
					pet.AnachronisticFX.SetBool("LookLeft", true);
				}
			}

			if (distance > 0)
			{
				pet.transform.position = pet.transform.position + direction * Mathf.Min(distance, _movespeed * Time.deltaTime);
			}

		}
		else
		{
			pet.AnachronisticFX.SetBool("Idle", true);
			direction = Vector3.zero;
			animation = GameData.SelectedPet.petAnimations.IdleAnimation;

		}


		if (distance > _movespeed * Time.deltaTime)
		{
			pet.Sprite.GetComponent<Renderer>().material.SetFloat("AnimationSpeed", 1);
			pet.AnachronisticFX.SetFloat("AnimationSpeed", 1);
		}
		else
		{
			pet.Sprite.GetComponent<Renderer>().material.SetFloat("AnimationSpeed", 0.75f);
			pet.AnachronisticFX.SetFloat("AnimationSpeed", 0.75f);
		}



		pet.Sprite.GetComponent<Renderer>().material.SetTexture("SpriteSheet", animation.Texture);
		pet.Sprite.GetComponent<Renderer>().material.SetVector("SpriteSheetSize", animation.FrameCount);
		pet.Sprite.GetComponent<Renderer>().material.SetVector("SpriteSheetAnimation", animation.FrameCount);

	}
}

public class BowAttackBehaviour : IdlePetBehaviour
{
	GameObject _weaponProjectile;

    public override float GetPickWeight(Pet pet)
    {
		if (Vector3.SqrMagnitude(pet.Player.transform.position - pet.transform.position) > 100f)
		{
			return 0f;
		}

		return 2f;
	}
    public override void OnPickBehaviour(Pet pet)
    {
		m_durationLeft = 10f;
		PixelAnimationData idleAnimation = GameData.SelectedPet.petAnimations.IdleAnimation;
		pet.Sprite.GetComponent<Renderer>().material.SetTexture("SpriteSheet", idleAnimation.Texture);
		pet.Sprite.GetComponent<Renderer>().material.SetVector("SpriteSheetSize", idleAnimation.FrameCount);
		pet.Sprite.GetComponent<Renderer>().material.SetVector("SpriteSheetAnimation", idleAnimation.FrameCount);
		_weaponProjectile = Resources.Load<GameObject>("WeaponProjectiles/ArrowProjectile");

	}


    public override bool CanChangebehaviour(Pet pet)
    {
		if (!(m_durationLeft <= 0f))
		{
			return Vector3.SqrMagnitude(pet.Player.transform.position - pet.transform.position) > 100;
		}

		return true;
	}

    public override string GetBehaviourName()
    {
        return "ExampleMod_BowAttack";
    }

    float attackTimer = 0;

    public override void Update(Pet pet)
    {
		base.Update(pet);

		attackTimer -= Time.deltaTime;

        if (attackTimer < 0)
        {
            

			if (EnemyManager.GetEnemiesCount > 0)
			{
				attackTimer = 2.0f * pet.Player.GetPlayerStats.AttackDelay.Value;



				List<DefaultProjectilAI> projectilesList = new List<DefaultProjectilAI>();

				float projectilecount = 1 + pet.Player.GetPlayerStats.AdditionalProjectile.Value;

				GameObject[] Destructibles = GameObject.FindGameObjectsWithTag("Destructible");

				List<GameObject> TargetCandidate = new List<GameObject>();

				for (int j = 0; j < EnemyManager.GetEnemiesCount; j++)
				{
					if (EnemyManager.GetEnemy(j).dontTargetMe)
						continue;

					TargetCandidate.Add(EnemyManager.GetEnemy(j).gameObject);
				}


				float mindistance = 99999999;
				List<GameObject> TargetList = new List<GameObject>();
				GameObject Target = null;
				GameObject tempTarget = null;
				int selectedID = -1;
				for (int i = 0; i < (projectilecount); i++)
				{
					mindistance = 99999999;
					Target = null;
					for (int j = TargetCandidate.Count - 1; j >= 0; j--)
					{
						tempTarget = TargetCandidate[j];
						float distance = (pet.transform.position - tempTarget.transform.position).sqrMagnitude;
						if (distance < mindistance)
						{
							Target = tempTarget;
							mindistance = distance;
							selectedID = j;
						}
					}

					if (Target == null)
						break;

					TargetCandidate.RemoveAt(selectedID);
					TargetList.Add(Target);

				}

				if (TargetList.Count < projectilecount)
				{
					for (int i = 0; i < Destructibles.Length; i++)
					{
						/*if (!Destructibles[i].activeSelf)
							continue;*/
						if (Vector3.Distance(Destructibles[i].transform.position, pet.transform.position) <= 20)
						{
							TargetList.Add(Destructibles[i]);
							break;
						}
					}
				}

				for (int i = 0; i < TargetList.Count; i++)
				{
					if (TargetList[i] != null)
					{
						Vector3 ProjectileDirection = (TargetList[i].transform.position - pet.transform.position).normalized;
						DefaultProjectilAI poolable = new DefaultProjectilAI();
						poolable.PoolPrefabAssociated = _weaponProjectile;

						projectilesList.Add(poolable);
						ProjectileManager.SpawnProjectile(poolable, pet.transform.position + new Vector3(0, 1.4f + UnityEngine.Random.Range(-0.1f, 0.1f), 0), Quaternion.LookRotation(ProjectileDirection));
					}
				}


				for (int i = 0; i < projectilesList.Count; i++)
				{
					GameObject projectileGO = projectilesList[i].gameObject;
					projectileGO.transform.localScale = Vector3.one * pet.Player.GetPlayerStats.ProjectileSize.GetValue();
					DefaultProjectilAI projectile = projectilesList[i];
					projectile.Owner = pet.Player;
					projectile.damageSource = "MiniRog";
					DamageMultiplierData DamageMultiplier = pet.Player.GetDamageMultiplier;
					projectile.DamageValue = 2 * DamageMultiplier.DamageMultiplier;
					projectile.CriticalStack = DamageMultiplier.CriticalStack;
					projectile.DefencePiercing = pet.Player.GetPlayerStats.DefencePiercing.Value;
					projectile.Piercing = (int)(2 + pet.Player.GetPlayerStats.ProjectilePiercing.GetValue()) + 1;
					projectile.Speed = 15 * pet.Player.GetPlayerStats.ProjectileSpeed.GetValue() * (0.75f + UnityEngine.Random.value * 0.25f);
					projectile.LifeTime = pet.Player.GetPlayerStats.ProjectileLifeTime.GetValue();
					projectile.knockback = pet.Player.GetPlayerStats.KnockBack.GetValue() * 0.75f;

					projectile.modifierLevel = pet.Player.ClonePlayerModifier;
					projectile.Awake();
				}
			}

		}
    }

}

