using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
	Triangle element

	Defined by its 3 vertices

	TODO: handle co-linear case (not bothering with yet as not expecting to encounter it)
*/

public class Triangle : Element, RJWard.Core.IDebugDescribable
{
	public static readonly bool DEBUG_TRIANGLE = true;

	#region private data

	private List< Vector2 > vertices_ = new List<Vector2>( 3 ) { Vector3.zero, Vector3.zero, Vector3.zero }; // the vertices

#if UNITY_EDITOR
	// for in-editor modification

	public Vector2[] modVertices = new Vector2[3];

	private void AdjustIfModded()
	{
		bool doAdjust = false;
		for (int i = 0; i<3; i++)
		{
			if (modVertices[i] != vertices_[i])
			{
				vertices_[i] = modVertices[i];
				doAdjust = true;
			}
		}
		if (doAdjust)
		{
			if (DEBUG_TRIANGLE)
			{
				Debug.Log( "Modded "+gameObject.name );
			}
			AdjustMesh( );
		}
	}
#endif

	#endregion private data

	#region MB Flow

	protected override void PostAwake()
	{
#if UNITY_EDITOR
		for (int i = 0; i<3; i++)
		{
			modVertices[i] = vertices_[i];
		}
#endif
	}

	private void Update ()
	{
#if UNITY_EDITOR
		AdjustIfModded( );
#endif
	}

	public void Init(Field f, float d, Vector2[] vs, Color c)
	{
		if (vs.Length != 3)
		{
			throw new System.Exception( "vs.Length should be 3, not "+vs.Length );
		}

		base.Init( f, d );

		for (int i = 0; i<3; i++)
		{
			vertices_[i] = vs[i];
		}

		if (DEBUG_TRIANGLE)
		{
			Debug.Log( "Init() " + this.DebugDescribe( ) );
		}

		AdjustMesh( );
		SetColour( c );
	}

	public void InitRightAngled( Field f, float d, Vector2[] hypotenuseEnds, float angle, Color c )
	{
		if (hypotenuseEnds.Length != 2)
		{
			throw new System.Exception( "baseline.Length should be 2, not "+hypotenuseEnds.Length );
		}

		if (angle >= 90f)
		{
			throw new System.Exception( "angle in right triangle should be <90, not " + angle );
		}

		base.Init( f, d );

		for (int i = 0; i < 2; i++)
		{
			vertices_[i] = hypotenuseEnds[i];
		}

		Vector2 hypotenuse = hypotenuseEnds[1] - hypotenuseEnds[0];
		Vector2 perp = Vector3.Cross( hypotenuse, s_normal ).normalized;

		float r = 0.5f * hypotenuse.magnitude; // radius of circle
		float d1 = 2f * r * Mathf.Cos( Mathf.Deg2Rad * angle ); // side next to angle (cosine rule)

		float dx = d1 * Mathf.Cos( Mathf.Deg2Rad * angle ); // distance from V1 of projection of V2 onto hypotenuse
		float dxProportion = dx / hypotenuse.magnitude;

		float h = Mathf.Sqrt( d1 * d1 - dx * dx ); // height of triangle (length of projection of V2 onto hypotenuse)

		Vector2 X = hypotenuseEnds[1] - dxProportion * hypotenuse; // position of projection of V2 onto hypotenuse

		vertices_[2] = X + perp * h;

		if (DEBUG_TRIANGLE)
		{
			Debug.Log( "Init() " + this.DebugDescribe( ) );
		}

		AdjustMesh( );
		SetColour( c );
	}


	#endregion MB Flow

	#region Mesh

	// Call this when the definition changes
	public void AdjustMesh()
	{
		if (field == null) // Don't make mesh if not initialised
		{
			return;
		}

#if UNITY_EDITOR
		for (int i = 0; i < 3; i++)
		{
			modVertices[i] = vertices_[i];
		}
#endif
		Mesh mesh = GetMesh( );
		
		Vector3[] verts = new Vector3[3];
		Vector2[] uvs = new Vector2[3];
		Vector3[] normals = new Vector3[3];

		for (int i = 0; i < 3; i++)
		{
			verts[i] = new Vector3( vertices_[i].x, vertices_[i].y, depth );
			uvs[i] = Vector2.zero;
			normals[i] = s_normal;
		}

		mesh.vertices = verts;
		mesh.triangles = new int[3] { 0, 1, 2 };
		mesh.uv = uvs;
		mesh.normals = normals;

		mesh.RecalculateBounds( );
		mesh.Optimize( );

		if (DEBUG_TRIANGLE)
		{
			Debug.Log( "Adjust() " + this.DebugDescribe( ) );
		}
	}

	#endregion Mesh

	#region geometry helpers

	public Vector2[] GetSideInternal(int n)
	{
		return GetSide( n, false );
	}

	public Vector2[] GetSideExternal( int n )
	{
		return GetSide( n, true);
	}

	private Vector2[] GetSide(int n, bool flip ) // n = 0 is between vertices 0 & 1, n = 1 is between 1 & 2, n = 2 is between 2 & 0. flip reverses order
	{
		if (n < 0 || n > 2)
		{
			throw new System.Exception( "For side, n must be in [0,2], not " + n.ToString( ) );
		}
		Vector2[] result = new Vector2[2];
		switch (n)
		{
			case 0:
				result[0] = vertices_[0];
				result[1] = vertices_[1];
				break;
			case 1:
				result[0] = vertices_[1];
				result[1] = vertices_[2];
				break;
			case 2:
				result[0] = vertices_[2];
				result[1] = vertices_[0];
				break;
		}
		if (flip)
		{
			Vector2 tmp = result[0];
			result[0] = result[1];
			result[1] = tmp;
		}
		return result;
	}

	public float GetSideLength(int n) // n = 0 is between vertices 0 & 1, n = 1 is between 1 & 2, n = 2 is between 2 & 0
	{
		if (n < 0 || n > 2)
		{
			throw new System.Exception( "For side length, n must be in [0,2], not " + n.ToString( ) );
		}
		Vector2[] side = GetSideInternal( n );
		return Vector2.Distance( side[0], side[1] );
	}

	public float GetInternalAngleDegrees(int n) // n is the vertex index 
	{
		if (n < 0 || n > 2)
		{
			throw new System.Exception( "For internal angle, n must be in [0,2], not " + n.ToString( ) );
		}
		float angle = 0f;

		/*
		switch (n) // used to work out single formula below
		{
			case 0:
				a = GetSideLength( 1 );
				b = GetSideLength( 2 );
				c = GetSideLength( 0 );
				break;
			case 1:
				a = GetSideLength( 2 );
				b = GetSideLength( 0 );
				c = GetSideLength( 1 );
				break;
			case 2:
				a = GetSideLength( 0 );
				b = GetSideLength( 1 );
				c = GetSideLength( 2 );
				break;
		}
		*/

		int indexOfA = (n + 1);
		float a = GetSideLength( indexOfA % 3);
		float b = GetSideLength( (indexOfA + 1) % 3 );
		float c = GetSideLength( (indexOfA + 2) % 3 );

		float cos = (a * a - b * b - c * c) / (-2f * b * c);
		angle = Mathf.Rad2Deg * Mathf.Acos( cos );

		return angle;
	}

	#endregion geometry helpers

	#region IDebugDescribable

	public void DebugDescribe(System.Text.StringBuilder sb)
	{
		sb.Append( "Triangle '" ).Append( gameObject.name ).Append( "': " );
		for (int i = 0; i<vertices_.Count; i++)
		{
			sb.Append( vertices_[i] ).Append( " " );
		}
		sb.Append( " d=" ).Append( depth );
	}

	#endregion IDebugDescribable

	#region Non-geometrical Appaarance

	public void SetColour( Color c )
	{
		SetColour( c, 1f );
	}

	public void SetColour(Color c, float a)
	{
		cachedMaterial.SetColor( "_Color", c );
		cachedMaterial.SetFloat( "_Alpha", a );
	}

	#endregion Non-geometrical Appaarance
}
