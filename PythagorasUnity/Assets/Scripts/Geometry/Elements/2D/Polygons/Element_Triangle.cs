using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
	Triangle element

	Defined by its 3 vertices

	TODO: handle co-linear case (not bothering with yet as not expecting to encounter it)
*/

namespace RJWard.Geometry
{
	public class Element_Triangle : ElementPolygonBase, RJWard.Core.IDebugDescribable
	{
		public static readonly bool DEBUG_TRIANGLE = true;
		public static readonly bool DEBUG_TRIANGLE_VERBOSE = false;

		#region private data

		private List< Vector2 > vertices_ = new List< Vector2 >( 3 ) { Vector3.zero, Vector3.zero, Vector3.zero }; // the vertices

		#endregion private data

		#region in-editor modding 

#if UNITY_EDITOR
		// for in-editor modification

		public Vector2[] modVertices = new Vector2[3];

		override protected void CheckIfModded( )
		{
			bool modded = false;
			for (int i = 0; i < 3; i++)
			{
				if (modVertices[i] != vertices_[i])
				{
					vertices_[i] = modVertices[i];
					modded = true;
				}
			}
			if (modded)
			{
				if (DEBUG_TRIANGLE)
				{
					Debug.Log( "Modded " + gameObject.name );
				}
				SetMeshDirty( );
			}
		}

		protected override void SetModdingValues( )
		{
			for (int i = 0; i < 3; i++)
			{
				modVertices[i] = vertices_[i];
			}
		}

#endif

		#endregion in-editor modding 

		#region Setup

		public void Init( 
			GeometryFactory gf, Field f, float d, // for base
			Vector2[] vs, 
			Color c )
		{
			if (vs.Length != 3)
			{
				throw new System.Exception( "vs.Length should be 3, not " + vs.Length + " on trying to Init " + gameObject.name );
			}

			base.Init( gf, f, d, 3 );

			for (int i = 0; i < 3; i++)
			{
				vertices_[i] = vs[i];
			}

			decorator = new ElementDecorator_Triangle( c, 1f, HandleColourChanged, HandleAlphaChanged );

			if (DEBUG_TRIANGLE)
			{
				Debug.Log( "Init() " + this.DebugDescribe( ) );
			}

			SetMeshDirty( );
		}

		public void InitRightAngled(  
			GeometryFactory gf, Field f, float d,  // for base
			Vector2[] hypotenuseEnds, 
			float angle, 
			Color c )
		{
			if (hypotenuseEnds.Length != 2)
			{
				throw new System.Exception( "baseline.Length should be 2, not " + hypotenuseEnds.Length + " on trying to Init (right-angled) " + gameObject.name );
			}

			if (angle >= 90f)
			{
				throw new System.Exception( "angle in right triangle should be <90, not " + angle + " on trying to Init (right-angled) " + gameObject.name );
			}

			base.Init( gf, f, d, 3 );

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

			decorator = new ElementDecorator_Triangle( c, 1f, HandleColourChanged, HandleAlphaChanged );

			if (DEBUG_TRIANGLE)
			{
				Debug.Log( "Init() " + this.DebugDescribe( ) );
			}
			SetMeshDirty( );
		}

		private Element_StraightLine CreateEdge(int n)
		{
			Element_StraightLine edge = geometryFactory.AddStraightLineToField(
				field,
				name+" Edge_" + n.ToString( ),
				depth-GeometryHelpers.internalLayerSeparation,
				new Vector2[]
				{
						vertices_[ modIndex(n)],
						vertices_[modIndex(n+1)]
				},
				Element2DBase.defaultEdgeWidth,
				Color.cyan /* TODO: decorator.colour*/ );
			edge.cachedTransform.SetParent( cachedTransform );
			edge.gameObject.tag = GeometryHelpers.Tag_SubElement;
			SetEdge( n, edge );
			return edge;
		}

		#endregion Setup

		#region creation

		protected override void OnClone< T >( T src )
		{
			Element_Triangle t = src as Element_Triangle;
			if (t == null)
			{
				throw new System.Exception( gameObject.name + ": Triangles can currently only be cloned as Triangles" );
			}
			Init( t.geometryFactory, t.field, t.depth, t.vertices_.ToArray( ), decorator.colour );
		}

		public override ElementBase Clone( string name )
		{
			return this.Clone< Element_Triangle >( name );
		}

		#endregion creation

		#region Element2DBase

		protected override Vector2[] GetVertices( )
		{
			return vertices_.ToArray();
		}

		#endregion Element2DBase

		#region Mesh

		// Call this when the definition changes
		override protected void DoAdjustMesh( )
		{
			if (field == null) // Don't make mesh if not initialised
			{
				return;
			}

#if UNITY_EDITOR
			SetModdingValues( );
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

			for (int i = 0; i < 3; i++)
			{
				Element1DBase edge = GetEdge( i );
				if (edge == null)
				{
					edge = CreateEdge( i );
				}
				else
				{
					(edge as Element_StraightLine).SetEnds( vertices_[modIndex( i )], vertices_[modIndex(i + 1)] );
				}
			}
			if (DEBUG_TRIANGLE_VERBOSE)
			{
				Debug.Log( "DoAdjustMesh() " + this.DebugDescribe( ) );
			}
		}

		#endregion Mesh

		#region geometry helpers

		public Vector2[] GetSideInternal( int n )
		{
			return GetSide( n, false );
		}

		public Vector2[] GetSideExternal( int n )
		{
			return GetSide( n, true );
		}

		private Vector2[] GetSide( int n, bool flip ) // n = 0 is between vertices 0 & 1, n = 1 is between 1 & 2, n = 2 is between 2 & 0. flip reverses order
		{
			if (n < 0 || n > 2)
			{
				throw new System.Exception( "For side of " + gameObject.name + ", n must be in [0,2], not " + n.ToString( ) );
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

		public float GetSideLength( int n ) // n = 0 is between vertices 0 & 1, n = 1 is between 1 & 2, n = 2 is between 2 & 0
		{
			if (n < 0 || n > 2)
			{
				throw new System.Exception( "For side length of " + gameObject.name + ", n must be in [0,2], not " + n.ToString( ) );
			}
			Vector2[] side = GetSideInternal( n );
			return Vector2.Distance( side[0], side[1] );
		}

		public float GetInternalAngleDegrees( int n ) // n is the vertex index 
		{
			if (n < 0 || n > 2)
			{
				throw new System.Exception( "For internal angle of " + gameObject.name + ", n must be in [0,2], not " + n.ToString( ) );
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
			float a = GetSideLength( indexOfA % 3 );
			float b = GetSideLength( (indexOfA + 1) % 3 );
			float c = GetSideLength( (indexOfA + 2) % 3 );

			float cos = (a * a - b * b - c * c) / (-2f * b * c);
			angle = Mathf.Rad2Deg * Mathf.Acos( cos );

			return angle;
		}

		#endregion geometry helpers

		#region Non-geometrical Appaarance

		override protected void HandleColourChanged( Color c)
		{
			cachedMaterial.SetColor( "_Color", c );
		}

		override protected void HandleAlphaChanged( float a )
		{
			cachedMaterial.SetFloat( "_Alpha", a );
		}

		#endregion Non-geometrical Appaarance

		#region IDebugDescribable

		public void DebugDescribe( System.Text.StringBuilder sb )
		{
			sb.Append( "Triangle '" ).Append( gameObject.name ).Append( "': " );
			for (int i = 0; i < vertices_.Count; i++)
			{
				sb.Append( vertices_[i] ).Append( " " );
			}
			sb.Append( " d=" ).Append( depth );
		}

		#endregion IDebugDescribable


	}

}
