using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour, IPauseable
{
	#region Variables

	[Header( "Level" )]

	protected bool _queued;
	protected bool _started;
	protected bool _completed;

	[Header( "Pausing" )]

	protected bool _paused;

	#endregion

	#region Level

	public void QueueLevel()
	{
		if ( !_queued )
		{
			_queued = true;
			OnQueueLevel();
		}
	}

	public void StartLevel()
	{
		if ( !_started )
		{
			_started = true;
			OnStartLevel();
		}
	}

	public void StopLevel()
	{
		if ( _started )
		{
			OnStopLevel();
			_started = false;
		}
	}

	public void CompleteLevel()
	{
		if ( !_completed )
		{
			_completed = true;
			OnCompleteLevel();
		}
	}

	public void CheckLevelProgress()
	{
		if ( OnCheckLevelProgress() )
			CompleteLevel();
	}

	public bool IsStarted()
	{
		return _started;
	}

	public bool IsNotStarted()
	{
		return !_started;
	}

	public bool IsCompleted()
	{
		return _completed;
	}

	public bool IsNotCompleted()
	{
		return !_completed;
	}

	virtual protected void OnQueueLevel()
	{
		// override, if necessary

		if ( Game.instance.player != null )
			Game.instance.player.Spawn( Vector3.zero );
	}

	virtual protected void OnStartLevel()
	{
		// override, if necessary
	}

	virtual protected void OnStopLevel()
	{
		// override, if necessary
	}

	virtual protected void OnCompleteLevel()
	{
		// override, if necessary
	}

	virtual protected bool OnCheckLevelProgress()
	{
		// override, if necessary

		return false;
	}

	#endregion

	#region Pausing

	public void Pause()
	{
		if ( !_paused )
		{
			_paused = true;
			OnPause();
		}
	}

	public void Unpause()
	{
		if ( _paused )
		{
			OnUnpause();
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

	virtual protected void OnPause()
	{
		// override, if necessary
	}

	virtual protected void OnUnpause()
	{
		// override, if necessary
	}

	#endregion

}
