using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour 
{
	public float duration;
	public int tickCount;

	public delegate void TimerCallback();
	public TimerCallback onTick;
	public TimerCallback onComplete;

	private float _elapsed;
	private float _elapsedSinceStart;
	private int _currentCount;
	private bool _running;

	public void Destroy()
	{
		Destroy( this );
	}

	private void OnDestroy()
	{
		onTick = null;
		onComplete = null;
	}

	private void Update()
	{
		if ( _running )
		{
			_elapsed += Time.deltaTime;
			_elapsedSinceStart += Time.deltaTime;

			if ( _elapsed >= duration ) 
				Tick();
		}
	}

	public void StartTimer()
	{
		if ( !_running )
		{
			_elapsedSinceStart = 0.0f;

			if ( _currentCount < tickCount || tickCount <= 0 )
				_running = true;
		}
	}

	public void StopTimer()
	{
		_running = false;
	}

	public void ResetTimer( bool restart = false )
	{
		_elapsed = 0.0f;
		_elapsedSinceStart = 0.0f;
		_currentCount = 0;

		if ( restart ) 
			StartTimer();
	}

	public bool IsRunning()
	{
		return _running;
	}

	public bool IsComplete()
	{
		return !_running && _currentCount >= tickCount && tickCount > 0;
	}

	private void Tick()
	{
		_currentCount++;

		if ( onTick != null ) onTick();
		if ( _currentCount >= tickCount && tickCount > 0 ) Complete();

		_elapsed = 0.0f;
	}

	private void Complete()
	{
		StopTimer();

		if ( onComplete != null ) 
			onComplete();
	}

	public float elapsed
	{
		get { return _elapsed; }
	}

	public float elapsedSinceStart
	{
		get { return _elapsedSinceStart; }
	}

	public int currentCount
	{
		get { return _currentCount; }
	}

	public bool running
	{
		get { return _running; }
	}

	static public Timer CreateTimer( float duration, int tickCount = 1 )
	{
		GameObject timerGameObj = new GameObject();
		timerGameObj.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;

		Timer timer = timerGameObj.AddComponent<Timer>();
		timer.hideFlags = HideFlags.HideInInspector;
		timer.duration = duration;
		timer.tickCount = tickCount;

		return timer;
	}

	static public Timer CreateTimer( GameObject obj, float duration, int tickCount = 1 )
	{
		Timer timer = obj.AddComponent<Timer>();
		timer.hideFlags = HideFlags.HideInInspector;
		timer.duration = duration;
		timer.tickCount = tickCount;

		return timer;
	}

}
