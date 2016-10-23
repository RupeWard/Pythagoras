﻿using UnityEngine;
using System.Collections;

/*
	Defines an atrea within which geometric elements appear
*/
public class Field : MonoBehaviour, RJWard.Core.IDebugDescribable
{
	// TODO: Add a scaling to make definition of elements simpler (thinking about different screen sizes here)

	public static readonly bool DEBUG_FIELD = true;

	#region inspector data

	public float depth = 0f; // position in world space

	private Rect viewRect_ = new Rect( 0f, 0f, 1f, 1f );	// defines the field extent relative to screen size
	private Rect rect_ = new Rect( );						// field extent in world coords

	public Rect rect
	{
		get { return rect_; }
	}

	#endregion inspector data

	#region private hooks

	private Transform cachedTransform_ = null;
	public Transform cachedTransform
	{
		get { return cachedTransform_; }
	}

	#endregion private hooks

	#region Flow

	private void Init()
	{
		transform.position = new Vector3( 0f, 0f, depth );

		float left = Camera.main.pixelWidth * viewRect_.xMin;
		float right = Camera.main.pixelWidth * viewRect_.xMax;
		float bottom = Camera.main.pixelHeight * viewRect_.yMin;
		float top = Camera.main.pixelHeight * viewRect_.yMax;

		Vector3 worldTopRight = Camera.main.ScreenToWorldPoint( new Vector3( right, top, 0f ) );
		Vector3 worldBottomLeft = Camera.main.ScreenToWorldPoint( new Vector3( left, bottom, 0f ) );

		rect_.xMin = worldBottomLeft.x;
		rect_.yMin = worldBottomLeft.y;
		rect_.xMax = worldTopRight.x;
		rect_.yMax = worldTopRight.y;

		if (DEBUG_FIELD)
		{
			Debug.Log( "Init(): "+this.DebugDescribe());
		}
	}

	private void Awake()
	{
		cachedTransform_ = transform;
		Init( );
	}

	#endregion Flow

	#region IDebugDescribable

	public void DebugDescribe( System.Text.StringBuilder sb )
	{
		sb.Append( "Field '" ).Append( gameObject.name ).Append( "':" );
		sb.Append( " viewRect=" ).Append( viewRect_.ToString( ) );
		sb.Append( " rect = " ).Append( rect_.ToString( ) );
		sb.Append( " d=" ).Append( depth );
	}

	#endregion IDebugDescribable


}
