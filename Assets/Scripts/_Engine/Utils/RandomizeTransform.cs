using UnityEngine;
using System.Collections;

public class RandomizeTransform : MonoBehaviour
{
	public RandomRotation[] rotations;
	public RandomScale[] scales;
	public RandomTranslation[] translations;

	[HideInInspector]
	public Transform defaultTransform;
	[HideInInspector]
	public bool defaultToBaseTransform;

	public bool destroyScriptAfter;

	private void Start()
	{
		if ( defaultTransform == null )
			defaultTransform = transform;

		RandomizeRotation();
		RandomizeScale();
		RandomizeTranslation();

		Invoke( "OnTransformationComplete", 1.0f );
	}

	private void OnDestroy()
	{
		if ( rotations != null )
		{
			int len = rotations.Length;
			for ( int i = 0; i < len; i++ )
				rotations[i].Dispose();

			rotations = null;
		}

		if ( scales != null )
		{
			int len = scales.Length;
			for ( int i = 0; i < len; i++ )
				scales[i].Dispose();

			scales = null;
		}

		if ( translations != null )
		{
			int len = translations.Length;
			for ( int i = 0; i < len; i++ )
				translations[i].Dispose();

			translations = null;
		}
	}

	private void OnTransformationComplete()
	{
		if ( destroyScriptAfter )
			Destroy( this );
	}

	private void RandomizeRotation()
	{
		int len = rotations.Length;
		for ( int i = 0; i < len; i++ )
		{
			RandomRotation rotationRandomization = rotations[i];

			if ( defaultToBaseTransform )
				rotationRandomization.transform = defaultTransform;

			if ( rotationRandomization.transform != null )
			{
				Vector3 rotation = rotationRandomization.transform.localEulerAngles;
				float rotationValue = Random.Range( rotationRandomization.min, rotationRandomization.max );
				Axis axis = rotationRandomization.axis;

				// x
				if ( axis == Axis.X || axis == Axis.XZ || axis == Axis.XYZ ) rotation.x = rotationValue;

				// y
				if ( !rotationRandomization.uniform ) rotationValue = Random.Range( rotationRandomization.min, rotationRandomization.max );
				if ( axis == Axis.Y || axis == Axis.YZ || axis == Axis.XYZ ) rotation.y = rotationValue;

				// z
				if ( !rotationRandomization.uniform ) rotationValue = Random.Range( rotationRandomization.min, rotationRandomization.max );
				if ( axis == Axis.Z || axis == Axis.XZ || axis == Axis.XYZ ) rotation.z = rotationValue;

				rotationRandomization.transform.localEulerAngles = rotation;
			}
		}
	}

	private void RandomizeScale()
	{
		int len = scales.Length;
		for ( int i = 0; i < len; i++ )
		{
			RandomScale scaleRandomization = scales[i];

			if ( defaultToBaseTransform )
				scaleRandomization.transform = defaultTransform;

			if ( scaleRandomization.transform != null )
			{
				Vector3 scale = scaleRandomization.transform.localScale;
				float scaleValue = Random.Range( scaleRandomization.min, scaleRandomization.max );
				Axis axis = scaleRandomization.axis;

				// x
				if ( axis == Axis.X || axis == Axis.XZ || axis == Axis.XYZ ) scale.x *= scaleValue;

				// y
				if ( !scaleRandomization.uniform ) scaleValue = Random.Range( scaleRandomization.min, scaleRandomization.max );
				if ( axis == Axis.Y || axis == Axis.YZ || axis == Axis.XYZ ) scale.y *= scaleValue;

				// z
				if ( !scaleRandomization.uniform ) scaleValue = Random.Range( scaleRandomization.min, scaleRandomization.max );
				if ( axis == Axis.Z || axis == Axis.XZ || axis == Axis.XYZ ) scale.z *= scaleValue;

				scaleRandomization.transform.localScale = scale;
			}
		}
	}

	private void RandomizeTranslation()
	{
		int len = translations.Length;
		for ( int i = 0; i < len; i++ )
		{
			RandomTranslation translationRandomization = translations[i];

			if ( defaultToBaseTransform )
				translationRandomization.transform = defaultTransform;

			if ( translationRandomization.transform != null )
			{
				Vector3 translation = new Vector3();
				float translationValue = Random.Range( translationRandomization.min, translationRandomization.max );
				Axis axis = translationRandomization.axis;

				// x
				if ( axis == Axis.X || axis == Axis.XZ || axis == Axis.XYZ ) translation.x += translationValue;

				// y
				if ( !translationRandomization.uniform ) translationValue = Random.Range( translationRandomization.min, translationRandomization.max );
				if ( axis == Axis.Y || axis == Axis.YZ || axis == Axis.XYZ ) translation.y += translationValue;

				// z
				if ( !translationRandomization.uniform ) translationValue = Random.Range( translationRandomization.min, translationRandomization.max );
				if ( axis == Axis.Z || axis == Axis.XZ || axis == Axis.XYZ ) translation.z += translationValue;

				Rigidbody rigidBody = translationRandomization.transform.gameObject.GetComponent<Rigidbody>();

				if ( rigidBody != null )
				{
					if ( rigidBody.isKinematic )
					{
						// the object needs to be non-kinematic such that it can be pushed out of place if overlapping with another object
						// however, it needs to only be non-kinematic for atleast one frame so Unity's physics code runs and moves the object

						rigidBody.isKinematic = false;
						StartCoroutine( ReapplyKinematicRigidbody( rigidBody ) );
					}

					translationRandomization.transform.Translate( translation );
				}
				else
				{
					translationRandomization.transform.position = translationRandomization.transform.position + translation;
				}
			}
		}
	}

	private IEnumerator ReapplyKinematicRigidbody( Rigidbody rigidBody )
	{
		float elapsed = -float.Epsilon;
		float duration = float.Epsilon;

		while ( elapsed <= duration )
		{
			elapsed += Time.deltaTime;
			yield return null;
		}

		if ( rigidBody != null )
			rigidBody.isKinematic = true;
	}

}

[System.Serializable]
public class RandomRotation
{
	public Transform transform;
	public Axis axis;
	public bool uniform;

	[Range( -360.0f, 360.0f )]
	public float min;
	[Range( -360.0f, 360.0f )]
	public float max;

	public void Dispose()
	{
		transform = null;
	}

	public RandomRotation Clone()
	{
		RandomRotation clone = new RandomRotation();
		clone.transform = transform;
		clone.axis = axis;
		clone.uniform = uniform;
		clone.min = min;
		clone.max = max;
		return clone;
	}

	static public RandomRotation[] CloneArray( RandomRotation[] sources )
	{
		int len = sources.Length;
		RandomRotation[] clones = new RandomRotation[len];

		for ( int i = 0; i < len; i++ )
			clones[i] = sources[i].Clone();

		return clones;
	}

}

[System.Serializable]
public class RandomScale
{
	public Transform transform;
	public Axis axis;
	public bool uniform;

	public float min = 1.0f;
	public float max = 1.0f;

	public void Dispose()
	{
		transform = null;
	}

	public RandomScale Clone()
	{
		RandomScale clone = new RandomScale();
		clone.transform = transform;
		clone.axis = axis;
		clone.uniform = uniform;
		clone.min = min;
		clone.max = max;
		return clone;
	}

	static public RandomScale[] CloneArray( RandomScale[] sources )
	{
		int len = sources.Length;
		RandomScale[] clones = new RandomScale[len];

		for ( int i = 0; i < len; i++ )
			clones[i] = sources[i].Clone();

		return clones;
	}

}

[System.Serializable]
public class RandomTranslation
{
	public Transform transform;
	public Axis axis;
	public bool uniform;

	public float min = 0.0f;
	public float max = 0.0f;

	public void Dispose()
	{
		transform = null;
	}

	public RandomTranslation Clone()
	{
		RandomTranslation clone = new RandomTranslation();
		clone.transform = transform;
		clone.axis = axis;
		clone.uniform = uniform;
		clone.min = min;
		clone.max = max;
		return clone;
	}

	static public RandomTranslation[] CloneArray( RandomTranslation[] sources )
	{
		int len = sources.Length;
		RandomTranslation[] clones = new RandomTranslation[len];

		for ( int i = 0; i < len; i++ )
			clones[i] = sources[i].Clone();

		return clones;
	}

}
