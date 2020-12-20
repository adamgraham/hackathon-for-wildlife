using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponSystem : MonoBehaviour
{
	#region Variables

	[HideInInspector]
	internal Character character;

	public Transform directionTransform = null;
	public string generalAttackControl = "Attack";

	[Header( "Weapons" )]

	public List<Weapon> weapons;

	protected Weapon _currentWeapon;
	protected int _currentWeaponIndex;

	[Header( "Weapon Switching" )]

	public bool canSwitchWeapons = false;
	public string switchWeaponNextControl = null;
	public string switchWeaponPreviousControl = null;

	[Header( "Calculation Multipliers" )]

	public float damageMultiplier = 1.0f;
	public float attackSpeedMultiplier = 1.0f;
	public float cooldownMultiplier = 1.0f;

	#endregion

	#region Unity Events

	private void Awake()
	{
		if ( character == null )
			character = gameObject.GetComponent<Character>();

		DisableAllWeapons();
		SwitchWeapon( 0 );
	}

	private void OnEnable()
	{
		if ( _currentWeapon != null )
			_currentWeapon.enabled = true;
	}

	private void OnDisable()
	{
		DisableAllWeapons();
	}

	#endregion

	#region General

	public void DisableAllWeapons()
	{
		if ( weapons != null )
		{
			int len = weapons.Count;
			for ( int i = 0; i < len; i++ )
				weapons[i].enabled = false;
		}
	}

	public bool CanAttack()
	{
		return ((character != null) ? (character.IsSpawned() && character.enabled) : false);
	}

	public GameObject GetCharacterGameObject()
	{
		return (character != null) ? character.gameObject : null;
	}

	#endregion

	#region Weapon Switching

	private void Update()
	{
		if ( canSwitchWeapons )
		{
			if ( switchWeaponNextControl != null && switchWeaponNextControl.Length > 0 )
				if ( Input.GetKeyDown( switchWeaponNextControl ) )
					NextWeapon();

			if ( switchWeaponPreviousControl != null && switchWeaponPreviousControl.Length > 0 )
				if ( Input.GetKeyDown( switchWeaponPreviousControl ) )
					PreviousWeapon();
		}
	}

	public void SwitchWeapon( int index )
	{
		if ( weapons != null )
		{
			if ( weapons.Count > 0 )
			{
				if ( index < 0 )
					index = weapons.Count - 1;
				else if ( index >= weapons.Count ) 
					index = 0;

				if ( _currentWeapon != null )
					_currentWeapon.weaponSystem = null;

				_currentWeaponIndex = index;
				_currentWeapon = weapons[_currentWeaponIndex];

				if ( _currentWeapon != null )
				{
					_currentWeapon.weaponSystem = this;
					_currentWeapon.enabled = true;
				}
			}
		}
	}

	public void NextWeapon()
	{
		SwitchWeapon( _currentWeaponIndex + 1 );
	}

	public void PreviousWeapon()
	{
		SwitchWeapon( _currentWeaponIndex - 1 );
	}

	#endregion

	#region Weapon Adding/Removing

	public void AddWeapon( Weapon weapon )
	{
		if ( weapon != null )
		{
			if ( weapons != null )
			{
				if ( !weapons.Contains( weapon ) )
				{
					weapons.Add( weapon );
					weapon.enabled = false;
				}
			}
		}
	}

	public void RemoveWeapon( Weapon weapon )
	{
		if ( weapon != null )
		{
			if ( weapons != null )
			{
				if ( weapons.Contains( weapon ) )
				{
					weapons.Remove( weapon );
					weapon.enabled = false;
				}
			}
		}
	}

	#endregion

}
