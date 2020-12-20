using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent( typeof( Canvas ) )]
public class HUD : MonoBehaviour
{
	#region Variables

	static private HUD _instance;
	static public HUD instance
	{
		get
		{
			if ( _instance == null )
			{
				GameObject scriptGameObj = new GameObject();
				scriptGameObj.name = "HUD";

				_instance = scriptGameObj.AddComponent<HUD>();
			}

			return _instance;
		}
	}

	[Header( "General Settings" )]

	public string showHideControl;

	[Header( "Text" )]

	public Text interactionPrompt;

	#endregion

	#region Unity Events

	private void Awake()
	{
		if ( _instance == null )
			_instance = this;

		if ( interactionPrompt != null )
			Interactable.textPrompt = interactionPrompt;

		OnAwake();
	}

	private void Start()
	{
		HideAll();
		ShowAll();
		OnStart();
	}

	private void OnDestroy()
	{
		if ( _instance == this )
			_instance = null;

		showHideControl = null;
		interactionPrompt = null;

		OnDispose();
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

	#endregion

	#region Show HUD

	public void ShowAll()
	{
		if ( interactionPrompt != null )
			interactionPrompt.gameObject.SetActive( true );
	}

	#endregion

	#region Hide HUD

	public void HideAll()
	{
		if ( interactionPrompt != null )
		{
			interactionPrompt.text = "";
			interactionPrompt.gameObject.SetActive( false );
		}
	}

	#endregion

}
