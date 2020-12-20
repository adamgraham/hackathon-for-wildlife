using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour, IPauseable
{
	#region Variables

	[Header( "References" )]

	public Level level;
	public Player player;

	[Header( "Pausing" )]

	private bool _paused;

	[Header( "Instance" )]

	static private Game _instance;
	static private bool _creating;

	static public Game instance
	{
		get
		{
			Game __instance = _instance;

			if ( __instance == null )
			{
				if ( !_creating )
				{
					_creating = true;

					GameObject obj = new GameObject();
					obj.name = "Game";

					__instance = obj.AddComponent<Game>();
				}
			}

			return __instance;
		}
	}

	#endregion

	#region Unity Events

	private void Awake()
	{
		if ( _instance == null )
			_instance = this;
		else
			DestroyImmediate( this );

		_creating = false;

		ScreenFader.FadeToBlack( 0.0f );
	}

	private void Start()
	{
		if ( player == null )
			player = FindPlayerObject();

		if ( level == null )
			level = FindLevelObject();

		ScreenFader.FadeFromBlack();
		QueueLevel();
	}

	private void OnDestroy()
	{
		if ( _instance == this )
			_instance = null;
	}

	private Player FindPlayerObject()
	{
		Player mPlayer = GameObject.FindObjectOfType<Player>();
		
		if ( mPlayer == null )
		{
			GameObject mTagged = GameObject.FindGameObjectWithTag( "Player" );
			
			if ( mTagged != null )
				mPlayer = mTagged.GetComponent<Player>();
		}

		if ( mPlayer == null )
		{
			Transform mRoot = transform.root;
			
			if ( mRoot != null )
				mPlayer = mRoot.GetComponentInChildren<Player>();
		}

		return mPlayer;
	}

	private Level FindLevelObject()
	{
		Level mLevel = GameObject.FindObjectOfType<Level>();

		if ( mLevel == null )
		{
			GameObject mTagged = GameObject.FindGameObjectWithTag( "Level" );

			if ( mTagged != null )
				mLevel = mTagged.GetComponent<Level>();
		}

		if ( mLevel == null )
		{
			Transform mRoot = transform.root;

			if ( mRoot != null )
				mLevel = mRoot.GetComponentInChildren<Level>();
		}

		return mLevel;
	}

	#endregion

	#region Level

	private void QueueLevel()
	{
		if ( level != null )
		{
			level.QueueLevel();
			Invoke( "StartLevel", 1.0f );
		}
		else if ( player != null )
		{
			player.Spawn( Vector3.zero );
		}
	}

	private void StartLevel()
	{
		if ( level != null )
			level.StartLevel();
	}

	public bool IsLevelActive()
	{
		return level != null && level.IsStarted();
	}

	#endregion

	#region Pausing

	public void Pause()
	{
		if ( !_paused )
		{
			_paused = true;

			if ( level != null )
				level.Pause();

			if ( player != null )
				player.Pause();
		}
	}

	public void Unpause()
	{
		if ( _paused )
		{
			if ( level != null )
				level.Unpause();

			if ( player != null )
				player.Unpause();

			_paused = false;
		}
	}

	public void TogglePause()
	{
		if ( _paused )
			Unpause();
		else
			Pause();
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
