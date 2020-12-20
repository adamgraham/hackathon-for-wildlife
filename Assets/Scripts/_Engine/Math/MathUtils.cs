using UnityEngine;
using System.Collections;

public class MathUtils
{
	#region Variables

	public const float EPSILON = float.Epsilon;
	public const float EPSILON_NEGATIVE = -float.Epsilon;
	public const float DIAGONAL_SPEED = 1.0f / 1.41421356237f; // 1.0f / Mathf.sqrt(2);

	#endregion

	#region Misc

	static public float CalculateDiagonalMultiplier( float x, float z )
	{
		return (IsNotZero( x ) && IsNotZero( z )) ? DIAGONAL_SPEED : 1.0f;
	}

	#endregion

	#region Clamping

	static public Vector3 ClampVector( Vector3 value, Vector3 min, Vector3 max )
	{
		value.x = Mathf.Clamp( value.x, min.x, max.x );
		value.y = Mathf.Clamp( value.y, min.y, max.y );
		value.z = Mathf.Clamp( value.z, min.z, max.z );

		return value;
	}

	#endregion

	#region Rounding

	static public Vector3 FloorVector( Vector3 vector )
	{
		vector.x = Mathf.Floor( vector.x );
		vector.y = Mathf.Floor( vector.y );
		vector.z = Mathf.Floor( vector.z );

		return vector;
	}

	static public Vector3 RoundVector( Vector3 vector )
	{
		vector.x = Mathf.Round( vector.x );
		vector.y = Mathf.Round( vector.y );
		vector.z = Mathf.Round( vector.z );

		return vector;
	}

	static public Vector3 CeilVector( Vector3 vector )
	{
		vector.x = Mathf.Ceil( vector.x );
		vector.y = Mathf.Ceil( vector.y );
		vector.z = Mathf.Ceil( vector.z );

		return vector;
	}

	#endregion

	#region IsZero

	static public bool IsZero( float value )
	{
		return (value > EPSILON_NEGATIVE) && (value < EPSILON);
	}

	static public bool IsZero( float value, float epsilon )
	{
		return (value > -epsilon) && (value < epsilon);
	}

	static public bool IsZero( Vector3 vector )
	{
		return IsZero( vector.x ) && IsZero( vector.y ) && IsZero( vector.z );
	}

	static public bool IsZero( Vector3 vector, float epsilon )
	{
		return IsZero( vector.x, epsilon ) && IsZero( vector.y, epsilon ) && IsZero( vector.z, epsilon );
	}

	#endregion

	#region IsNotZero

	static public bool IsNotZero( float value )
	{
		return !IsZero( value );
	}

	static public bool IsNotZero( float value, float epsilon )
	{
		return !IsZero( value, epsilon );
	}

	static public bool IsNotZero( Vector3 vector )
	{
		return !IsZero( vector );
	}

	static public bool IsNotZero( Vector3 vector, float epsilon )
	{
		return !IsZero( vector, epsilon );
	}

	#endregion

	#region IsEqual

	static public bool IsEqual( float a, float b )
	{
		return Mathf.Abs( a - b ) < EPSILON;
	}

	static public bool IsEqual( float a, float b, float epsilon )
	{
		return Mathf.Abs( a - b ) < epsilon;
	}

	static public bool IsEqual( Vector3 a, Vector3 b )
	{
		return IsEqual( a.x, b.x ) && IsEqual( a.y, b.y ) && IsEqual( a.z, b.z );
	}

	static public bool IsEqual( Vector3 a, Vector3 b, float epsilon )
	{
		return IsEqual( a.x, b.x, epsilon ) && IsEqual( a.y, b.y, epsilon ) && IsEqual( a.z, b.z, epsilon );
	}

	#endregion

	#region IsNotEqual

	static public bool IsNotEqual( float a, float b )
	{
		return !IsEqual( a, b );
	}

	static public bool IsNotEqual( float a, float b, float epsilon )
	{
		return !IsEqual( a, b, epsilon );
	}

	static public bool IsNotEqual( Vector3 a, Vector3 b )
	{
		return !IsEqual( a, b );
	}

	static public bool IsNotEqual( Vector3 a, Vector3 b, float epsilon )
	{
		return !IsEqual( a, b, epsilon );
	}

	#endregion

}
