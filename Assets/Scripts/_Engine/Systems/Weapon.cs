using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
	#region Variables

	[Header( "References" )]

	[HideInInspector]
	public WeaponSystem weaponSystem;

	public Transform directionTransform = null;

	[Header( "Weapon Stats" )]

	public string weaponName = null;
	public string attackControl = "Attack";

	public float damageMin;
	public float damageMax;
	public float attackDurationMin;
	public float attackDurationMax;
	public float cooldownMin;
	public float cooldownMax;

	protected bool _attacking;
	protected bool _cooldown;

	#endregion

	#region Unity Events

	private void Awake()
	{
		if ( weaponName == null || weaponName.Length == 0 )
			weaponName = name;

		enabled = false;
		OnAwake();
	}

	private void Start()
	{
		OnStart();
	}

	private void OnEnable()
	{
		if ( directionTransform == null )
		{
			directionTransform = (weaponSystem != null) ? weaponSystem.directionTransform : null;

			if ( directionTransform == null )
				directionTransform = transform;
		}

		if ( attackControl == null )
			attackControl = (weaponSystem != null) ? weaponSystem.generalAttackControl : null;

		OnEnabled();
	}

	private void OnDisable()
	{
		OnDisabled();
	}

	private void OnDestroy()
	{
		OnDispose();

		weaponSystem = null;
		directionTransform = null;
		weaponName = null;
		attackControl = null;
	}

	virtual protected void OnAwake()
	{
		// override, if necessary
	}

	virtual protected void OnStart()
	{
		// override, if necessary
	}

	virtual protected void OnEnabled()
	{
		// override, if necessary
	}

	virtual protected void OnDisabled()
	{
		// override, if necessary
	}

	virtual protected void OnDispose()
	{
		// override, if necessary
	}

	#endregion

	#region Attacking

	private void Update()
	{
		if ( CanAttack() )
			if ( Input.GetButton( attackControl ) )
				Attack();
	}

	public bool CanAttack()
	{
		return (!_attacking && !_cooldown) ? ((weaponSystem != null) ? weaponSystem.CanAttack() : false) : false;
	}

	public void Attack()
	{
		if ( !_attacking )
		{
			_attacking = true;

			OnAttack();
			Cooldown();

			CancelInvoke( "CompleteAttack" );

			float attackSpeed = CalculateAttackSpeed();

			if ( attackSpeed > 0.0f )
				Invoke( "CompleteAttack", attackSpeed );
			else
				CompleteAttack();
		}
	}

	private void CompleteAttack()
	{
		_attacking = false;
		OnAttackComplete();
	}

	virtual protected void OnAttack()
	{
		// override, if necessary
	}

	virtual protected void OnAttackComplete()
	{
		// override, if necessary
	}

	#endregion

	#region Cooldown

	public void Cooldown()
	{
		if ( !_cooldown )
		{
			_cooldown = true;

			OnCooldown();

			CancelInvoke( "CompleteCooldown" );

			float cooldown = CalculateCooldownDuration();

			if ( cooldown > 0.0f )
				Invoke( "CompleteCooldown", cooldown );
			else
				CompleteCooldown();
		}
	}

	private void CompleteCooldown()
	{
		_cooldown = false;
		OnCooldownComplete();
	}

	virtual protected void OnCooldown()
	{
		// override, if necessary
	}

	virtual protected void OnCooldownComplete()
	{
		// override, if necessary
	}

	#endregion

	#region Calculations

	virtual public float CalculateDamage()
	{
		return Random.Range( damageMin, damageMax ) * ((weaponSystem != null) ? weaponSystem.damageMultiplier : 1.0f);
	}

	virtual public float CalculateAttackSpeed()
	{
		return Random.Range( attackDurationMin, attackDurationMax ) * ((weaponSystem != null) ? weaponSystem.attackSpeedMultiplier : 1.0f);
	}

	virtual public float CalculateCooldownDuration()
	{
		return Random.Range( cooldownMin, cooldownMax ) * ((weaponSystem != null) ? weaponSystem.cooldownMultiplier : 1.0f);
	}

	virtual public float CalculateAttackAngle()
	{
		return (directionTransform != null) ? directionTransform.transform.eulerAngles.y : transform.eulerAngles.y;
	}

	virtual public Vector3 CalculateAttackDirection()
	{
		return (directionTransform != null) ? directionTransform.forward : transform.forward;
	}

	#endregion

}
