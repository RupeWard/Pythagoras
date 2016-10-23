using UnityEngine;
using System.Collections;

public class Field : MonoBehaviour
{
	public static readonly bool DEBUG_FIELD = true;

	#region inspector data

	public float depth = 0f;

	private Rect viewRect_ = new Rect( 0f, 0f, 1f, 1f );
	private Rect rect_ = new Rect( );

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
			Debug.Log( "Field.Init(): viewRect = " + viewRect_.ToString( ) + ", rect = " + rect_.ToString( ) );
		}
	}

	private void Awake()
	{
		Init( );
	}

	private void Start ()
	{
	}

	private void Update ()
	{
	
	}

	#endregion Flow

}
