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
			throw new System.Exception( "vs.Length should be 3" );
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
