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
	public class Element_Circle : Element2DBase, RJWard.Core.IDebugDescribable
	{
		public static readonly bool DEBUG_CIRCLE = true;
		public static readonly bool DEBUG_CIRCLE_VERBOSE = false;

		#region private data

		private Vector2 centre_;
		private float radius_ = 0f;
		private float minSectorEdgeLength_ = 0.1f;

		#endregion private data

		#region in-editor modding 

#if UNITY_EDITOR
		// for in-editor modification

		public Vector2 modCentre;
		public float modRadius;

		override protected void CheckIfModded( )
		{
			bool modded = false;
			if (modCentre != centre_)
			{
				centre_ = modCentre;
				modded = true;
			}
			if (modRadius != radius_)
			{
				radius_ = modRadius;
				modded = true;
			}
			
			if (modded)
			{
				if (DEBUG_CIRCLE)
				{
					Debug.Log( "Modded " + gameObject.name );
				}
				SetMeshDirty( );
			}
		}

		protected override void SetModdingValues( )
		{
			modCentre = centre_;
			modRadius = radius_;
		}

#endif

		#endregion in-editor modding 

		#region Setup

		public static int GetNumSectors( float radius, float minSectorEdgeLength)
		{
			float perimeter = 2f * Mathf.PI * radius;
			return Mathf.CeilToInt( perimeter / minSectorEdgeLength );
		}

		public void Init( 
			GeometryFactory gf, Field f, float d, // for base
			Vector2 ce,
			float r, 
			Color c )
		{
			base.Init( gf, f, d);

			centre_ = ce;
			radius_ = r;

			decorator = new ElementDecorator_Circle( c, 1f, HandleColourChanged, HandleAlphaChanged );

			if (DEBUG_CIRCLE)
			{
				Debug.Log( "Init() " + this.DebugDescribe( ) );
			}

			SetMeshDirty( );
		}

		/*
		private Element_StraightLine CreateEdge(int n)
		{
			Element_StraightLine edge = geometryFactory.AddStraightLineToField(
				field,
				name+" Edge_" + n.ToString( ),
				-GeometryHelpers.internalLayerSeparation,
				new Vector2[]
				{
						vertices_[ modIndex(n)],
						vertices_[modIndex(n+1)]
				},
				Element2DBase.defaultEdgeWidth,
				decorator.colour );
			edge.cachedTransform.SetParent( cachedTransform );
			edge.gameObject.tag = "Edge";
			SetEdge( n, edge );
			return edge;
		}
	*/
		#endregion Setup

		#region creation

		protected override void OnClone< T >( T src )
		{
			Element_Circle t = src as Element_Circle;
			if (t == null)
			{
				throw new System.Exception( gameObject.name + ": Circles can currently only be cloned as Circles" );
			}
			Init( t.geometryFactory, t.field, t.depth, t.centre_, t.radius_, decorator.colour );
		}

		public override ElementBase Clone( string name )
		{
			return this.Clone< Element_Circle>( name );
		}

		#endregion creation

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
			int numSectors = GetNumSectors( radius_, minSectorEdgeLength_ );

			Mesh mesh = GetMesh( );

			Vector3[] verts = new Vector3[numSectors + 1];
			Vector2[] uvs = new Vector2[numSectors + 1];
			Vector3[] normals = new Vector3[numSectors + 1];

			verts[0] = new Vector3( centre_.x, centre_.y, depth );
			uvs[0] = Vector2.zero;
			normals[0] = s_normal;

			float angleStep = (2f * Mathf.PI) / numSectors;

			for (int i = 0; i < numSectors; i++)
			{
				float angle = angleStep * i;
				 
				verts[i] = new Vector3( centre_.x + radius_ * Mathf.Sin(angle), centre_.y + radius_ * Mathf.Cos( angle ), depth );
				uvs[i] = Vector2.zero;
				normals[i] = s_normal;
			}

			List<int> tris = new List<int>( );
			for (int i = 0; i < numSectors; i++)
			{
				int nexti = (i + 1) % numSectors;

				tris.Add( 0 );
				tris.Add( 1 + i );
				tris.Add( 1 + nexti );
			}

			mesh.vertices = verts;
			mesh.triangles = tris.ToArray();
			mesh.uv = uvs;
			mesh.normals = normals;

			mesh.RecalculateBounds( );
			mesh.Optimize( );

			/*
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
			}*/

			if (DEBUG_CIRCLE_VERBOSE)
			{
				Debug.Log( "DoAdjustMesh() " + this.DebugDescribe( ) );
			}
		}

		#endregion Mesh

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
			sb.Append( "Circle '" ).Append( gameObject.name ).Append(": ");
			sb.Append( "'C = " ).Append(centre_.ToString());
			sb.Append( " R = " ).Append( radius_ );
			sb.Append( " d=" ).Append( depth );
		}

		#endregion IDebugDescribable


	}

}
