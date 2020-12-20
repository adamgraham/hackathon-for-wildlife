using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DayNightCycle : MonoBehaviour 
{
	#region Variables

	[Header( "Time Settings" )]

	public float fullDayInSeconds = 60;
	[Range( 0.0f, 1.0f )]
	public float percentNight = 0.25f;

	private int _time;
	private int _nightStart;
	private int _nightEnd;
	
	static readonly int MINUTES_IN_A_DAY = 60 * 24;

	[Header( "Sun Settings" )]

	public Light sun;
	public Color sunFullDayColor = Color.white;
	public Color sunFullNightColor = Color.black;
	public float sunFullDayIntensity = 1.0f;
	public float sunFullNightIntensity = 0.0f;

	[Header( "Animation Settings" )]
	
	public Ease ease = Ease.InOutQuad;

	private Tween _timeTween;

	#endregion

	#region Unity Events

	private void Start()
	{
		StartDay();
	}

	#endregion

	#region Time Cycle Simulation

	private void StartDay()
	{
		_nightEnd = MINUTES_IN_A_DAY;
		_nightStart = Mathf.Clamp( _nightEnd - (int)(MINUTES_IN_A_DAY * percentNight), 0, MINUTES_IN_A_DAY );
		_time = 0;

		float timeBeforeHighSun = fullDayInSeconds * GetPercentHighSun();
		float timeInBetween = fullDayInSeconds * GetPercentBetweenHighSunAndMoon();
		float timeAfterHighMoon = fullDayInSeconds - (fullDayInSeconds * GetPercentHighMoon());

		//Debug.Log( timeTillHighSun );
		//Debug.Log( timeTillHighMoon );
		//Debug.Log( timeInBetween );

		sun.DOKill();
		sun.transform.DOKill();

		if ( _timeTween != null ) 
			_timeTween.Kill();

		// time
		_timeTween = DOTween.To( () => _time, x => _time = x, MINUTES_IN_A_DAY, fullDayInSeconds ).
			SetEase( ease ).OnComplete( StartDay );

		// rotation
		Vector3 fullRotation = new Vector3( sun.transform.localEulerAngles.x, sun.transform.localEulerAngles.y + 360.0f, sun.transform.localEulerAngles.z );
		sun.transform.DOLocalRotate( fullRotation, fullDayInSeconds, RotateMode.WorldAxisAdd ).
			SetEase( Ease.Linear );

		// intensity
		sun.DOIntensity( sunFullDayIntensity, timeBeforeHighSun ).
			SetEase( ease );
		sun.DOIntensity( sunFullNightIntensity, timeInBetween ).
			SetEase( ease ).SetDelay( timeBeforeHighSun );
		sun.DOIntensity( sun.intensity, timeAfterHighMoon ).
			SetEase( ease ).SetDelay( timeBeforeHighSun + timeInBetween );

		// color
		sun.DOColor( sunFullDayColor, timeBeforeHighSun ).
			SetEase( ease );
		sun.DOColor( sunFullNightColor, timeInBetween ).
			SetEase( ease ).SetDelay( timeBeforeHighSun );
		sun.DOColor( sun.color, timeAfterHighMoon ).
			SetEase( ease ).SetDelay( timeBeforeHighSun + timeInBetween );
	}

	#endregion

	#region Helper Methods

	public bool IsDayTime()
	{
		return !IsNightTime();
	}

	public bool IsNightTime()
	{
		return _time >= _nightStart && _time < _nightEnd;
	}

	private float GetPercentDay()
	{
		return 1.0f - percentNight;
	}

	private float GetPercentNight()
	{
		return percentNight;
	}

	private float GetPercentHighSun()
	{
		return (GetPercentDay() * 0.5f);
	}

	private float GetPercentHighMoon()
	{
		return GetPercentDay() + (GetPercentNight() * 0.5f);
	}

	private float GetPercentBetweenHighSunAndMoon()
	{
		return Mathf.Abs( GetPercentHighSun() - GetPercentHighMoon() );
	}

	#endregion

}
