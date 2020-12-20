using UnityEngine;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
	#region Variables

	[Header( "Settings" )]

	public float maxHealth = 100.0f;
	public int maxLives = 1;

	public bool invulnerable = false;
	public bool infiniteLives = false;

	private IKillable _object;
	private float _health;
	private int _lives;

	[Header( "UI" )]

	public EnergyBar healthBar = null;

	[Header( "Callbacks" )]

	public HealthSystemCallback onDamage;
	public HealthSystemCallback onHeal;
	public HealthSystemCallback onKill;
	public delegate void HealthSystemCallback();

	#endregion

	#region Unity Events

	private void Start()
	{
		_health = maxHealth;
		_lives = maxLives;

		_object = gameObject.GetComponent( typeof( IKillable ) ) as IKillable;

		if ( healthBar != null )
			healthBar.value = GetHealthPercent();
	}

	private void OnDestroy()
	{
		onDamage = null;
		onHeal = null;
		onKill = null;
		healthBar = null;

		_object = null;
	}

	#endregion

	#region Health System

	public float Damage( float amount )
	{
		if ( amount < 0.0f ) 
			return Heal( amount );

		if ( !invulnerable )
		{
			_health -= amount;
			_object.OnDamage( _health, amount );

			if ( onDamage != null )
				onDamage();

			if ( healthBar != null )
				healthBar.Deplete( amount / maxHealth, false );

			if ( _health < 0.0f )
				Kill();
		}

		return _health;
	}

	public float Heal( float amount )
	{
		if ( amount < 0.0f ) 
			return Damage( amount );
		
		_health += amount;
		_object.OnHeal( _health, amount );

		if ( onHeal != null )
			onHeal();

		if ( healthBar != null )
			healthBar.Charge( amount / maxHealth );

		if ( _health > maxHealth ) 
			_health = maxHealth;

		return _health;
	}

	public void Kill()
	{
		if ( !infiniteLives )
			--_lives;

		_object.OnKill( _lives );

		if ( onKill != null )
			onKill();

		if ( HasLives() )
			ResetHealth();
	}

	public float ResetHealth()
	{
		_health = maxHealth;
		_object.OnResetHealth( _health );

		return _health;
	}

	public int ResetLives()
	{
		_lives = maxLives;
		_object.OnResetLives( _lives );

		return _lives;
	}

	public float GetHealth()
	{
		return _health;
	}

	public float GetHealthPercent()
	{
		return _health / maxHealth;
	}

	public int GetLives()
	{
		return _lives;
	}

	public bool IsFullHealth()
	{
		return _health >= maxHealth;
	}

	public bool IsFullLives()
	{
		return _lives >= maxLives;
	}

	public bool IsOutOfHealth()
	{
		return _health <= 0.0f;
	}

	public bool IsOutOfLives()
	{
		return _lives <= 0;
	}

	public bool HasHealth()
	{
		return _health > 0.0f;
	}

	public bool HasLives()
	{
		return _lives > 0;
	}

	#endregion

}
