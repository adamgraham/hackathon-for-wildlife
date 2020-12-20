using UnityEngine;
using System.Collections;

[RequireComponent( typeof ( Transform ) )]
public class AnchoredTransform : MonoBehaviour
{
	#region Variables

	public Anchor anchor = Anchor.Center;

	private Vector3 _direction;
	private Vector3 _offset;
	private Vector3 _vector;

	static private Vector3 VECTOR3_ZERO = Vector3.zero;

	#endregion

	#region AttachTo()

	public Vector3 AttachTo( AnchoredTransform otherTransform )
	{
		_vector = otherTransform.GetAnchorPosition() - GetAnchorOffset();
		return Attach();
	}

	public Vector3 AttachTo( AnchoredTransform otherTransform, Vector3 additionalOffset )
	{
		_vector = otherTransform.GetAnchorPosition() - GetAnchorOffset( additionalOffset );
		return Attach();
	}

	public Vector3 AttachTo( AnchoredTransform otherTransform, Anchor otherAnchor )
	{
		_vector = otherTransform.GetAnchorPosition( otherAnchor ) - GetAnchorOffset();
		return Attach();
	}

	public Vector3 AttachTo( AnchoredTransform otherTransform, Anchor otherAnchor, Vector3 additionalOffset )
	{
		_vector = otherTransform.GetAnchorPosition( otherAnchor ) - GetAnchorOffset( additionalOffset );
		return Attach();
	}

	public Vector3 AttachTo( Transform otherTransform )
	{
		_vector = otherTransform.position - GetAnchorOffset();
		return Attach();
	}

	public Vector3 AttachTo( Transform otherTransform, Vector3 additionalOffset )
	{
		_vector = otherTransform.position - GetAnchorOffset( additionalOffset );
		return Attach();
	}

	#endregion

	#region GetAnchorPosition()

	public Vector3 GetAnchorPosition()
	{
		return GetAnchorPosition( anchor, VECTOR3_ZERO );
	}

	public Vector3 GetAnchorPosition( Vector3 additionalOffset )
	{
		return GetAnchorPosition( anchor, additionalOffset );
	}

	public Vector3 GetAnchorPosition( Anchor otherAnchor )
	{
		return GetAnchorPosition( otherAnchor, VECTOR3_ZERO );
	}

	public Vector3 GetAnchorPosition( Anchor otherAnchor, Vector3 additionalOffset )
	{
		CalculateOffset( otherAnchor, additionalOffset );
		return transform.position + _offset;
	}

	#endregion

	#region GetAnchorOffset()

	public Vector3 GetAnchorOffset()
	{
		return GetAnchorOffset( anchor, VECTOR3_ZERO );
	}

	public Vector3 GetAnchorOffset( Vector3 additionalOffset )
	{
		return GetAnchorOffset( anchor, additionalOffset );
	}

	public Vector3 GetAnchorOffset( Anchor otherAnchor )
	{
		return GetAnchorOffset( otherAnchor, VECTOR3_ZERO );
	}

	public Vector3 GetAnchorOffset( Anchor otherAnchor, Vector3 additionalOffset )
	{
		CalculateOffset( otherAnchor, additionalOffset );
		return _offset;
	}

	#endregion

	#region GetAnchorOffsetX()

	public float GetAnchorOffsetX()
	{
		return GetAnchorOffsetX( anchor, VECTOR3_ZERO );
	}

	public float GetAnchorOffsetX( Vector3 additionalOffset )
	{
		return GetAnchorOffsetX( anchor, additionalOffset );
	}

	public float GetAnchorOffsetX( Anchor otherAnchor )
	{
		return GetAnchorOffsetX( otherAnchor, VECTOR3_ZERO );
	}

	public float GetAnchorOffsetX( Anchor otherAnchor, Vector3 additionalOffset )
	{
		CalculateOffset( otherAnchor, additionalOffset );
		return _offset.x;
	}

	#endregion

	#region GetAnchorOffsetY()

	public float GetAnchorOffsetY()
	{
		return GetAnchorOffsetY( anchor, VECTOR3_ZERO );
	}

	public float GetAnchorOffsetY( Vector3 additionalOffset )
	{
		return GetAnchorOffsetY( anchor, additionalOffset );
	}

	public float GetAnchorOffsetY( Anchor otherAnchor )
	{
		return GetAnchorOffsetY( otherAnchor, VECTOR3_ZERO );
	}

	public float GetAnchorOffsetY( Anchor otherAnchor, Vector3 additionalOffset )
	{
		CalculateOffset( otherAnchor, additionalOffset );
		return _offset.y;
	}

	#endregion

	private Vector3 Attach()
	{
		if ( anchor == Anchor.Bottom || anchor == Anchor.Top )
			_vector.x = transform.position.x;

		if ( anchor == Anchor.Left || anchor == Anchor.Right )
			_vector.y = transform.position.y;

		_vector.z = transform.position.z;

		transform.position = _vector;

		return _vector;
	}

	private void CalculateOffset( Anchor otherAnchor, Vector3 additionalOffset )
	{
		//if ( transform.hasChanged )
		//{
			_offset.x = 0.0f;
			_offset.y = 0.0f;
			_offset.z = 0.0f;

			if ( otherAnchor == Anchor.Left || otherAnchor == Anchor.TopLeft || otherAnchor == Anchor.CenterLeft || otherAnchor == Anchor.BottomLeft )
				_offset.x = (-0.5f + additionalOffset.x) * transform.localScale.x * transform.right.x;
			else if ( otherAnchor == Anchor.Right || otherAnchor == Anchor.TopRight || otherAnchor == Anchor.CenterRight || otherAnchor == Anchor.BottomRight )
				_offset.x = (0.5f + additionalOffset.x) * transform.localScale.x * transform.right.x;

			if ( otherAnchor == Anchor.Top || otherAnchor == Anchor.TopLeft || otherAnchor == Anchor.TopCenter || otherAnchor == Anchor.TopRight )
				_offset.y = (0.5f - additionalOffset.y) * transform.localScale.y * transform.up.y;
			else if ( otherAnchor == Anchor.Bottom || otherAnchor == Anchor.BottomLeft || otherAnchor == Anchor.BottomCenter || otherAnchor == Anchor.BottomRight )
				_offset.y = (-0.5f - additionalOffset.y) * transform.localScale.y * transform.up.y;

			//transform.hasChanged = false;
		//}
	}

}
