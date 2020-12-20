using UnityEngine;
using System.Collections;

public class EyesBlink : MonoBehaviour 
{
	public Renderer eyesRenderer;
	public Material eyesOpenMat;
	public Material eyesClosedMat;

	[Range( 0.0f, 1.0f )]
	public float blinkChance = 0.0035f;
	public float blinkDurationMin = 0.05f;
	public float blinkDurationMax = 0.10f;

	private Material[] _eyesClosed;
	private Material[] _eyesOpened;

	private bool _blinking;

	private void Awake()
	{
		_eyesClosed = new Material[2];
		_eyesClosed[0] = eyesClosedMat;
		_eyesClosed[1] = eyesClosedMat;

		_eyesOpened = new Material[2];
		_eyesOpened[0] = eyesOpenMat;
		_eyesOpened[1] = eyesOpenMat;
	}

	private void FixedUpdate()
	{
		if ( Random.value <= blinkChance )
			Blink();
	}

	public void Blink()
	{
		if ( !_blinking )
		{
			_blinking = true;

			CloseEyes();

			CancelInvoke( "BlinkComplete" );
			Invoke( "BlinkComplete", Random.Range( blinkDurationMin, blinkDurationMax ) );
		}
	}

	public void OpenEyes()
	{
		if ( eyesRenderer != null )
			eyesRenderer.materials = _eyesOpened;
	}

	public void CloseEyes()
	{
		if ( eyesRenderer != null )
			eyesRenderer.materials = _eyesClosed;
	}

	private void BlinkComplete()
	{
		_blinking = false;

		OpenEyes();
	}

}
