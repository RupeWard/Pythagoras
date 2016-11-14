using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
	Circle element

	Defined by its centre and radius
*/

namespace RJWard.Geometry
{
	public class Element_Circle : Element2DBase, RJWard.Core.IDebugDescribable
	{
		public static readonly bool DEBUG_CIRCLE = true;
		public static readonly bool DEBUG_CIRCLE_VERBOSE = true;

		private static readonly float minRadius = 0.05f;

		#region private data

		private Vector2 centre_;
		private float radius_ = 0f;
		private float minSectorEdgeLength_ = 0.02f;

		Element_Curve perimeterElement_ = null;

		#endregion private data

		#region in-editor modding 

#if UNITY_EDITOR
		// for in-editor modification

		public Vector2 modCentre;
		public float modRadius;

		override protected void CheckIfModded( )
		{
			bool modded = false;
			if (Vector2.Distance( modCentre, centre_) > Mathf.Epsilon)
			{
				centre_ = modCentre;
				modded = true;
			}
			if (!Mathf.Approximately(modRadius, radius_))
			{
				if (modRadius < minRadius)
				{
					Debug.LogWarning( "modRadius out of range at " + modRadius + ", fixing to minimum of " + minRadius );
					modRadius = radius_;
				}
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

		#region setters

		public void SetRadius(float r)
		{
			if (r < minRadius)
			{
				if (DEBUG_CIRCLE)
				{
					Debug.LogWarning( "Trying to set circle radius that's too small, reducing radius from " + r + " to " + minRadius );
				}
				r = minRadius;
			}
			if (! Mathf.Approximately(radius_, r))
			{
				radius_ = r;
				SetMeshDirty( );
			}
		}

		public void SetCentre(Vector2 c)
		{
			if ( Vector2.Distance(c, centre_) > Mathf.Epsilon)
			{
				centre_ = c;
				SetMeshDirty( );
			}
		}

		#endregion setters

		#region Setup

		// Helper to work out number of sectors from min edge length
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

			SetCentre( ce );
			SetRadius( r );

			decorator = new ElementDecorator_Circle( c, 1f, HandleColourChanged, HandleAlphaChanged );
			decorator.Apply( );

			if (DEBUG_CIRCLE)
			{
				Debug.Log( "Init() " + this.DebugDescribe( ) );
			}

			SetMeshDirty( );
		}

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

			List< Vector2 > perimeterPoints = new List< Vector2 >( );

			for (int i = 0; i < numSectors; i++)
			{
				float angle = angleStep * i;

				Vector2 perimeterPoint = new Vector2( centre_.x + radius_ * Mathf.Sin( angle ), centre_.y + radius_ * Mathf.Cos( angle ) );
				perimeterPoints.Add( perimeterPoint );

				verts[1+i] = new Vector3( perimeterPoint.x, perimeterPoint.y, depth );
				uvs[1+i] = Vector2.zero;
				normals[1+i] = s_normal;
			}

			List<int> tris = new List<int>( );
			for (int i = 0; i < numSectors; i++)
			{
				int nexti = (i + 1) % numSectors;

				tris.Add( 0 );
				tris.Add( 1 + i );
				tris.Add( 1 + nexti );
			}

			if (mesh.vertexCount > verts.Length) // Mesh fails when reducing number of vertices (because it immediately checks against triangles
			{
				mesh.Clear( );
			}

			mesh.vertices = verts;
			mesh.uv = uvs;
			mesh.normals = normals;
			mesh.triangles = tris.ToArray( );

			mesh.RecalculateBounds( );
			mesh.Optimize( );

			// TODO don't always destroy/create
			if (perimeterElement_ != null)
			{
				GameObject.Destroy( perimeterElement_.gameObject );
				perimeterElement_ = null;
			}

			perimeterElement_ = geometryFactory.AddCurveToField(
				field,
				name + " Perimeter",
				depth-GeometryHelpers.internalLayerSeparation,
				perimeterPoints,
				true,
				Element2DBase.defaultEdgeWidth,
				Color.cyan
				);
			perimeterElement_.cachedTransform.SetParent( cachedTransform );

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
			if (perimeterElement_ != null)
			{
				perimeterElement_.SetAlpha( a );
			}
		}

		#endregion Non-geometrical Appaarance

		#region IDebugDescribable

		public void DebugDescribe( System.Text.StringBuilder sb )
		{
			sb.Append( "Circle '" ).Append( gameObject.name ).Append(": ");
			sb.Append( "'C = " ).Append(centre_.ToString());
			sb.Append( " R = " ).Append( radius_ );
			sb.Append( " N = " ).Append( GetNumSectors( radius_, minSectorEdgeLength_ ) );
			sb.Append( " d=" ).Append( depth );
		}

		#endregion IDebugDescribable


	}

}
