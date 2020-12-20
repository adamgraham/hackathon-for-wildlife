using UnityEngine;
using System;
using System.Collections;

public class ArrayUtils
{
	static public GameObject GetRandomGameObjectFromArray( GameObject[] objects )
	{
		GameObject gameObject = null;

		if ( objects != null )
		{
			int len = objects.Length;
			if ( len > 0 ) gameObject = objects[UnityEngine.Random.Range( 0, len )];
		}

		return gameObject;
	}

	static public int IndexOf<T>( T[] array, T value, uint startIndex = 0 ) where T : IComparable 
	{
		int index = -1;

		if ( array != null )
		{
			int len = array.Length;
			for ( int i = (int)startIndex; i < len; i++ )
			{
				if ( value.CompareTo( array[i] ) == 0 )
				{
					index = i;
					break;
				}
			}
		}

		return index;
	}

	static public bool Contains( GameObject[] array, GameObject obj )
	{
		bool contains = false;

		if ( array != null )
		{
			int len = array.Length;
			for ( int i = 0; i < len; i++ )
			{
				GameObject target = array[i];
				if ( target != null )
				{
					if ( target == obj )
					{
						contains = true;
						break;
					}
				}
			}
		}

		return contains;
	}

	static public bool ContainsNonNull( ArrayList array, int startIndex = 0, int endIndex = int.MaxValue )
	{
		bool contains = false;

		if ( array != null )
		{
			int len = Mathf.Min( array.Count, endIndex );
			for ( int i = startIndex; i < len; i++ )
			{
				if ( array[i] != null )
				{
					contains = true;
					break;
				}
			}
		}

		return contains;
	}

	static public void NullArrayList( ArrayList array, int capacity )
	{
		array.Clear();
		array.Capacity = capacity;

		for ( int i = 0; i < capacity; i++ )
			array.Add( null );
	}

}
