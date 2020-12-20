using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

[RequireComponent( typeof( RectTransform ) )]
[RequireComponent( typeof( CanvasRenderer ) )]
[System.Serializable]
public class EnergyBar : MonoBehaviour, IPauseable 
{
	#region Variables

	[Header( "Construction" )]

	public Vector2 barSize;
	public Color barColor;
	public BarType barType;
	public enum BarType { LeftToRight, RightToLeft }

	private EnergyBarQuadrant _bar;
	private EnergyBarQuadrant _burnOffBar;
	private EnergyBarQuadrant _burnOnBar;

	[Header( "Settings" )]

	[Range( 0.0f, 1.0f )]
	public float defaultValue = 1.0f;
	public float fullRechargeDuration;
	public float fullDepleteDuration;

	private bool _charging;
	private bool _depleting;
	private bool _autoUpdateActualValue;
	private bool _rechargeOnDeplete;
	private bool _paused;

	[Header( "Text" )]

	public Text barText;
	public string barTextPrecursor = "";
	public TextStyle barTextStyle;
	public enum TextStyle { Percents, Integers, Decimals };

	[Header( "Animation" )]

	public Ease chargeEase = Ease.Linear;
	public Ease depleteEase = Ease.Linear;
	public AnimationType animationType;
	public enum AnimationType { Scale, BurnOff };

	// These properties will be shown or hidden by the CustomEditor
	[HideInInspector] public Color burnOffColor = Color.red;
	[HideInInspector] public Color burnOnColor = Color.green;
	[HideInInspector] public float burnTimeDelayPercent;

	private Tweener _valueTween;
	private float _valueCurrent;
	private float _valueActual;

	private float _burnOffValue;
	private float _burnOnValue;
	private float _burnOnNewValue;

	private float _posXMin;
	private float _posXMax;

	#endregion

	#region Unity Events

	private void Start()
	{
		_valueCurrent = 1.0f;
		_valueActual = _valueCurrent;

		_bar = EnergyBarQuadrant.Create( this );
		_bar.SetSize( barSize.x, barSize.y );
		_bar.SetBarType( barType );
		_bar.SetColor( barColor );
		_bar.name = "Energy Bar Quadrant";
		_bar.value = defaultValue;
		_bar.enabled = isActiveAndEnabled;

		_burnOffBar = _bar.Clone( this );
		_burnOffBar.name = "Burn Off Bar Quadrant";
		_burnOffBar.SetColor( burnOffColor );
		_burnOffBar.value = 0.0f;

		_burnOnBar = _bar.Clone( this );
		_burnOnBar.name = "Burn On Bar Quadrant";
		_burnOnBar.SetColor( burnOnColor );
		_burnOnBar.value = 0.0f;

		_posXMin = _bar.transform.localPosition.x;
		_posXMax = _bar.transform.localPosition.x + barSize.x;
	}

	private void OnDestroy()
	{
		if ( _valueTween != null )
			_valueTween.Kill();

		_bar = null;
		_burnOffBar = null;
		_valueTween = null;
	}

	private void OnEnable()
	{
		if ( _bar != null )
			_bar.enabled = true;
	}

	private void OnDisable()
	{
		if ( _bar != null )
			_bar.enabled = false;
	}

	#endregion

	#region General

	public float value
	{
		get { return _valueCurrent; }
		set
		{
			_valueCurrent = value;

			if ( _bar != null )
				_bar.value = Mathf.Clamp( _valueCurrent, 0.0f, 1.0f );

			if ( _autoUpdateActualValue ) 
				_valueActual = _valueCurrent;

			SetBarText();
		}
	}

	private float burnOffValue
	{
		get { return _burnOffValue; }
		set
		{
			_burnOffValue = value;

			if ( _burnOffBar != null )
				_burnOffBar.value = Mathf.Clamp( _burnOffValue, 0.0f, 1.0f );
		}
	}

	private float burnOnValue
	{
		get { return _burnOnValue; }
		set
		{
			_burnOnValue = value;

			if ( _burnOnBar != null )
				_burnOnBar.value = Mathf.Clamp( _burnOnValue, 0.0f, 1.0f );
		}
	}

	public float valuePercent
	{
		get { return Mathf.Clamp( _valueCurrent, 0.0f, 1.0f ); }
	}

	public void KillTween()
	{
		if ( _valueTween != null )
			_valueTween.Kill();
	}

	public bool IsFullyCharged()
	{
		return value >= 1.0f;
	}

	public bool IsFullyDepleted()
	{
		return value <= 0.0f;
	}

	public bool IsEmpty()
	{
		return value <= 0.0f;
	}

	public bool HasStamina()
	{
		return value > 0.0f;
	}

	private BarType GetOppositeBarType( BarType type )
	{
		if ( type == BarType.LeftToRight )
			return BarType.RightToLeft;
		else if ( type == BarType.RightToLeft )
			return BarType.LeftToRight;

		return 0;
	}

	private void SetBarText()
	{
		if ( barText != null )
		{
			barText.text = barTextPrecursor;

			if ( barText.text.Length > 0 )
				barText.text += " ";

			switch ( barTextStyle )
			{
				case TextStyle.Percents:
					barText.text += ((int)(_valueCurrent * 100.0f)).ToString() + "%";
					break;

				case TextStyle.Integers:
					barText.text += ((int)(_valueCurrent * 100.0f)).ToString();
					break;

				case TextStyle.Decimals:
					barText.text += _valueCurrent.ToString( "F2" );
					break;
			}
		}
	}

	#endregion

	#region Deplete

	public void Deplete( bool rechargeOnComplete = true )
	{
		// deplete to empty
		Deplete( 1.0f, rechargeOnComplete, true );
	}

	public void Deplete( float amountOfQuadrants, bool rechargeOnComplete = true )
	{
		Deplete( amountOfQuadrants, rechargeOnComplete, false );
	}

	private void Deplete( float percent, bool rechargeOnComplete, bool autoDepleting )
	{
		if ( percent > 0.0f )
		{
			if ( _valueTween != null )
				_valueTween.Kill();

			_charging = false;
			_depleting = true;
			_autoUpdateActualValue = false;
			_rechargeOnDeplete = rechargeOnComplete;

			float oldValue = _valueActual;
			float newValue = Mathf.Clamp( _valueActual - percent, 0.0f, 1.0f );

			if ( autoDepleting ) _autoUpdateActualValue = true;
			else _valueActual = newValue;

			if ( animationType == AnimationType.Scale || autoDepleting )
			{
				_valueTween = DOTween.To( () => value, x => value = x, newValue, fullDepleteDuration * percent ).
					SetEase( depleteEase ).
					OnComplete( OnDepleteComplete );
			}
			else if ( animationType == AnimationType.BurnOff )
			{
				value = newValue;
				burnOffValue = Mathf.Abs( oldValue - newValue );

				if ( _burnOffBar != null )
					_burnOffBar.transform.localPosition = new Vector3( _posXMin + (barSize.x * newValue), 0.0f, 0.0f );

				float duration = fullDepleteDuration * percent;
				float delay = duration * burnTimeDelayPercent;
				duration -= delay;

				_valueTween = DOTween.To( () => burnOffValue, x => burnOffValue = x, 0.0f, duration ).
					SetDelay( delay ).
					SetEase( depleteEase ).
					OnKill( OnBurnOffKilled ).
					OnComplete( OnDepleteComplete );
			}
		}
	}

	public bool IsDepleting()
	{
		return _depleting;
	}

	private void OnDepleteComplete()
	{
		_depleting = false;
		_valueActual = _valueCurrent;
		_valueTween = null;

		if ( _rechargeOnDeplete )
		{
			CancelInvoke( "Charge" );
			Invoke( "Charge", Mathf.Epsilon );
		}
	}

	private void OnBurnOffKilled()
	{
		if ( _burnOffBar != null )
			_burnOffBar.value = 0.0f;
	}

	#endregion

	#region Charge

	public void Charge()
	{
		// charge to full
		Charge( 1.0f, true );
	}

	public void Charge( float amountOfQuadrants )
	{
		Charge( amountOfQuadrants, false );
	}

	private void Charge( float percent, bool autoCharging )
	{
		CancelInvoke( "Charge" );

		if ( percent > 0.0f )
		{
			if ( _valueTween != null ) 
				_valueTween.Kill();

			_charging = true;
			_depleting = false;
			_autoUpdateActualValue = false;

			float oldValue = _valueActual;
			float newValue = Mathf.Clamp( _valueActual + percent, 0.0f, 1.0f );

			if ( autoCharging ) _autoUpdateActualValue = true;
			else _valueActual = newValue;

			if ( animationType == AnimationType.Scale || autoCharging )
			{
				_valueTween = DOTween.To( () => value, x => value = x, newValue, fullRechargeDuration * (1.0f - _valueCurrent) ).
					SetEase( chargeEase ).
					OnComplete( OnChargeComplete );
			}
			else if ( animationType == AnimationType.BurnOff )
			{
				_burnOnNewValue = newValue;
				burnOnValue = 0.0f;

				if ( _burnOnBar != null )
					_burnOnBar.transform.localPosition = new Vector3( _posXMin + (barSize.x * oldValue), 0.0f, 0.0f );

				float duration = fullRechargeDuration * percent;
				float delay = duration * burnTimeDelayPercent;
				duration -= delay;

				_valueTween = DOTween.To( () => burnOnValue, x => burnOnValue = x, Mathf.Abs( oldValue - newValue ), duration ).
					SetDelay( delay ).
					SetEase( chargeEase ).
					OnKill( OnBurnOnKilled ).
					OnComplete( OnChargeComplete );
			}
		}
	}

	public bool IsCharging()
	{
		return _charging;
	}

	private void OnChargeComplete()
	{
		_charging = false;
		_valueActual = _valueCurrent;
		_valueTween = null;
	}

	private void OnBurnOnKilled()
	{
		if ( _burnOnBar != null )
			_burnOnBar.value = 0.0f;

		value = _burnOnNewValue;

		CancelInvoke( "Charge" );
		Invoke( "Charge", Mathf.Epsilon );
	}

	#endregion

	#region Animations

	public void Flash( Color color, float duration, float interval )
	{
		if ( _bar != null )
			_bar.Flash( color, duration, interval );
	}

	public void FadeIn( float duration, float delay = 0.0f )
	{
		if ( _bar != null )
			_bar.FadeIn( duration, delay );
	}

	public void FadeOut( float duration, float delay = 0.0f )
	{
		if ( _bar != null )
			_bar.FadeOut( duration, delay );
	}

	public void FadeInOut( float inDuration, float outDuration, float inDelay = 0.0f, float outDelay = 0.0f )
	{
		if ( _bar != null )
			_bar.FadeInOut( inDuration, outDuration, inDelay, outDelay );
	}

	#endregion

	#region Pausing

	public void Pause()
	{
		if ( !_paused )
		{
			_paused = true;

			if ( _valueTween != null ) 
				_valueTween.Pause();
		}
	}

	public void Unpause()
	{
		if ( _paused )
		{
			_paused = false;

			if ( _valueTween != null ) 
				_valueTween.Play();
		}
	}

	public void TogglePause()
	{
		if ( _paused ) Unpause();
		else Pause();
	}

	public bool IsPaused()
	{
		return _paused;
	}

	public bool IsUnpaused()
	{
		return !_paused;
	}

	#endregion

}

[RequireComponent( typeof( RectTransform ) )]
[System.Serializable]
class EnergyBarQuadrant : MonoBehaviour
{
	#region Variables

	internal EnergyBar _energyBar;
	internal EnergyBar.BarType _barType;

	internal Image _image;

	internal RectTransform _transform;
	internal RectTransform _imageTransform;

	internal float _alpha;

	#endregion

	#region Unity Events

	internal void Init( EnergyBar energyBar )
	{
		_energyBar = energyBar;

		_transform = gameObject.GetComponent<RectTransform>();
		_transform.sizeDelta = Vector2.zero;
		_transform.localPosition = new Vector3( -_energyBar.barSize.x * 0.5f, 0.0f, 0.0f );

		_image = gameObject.GetComponentInChildren<Image>();
		_imageTransform = _image.GetComponent<RectTransform>();

		_alpha = _image.color.a;

		hideFlags = HideFlags.HideInInspector;
	}

	private void OnDestroy()
	{
		_energyBar = null;
		_transform = null;
		_imageTransform = null;
		_image = null;
	}

	private void OnEnable()
	{
		if ( _image != null )
			_image.enabled = true;
	}

	private void OnDisable()
	{
		if ( _image != null )
			_image.enabled = false;
	}

	static public EnergyBarQuadrant Create( EnergyBar parent )
	{
		GameObject quadrantObject = new GameObject( "EnergyBarQuadrant" );
		quadrantObject.transform.parent = parent.transform;
		quadrantObject.transform.localPosition = Vector3.zero;
		quadrantObject.transform.localScale = Vector3.one;

		GameObject imageObject = new GameObject( "Bar" );
		imageObject.AddComponent<Image>();
		imageObject.transform.SetParent( quadrantObject.transform );
		imageObject.transform.localPosition = Vector3.zero;
		imageObject.transform.localScale = Vector3.one;

		EnergyBarQuadrant quadrant = quadrantObject.AddComponent<EnergyBarQuadrant>();
		quadrant.Init( parent );
		quadrant.SetSize( 100.0f, 20.0f );
		quadrant.SetBarType( EnergyBar.BarType.LeftToRight );
		quadrant.SetColor( Color.white );

		return quadrant;
	}

	public EnergyBarQuadrant Clone( EnergyBar parent )
	{
		EnergyBarQuadrant clone = Create( parent );

		clone.SetSize( _imageTransform.rect.size );
		clone.SetBarType( _barType );
		clone.SetColor( _image.color );

		return clone;
	}

	#endregion

	#region General

	public float value
	{
		get { return _transform.localScale.x; }
		set { _transform.localScale = new Vector3( value, _transform.localScale.y, _transform.localScale.z ); }
	}

	public void SetSize( Vector2 size )
	{
		SetSize( size.x, size.y );
	}

	public void SetSize( float width, float height )
	{
		_imageTransform.sizeDelta = new Vector2( width, height );
	}

	public void SetBarType( EnergyBar.BarType type )
	{
		if ( type == EnergyBar.BarType.LeftToRight )
		{
			_imageTransform.anchorMin = new Vector2( 0.0f, _imageTransform.anchorMin.y );
			_imageTransform.anchorMax = new Vector2( 0.0f, _imageTransform.anchorMax.y );
			_imageTransform.localPosition = new Vector3( _imageTransform.rect.width * 0.5f, _imageTransform.localPosition.y, _imageTransform.localPosition.z );
		}
		else if ( type == EnergyBar.BarType.RightToLeft )
		{
			_imageTransform.anchorMin = new Vector2( 1.0f, _imageTransform.anchorMin.y );
			_imageTransform.anchorMax = new Vector2( 1.0f, _imageTransform.anchorMax.y );
			_imageTransform.localPosition = new Vector3( -_imageTransform.rect.width * 0.5f, _imageTransform.localPosition.y, _imageTransform.localPosition.z );
		}

		_barType = type;
	}

	public void SetColor( Color color )
	{
		_image.color = color;
	}

	#endregion

	#region Animations

	public void Flash( Color color, float duration, float interval )
	{
		if ( _image != null )
		{
			int amountFlashes = (int)(duration / (interval * 2.0f));
			if ( amountFlashes > 0 )
			{
				Color startingColor = _image.color;
				Sequence flash = DOTween.Sequence();

				for ( int j = 0; j < amountFlashes; j++ )
				{
					flash.Append( _image.DOColor( color, interval ) );
					flash.Append( _image.DOColor( startingColor, interval ) );
				}

				flash.Play();
			}
		}
	}

	public void FadeIn( float duration, float delay = 0.0f )
	{
		if ( _image != null )
		{
			_image.DOKill();
			_image.DOFade( _alpha, duration ).SetDelay( delay );
		}
	}

	public void FadeOut( float duration, float delay = 0.0f )
	{
		if ( _image != null )
		{
			_image.DOKill();
			_image.DOFade( 0.0f, duration ).SetDelay( delay );
		}
	}

	public void FadeInOut( float inDuration, float outDuration, float inDelay = 0.0f, float outDelay = 0.0f )
	{
		if ( _image != null )
		{
			_image.DOKill();
			_image.DOFade( _alpha, inDuration ).SetDelay( inDelay );
			_image.DOFade( 0.0f, outDuration ).SetDelay( outDelay );
		}
	}

	#endregion

}
