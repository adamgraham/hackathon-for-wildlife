using UnityEngine;
using System.Collections;

public interface IKillable
{
	void OnDamage( float health, float delta );
	void OnHeal( float health, float delta );
	void OnKill( int lives );
	void OnResetHealth( float health );
	void OnResetLives( int lives );

	HealthSystem GetHealthSystem();
}
