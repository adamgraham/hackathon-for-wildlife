using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PausableInvoke : MonoBehaviour 
{
	static private PausableInvoke _instance;
	static private Hashtable _invokes;

	public delegate void PausableInvokeCallback();

	static private PausableInvoke instance
	{
		get
		{
			PausableInvoke script = _instance;

			if ( script == null )
			{
				GameObject gameObject = new GameObject();
				gameObject.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
				gameObject.name = "PausableInvoke";

				script = gameObject.AddComponent<PausableInvoke>();

				_invokes = new Hashtable();
			}

			return script;
		}
	}

	private void Awake()
	{
		if ( _instance == null )
		{
			_instance = this;
			_instance.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;

			if ( _invokes == null )
				_invokes = new Hashtable();
		}
		else
		{
			DestroyImmediate( this );
		}
	}

	private void OnDestroy()
	{
		if ( _instance == this )
		{
			_instance = null;
			_invokes = null;
		}
	}

	private IEnumerator _Invoke( IPauseable target, PausableInvokeCallback callback, float delay )
	{
		float elapsed = -Mathf.Epsilon;

		while ( elapsed < delay )
		{
			if ( !target.IsPaused() ) elapsed += Time.deltaTime;
			yield return null;
		}

		callback();
		RemoveInvoke( target, callback );
	}

	static public void Invoke( IPauseable target, PausableInvokeCallback callback, float delay )
	{
		if ( target != null )
		{
			Coroutine coroutine = _instance.StartCoroutine( _instance._Invoke( target, callback, delay ) );

			List<PausableInvokeRoutine> targetInvokes = _invokes[target] as List<PausableInvokeRoutine>;
			if ( targetInvokes == null ) targetInvokes = new List<PausableInvokeRoutine>();

			PausableInvokeRoutine routine;
			routine.callback = callback;
			routine.coroutine = coroutine;

			targetInvokes.Add( routine );

			_invokes[target] = targetInvokes;
		}
	}

	static public void CancelInvoke( IPauseable target, PausableInvokeCallback callback )
	{
		if ( target != null )
		{
			List<PausableInvokeRoutine> targetInvokes = _invokes[target] as List<PausableInvokeRoutine>;

			if ( targetInvokes != null )
			{
				bool found = false;

				foreach ( PausableInvokeRoutine routine in targetInvokes )
				{
					if ( routine.callback == callback )
					{
						_instance.StopCoroutine( routine.coroutine );
						found = true;
					}
				}

				if ( found )
					RemoveInvoke( target, callback );
			}
		}
	}

	static private void RemoveInvoke( IPauseable target, PausableInvokeCallback callback )
	{
		if ( target != null )
		{
			List<PausableInvokeRoutine> targetInvokes = _invokes[target] as List<PausableInvokeRoutine>;

			if ( targetInvokes != null )
			{
				List<PausableInvokeRoutine> newList = new List<PausableInvokeRoutine>( targetInvokes );

				foreach ( PausableInvokeRoutine routine in targetInvokes )
				{
					if ( routine.callback == callback )
						newList.Remove( routine );
				}

				targetInvokes.Clear();
				targetInvokes = newList;
			}
		}
	}

}

internal struct PausableInvokeRoutine
{
	public PausableInvoke.PausableInvokeCallback callback;
	public Coroutine coroutine;
}
