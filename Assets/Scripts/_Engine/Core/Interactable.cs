using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using HighlightingSystem;

public class Interactable : MonoBehaviour
{
	public enum InteractionType { Touch, Gather, Attack, Unknown }

	#region Variables

	[Header( "Interaction Settings" )]

	public string[] interacteeTags = new string[] { "Player" };
	public string[] interactInputs = new string[] { "Interact" };

	public bool inputRequired = true;
	public bool oneTimeInteraction = false;
	public bool mouseEnabled = false;

	public delegate void InteractionCallback();
	public InteractionCallback onInteract;

	protected Collider _trigger;
	protected GameObject _interactee;

	protected bool _interacted;
	protected bool _inRange;

	[Header( "Cooldown" )]

	public float cooldown = 1.0f;

	protected bool _cooldown;

	[Header( "Audio" )]

	public AudioClipExtended[] interactSFX;

	[Header( "Highlighting" )]

	public bool highlightingEnabled;
	public GameObject highlightObject;
	public Color highlightColor = new Color( 1.0f, 1.0f, 0.75f );

	private Highlighter _highlighter;

	[Header( "Text Prompt" )]

	public bool promptEnabled = true;
	public string promptString = "Press {control} to interact.";

	static public Text textPrompt;

	#endregion

	#region Unity Events

	private void Awake()
	{
		OnAwake();
	}

	private void Start()
	{
		AddHighlighter();
		PhysicsUtils.AddTriggersFromColliders( gameObject );
		OnStart();
	}

	private void OnDestroy()
	{
		HideEffects();
		OnDispose();

		interacteeTags = null;
		interactInputs = null;
		interactSFX = null;
		onInteract = null;
		highlightObject = null;

		_trigger = null;
		_interactee = null;
		_highlighter = null;
	}

	private void OnEnable()
	{
		OnEnabled();
	}

	private void OnDisable()
	{
		_inRange = false;
		_trigger = null;

		HideEffects();
		OnDisabled();
	}

	private void OnTriggerEnter( Collider other )
	{
		if ( CanInteract( other ) )
		{
			_trigger = other;
			_inRange = true;

			if ( !inputRequired )
				Interact( _trigger );

			ShowEffects();
		}
	}

	private void OnTriggerExit( Collider other )
	{
		if ( other == _trigger )
		{
			_trigger = null;
			_inRange = false;

			HideEffects();
		}
	}

	private void OnMouseDown()
	{
		if ( mouseEnabled )
			if ( _inRange )
				Interact( _trigger );
	}

	virtual protected void OnAwake()
	{
		// override, if necessary
	}

	virtual protected void OnStart()
	{
		// override, if necessary
	}

	virtual protected void OnDispose()
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

	#endregion

	#region Update

	private void Update()
	{
		OnUpdate();

		if ( _inRange )
		{
			int len = interactInputs.Length;
			for ( int i = 0; i < len; i++ )
			{
				if ( Input.GetButtonDown( interactInputs[i] ) )
				{
					Interact( _trigger );
					break;
				}
			}
		}
	}

	virtual protected void OnUpdate()
	{
		// override, if necessary
	}

	#endregion

	#region Interaction

	public void Interact( GameObject interactee, InteractionType interaction = InteractionType.Unknown )
	{
		if ( isActiveAndEnabled )
		{
			if ( !_cooldown )
			{
				if ( !oneTimeInteraction || (oneTimeInteraction && !_interacted) )
				{
					if ( CanInteract( interactee ) )
					{
						if ( interactee.activeInHierarchy )
						{
							_interactee = interactee;
							_interacted = true;

							PlayInteractSoundEffect();
							OnInteract( interactee.gameObject, (interaction != InteractionType.Unknown) ? interaction : GetDefaultInteractionType() );
							StartCooldown();
						}
					}
				}
			}
		}
	}

	public void Interact( Collider interactee, InteractionType interaction = InteractionType.Unknown )
	{
		if ( isActiveAndEnabled )
		{
			if ( !_cooldown )
			{
				if ( !oneTimeInteraction || (oneTimeInteraction && !_interacted) )
				{
					if ( CanInteract( interactee ) )
					{
						if ( interactee.enabled && interactee.gameObject.activeInHierarchy )
						{
							_interactee = interactee.gameObject;
							_interacted = true;

							PlayInteractSoundEffect();
							OnInteract( interactee.gameObject, (interaction != InteractionType.Unknown) ? interaction : GetDefaultInteractionType() );
							StartCooldown();
						}
					}
				}
			}
		}
	}

	virtual protected void OnInteract( GameObject interactee = null, InteractionType interaction = InteractionType.Unknown )
	{
		// override
	}

	public void ClearInteraction( bool clearTriggerData = false )
	{
		_interactee = null;
		_interacted = false;

		if ( clearTriggerData )
		{
			_trigger = null;
			_inRange = false;
		}

		OnClearInteraction();
	}

	virtual protected void OnClearInteraction()
	{
		// override
	}

	private void PlayInteractSoundEffect()
	{
		if ( interactSFX != null )
		{
			int len = interactSFX.Length;
			for ( int i = 0; i < len; i++ )
				interactSFX[i].Play( transform.position );
		}
	}

	public bool CanInteract( GameObject interactee )
	{
		bool canInteract = false;

		if ( interactee != null && interacteeTags != null )
		{
			int len = interacteeTags.Length;
			for ( int i = 0; i < len; i++ )
			{
				if ( interacteeTags[i] == interactee.tag )
				{
					canInteract = true;
					break;
				}
			}
		}

		return canInteract;
	}

	public bool CanInteract( Collider interactee )
	{
		if ( interactee != null )
			return CanInteract( interactee.gameObject );

		return false;
	}

	public InteractionType GetDefaultInteractionType()
	{
		return InteractionType.Touch;
	}

	public GameObject GetInteractee()
	{
		return (_interactee != null) ? _interactee.gameObject : null;
	}

	#endregion

	#region Cooldown

	public void StartCooldown()
	{
		StartCooldown( cooldown );
	}

	public void StartCooldown( float time )
	{
		_cooldown = true;

		CancelInvoke( "StopCooldown" );
		Invoke( "StopCooldown", time );
	}

	public void StopCooldown()
	{
		CancelInvoke( "StopCooldown" );

		_cooldown = false;
	}

	public bool IsCooldown()
	{
		return _cooldown;
	}

	public bool IsNotCooldown()
	{
		return !_cooldown;
	}

	#endregion

	#region Effects

	public void ShowEffects()
	{
		if ( enabled )
		{
			if ( promptEnabled ) ShowTextPrompt();
			if ( highlightingEnabled ) ShowHighlight();
		}
	}

	public void HideEffects()
	{
		HideTextPrompt();
		HideHighlight();
	}

	#region Text Prompt

	public void ShowTextPrompt()
	{
		if ( textPrompt != null )
		{
			textPrompt.text = promptString;
			textPrompt.text = textPrompt.text.Replace( "{control}", GetControlString() );

			textPrompt.DOKill();
			textPrompt.DOFade( 1.0f, 0.5f );
		}
	}

	public void HideTextPrompt()
	{
		if ( textPrompt != null )
		{
			textPrompt.DOKill();
			textPrompt.DOFade( 0.0f, 0.5f );
		}
	}

	private string GetControlString()
	{
		// TO DO...
		return "Space";
	}

	#endregion

	#region Highlighting

	public void ShowHighlight()
	{
		if ( _highlighter != null )
		{
			_highlighter.SeeThroughOff();
			_highlighter.ConstantOn( highlightColor );
		}
	}

	public void HideHighlight()
	{
		if ( _highlighter != null )
			_highlighter.ConstantOff();
	}

	private void AddHighlighter()
	{
		if ( highlightObject == null )
			highlightObject = gameObject;

		_highlighter = highlightObject.AddComponent<Highlighter>();
	}

	#endregion

	#endregion

}
