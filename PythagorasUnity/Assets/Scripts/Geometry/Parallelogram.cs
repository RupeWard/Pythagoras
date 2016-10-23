using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Parallelogram : Element, RJWard.Core.IDebugDescribable
{
	public static readonly bool DEBUG_PARALLELOGRAM = true;

	#region inspector hooks
	#endregion inspector hooks

	#region private hooks

	#endregion private hooks

	#region private data

	private List< Vector2 > baseVertices_ = new List<Vector2>( 2 ) { Vector3.zero, Vector3.zero };
	private float height_ = 0f;
	private float angle_ = 90f;

#if UNITY_EDITOR

	public Vector2[] modVertices = new Vector2[2];
	public float modHeight = 0f;
	public float modAngle = 0f;

	private void AdjustIfModded()
	{
		bool doAdjust = false;
		for (int i = 0; i<2; i++)
		{
			if (modVertices[i] != baseVertices_[i])
			{
				baseVertices_[i] = modVertices[i];
				doAdjust = true;
			}
		}
		if (modHeight != height_)
		{
			height_ = modHeight;
			doAdjust = true;
		}
		if (modAngle != angle_)
		{
			angle_ = modAngle;
			doAdjust = true;
		}
		if (doAdjust)
		{
			if (DEBUG_PARALLELOGRAM)
			{
				Debug.Log( "Modded" );
			}
			Adjust( );
		}
	}
#endif

	#endregion private data

	#region MB Flow

	protected override void PostAwake( )
	{
#if UNITY_EDITOR
		for (int i = 0; i < 2; i++)
		{
			modVertices[i] = baseVertices_[i];
		}
		modHeight = height_;
		modAngle = angle_;
#endif
	}


	private void Update ()
	{
#if UNITY_EDITOR
		AdjustIfModded( );
#endif
	}

	public void Init(Field f, float d, Vector2[] vs, float h, float a)
	{
		if (vs.Length != 2)
		{
			throw new System.Exception( "vs.Length should be 3" );
		}

		base.Init( f, d );

		for (int i = 0; i<2; i++)
		{
			baseVertices_[i] = vs[i];
		}
		height_ = h;
		angle_ = a;

		if (DEBUG_PARALLELOGRAM)
		{
			Debug.Log( "Init() " + this.DebugDescribe( ) );
		}

		Adjust( );
	}

	#endregion MB Flow

	#region Mesh

	private Vector2[] GetPoints()
	{
		Vector2[] pts = new Vector2[4];

		pts[0] = baseVertices_[0];
		pts[1] = baseVertices_[1];

		Vector2 baseLine = pts[1] - pts[0];
		Vector3 perp = Vector3.Cross( baseLine, s_normal ).normalized;
		Vector2 perp2 = perp;
		float offset = 0f;

		if (!Mathf.Approximately( angle_, 90f))
		{
			offset = height_ / Mathf.Tan( Mathf.Deg2Rad * angle_ );
		}

		pts[2] = pts[1] + perp2 * height_ - baseLine.normalized * offset;
		pts[3] = pts[0] + perp2 * height_ - baseLine.normalized * offset;

		return pts; 
	}

	private static readonly Vector2[] s_uvs = new Vector2[]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(1f, 1f),
			new Vector2(0f, 1f),
		};

	public void Adjust()
	{
		if (field == null) // Don't make mesh if not initialised
		{
			return;
		}

#if UNITY_EDITOR
		for (int i = 0; i < 2; i++)
		{
			modVertices[i] = baseVertices_[i];
		}
		modHeight = height_;
		modAngle = angle_;
#endif
		Vector2[] pts = GetPoints( );
		
		Mesh mesh = GetMesh( );
		
		Vector3[] verts = new Vector3[4];
		Vector2[] uvs = new Vector2[4];
		Vector3[] normals = new Vector3[4];

		for (int i = 0; i < 4; i++)
		{
			verts[i] = new Vector3( pts[i].x, pts[i].y, depth );
			uvs[i] = s_uvs[i];
			normals[i] = s_normal;
		}

		mesh.vertices = verts;
		mesh.triangles = new int[6] { 0, 1, 2, 2, 3, 0 };
		mesh.uv = uvs;
		mesh.normals = normals;

		mesh.RecalculateBounds( );
		mesh.Optimize( );

		if (DEBUG_PARALLELOGRAM)
		{
			Debug.Log( "Adjust() " + this.DebugDescribe( ) );
		}
	}

	#endregion Mesh

	#region IDebugDescribable

	public void DebugDescribe(System.Text.StringBuilder sb)
	{
		sb.Append( "Parallelogram '" ).Append( gameObject.name ).Append( "': " );
		for (int i = 0; i<baseVertices_.Count; i++)
		{
			sb.Append( baseVertices_[i] ).Append( " " );
		}
		
		sb.Append( " height=" ).Append( height_ );
		sb.Append( " angle=" ).Append( angle_ );
		sb.Append( " d=" ).Append( depth );

		sb.Append( " pts=" );
		Vector2[] pts = GetPoints( );
		for (int i = 0; i<pts.Length; i++)
		{
			sb.Append( pts[i].ToString( ) );
		}

	}

	#endregion IDebugDescribable

}
