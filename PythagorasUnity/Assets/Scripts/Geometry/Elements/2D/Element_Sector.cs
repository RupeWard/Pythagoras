using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
	Sector element

	Defined by its centre, radius, angle, and direction
*/

namespace RJWard.Geometry
{
	public class Element_Sector : Element2DBase, RJWard.Core.IDebugDescribable
	{
		public static readonly bool DEBUG_SECTOR = true;
		public static readonly bool DEBUG_SECTOR_VERBOSE = true;

		private static readonly float minRadius = 0.05f;

		#region private data

		private Vector2 centre_;
		private float radius_ = 0f;
		private float angleExtentDegrees_ = 0f;
		private float angleDirectionDegrees_ = 0f;

		private float minSectorEdgeLength_ = 0.02f;

		Element_Curve arcElement_ = null;
		Element_StraightLine[] edgeElements_ = new Element_StraightLine[2];

		#endregion private data

		#region in-editor modding 

#if UNITY_EDITOR
		// for in-editor modification

		public Vector2 modCentre;
		public float modRadius;
		public float modAngleExtentDegrees;
		public float modAngleDirectionDegrees;

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
				if (modRadius < minRadius)
				{
					Debug.LogWarning( "modRadius out of range at " + modRadius + ", fixing to minimum of " + minRadius );
					modRadius = radius_;
				}
				radius_ = modRadius;
				modded = true;
			}
			if (modAngleExtentDegrees != angleExtentDegrees_)
			{
				angleExtentDegrees_ = modAngleExtentDegrees;
				modded = true;
			}
			if (modAngleDirectionDegrees != angleDirectionDegrees_)
			{
				angleDirectionDegrees_ = modAngleDirectionDegrees;
				modded = true;
			}
			if (modded)
			{
				if (DEBUG_SECTOR)
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
			modAngleExtentDegrees = angleExtentDegrees_;
			modAngleDirectionDegrees = angleDirectionDegrees_;
		}

#endif

		#endregion in-editor modding 

		#region Setup

		// Helper to work out number of sectors from min edge length
		public static int GetNumSectors( float radius, float angleDegrees, float minSectorEdgeLength)
		{
			float perimeter = 2f * Mathf.PI * radius * angleDegrees / 360f;
			return Mathf.CeilToInt( perimeter / minSectorEdgeLength );
		}

		public void Init( 
			GeometryFactory gf, Field f, float d, // for base
			Vector2 ce,
			float r, 
			float ae,
			float ad,
			Color c )
		{
			base.Init( gf, f, d);

			centre_ = ce;
			radius_ = r;
			if (radius_ < minRadius)
			{
				if (DEBUG_SECTOR)
				{
					Debug.LogWarning( "Trying to create a sector that's too small, reducing radius from " + r + " to " + minRadius );
				}
				radius_ = minRadius;
			}
			angleExtentDegrees_ = ae;
			angleDirectionDegrees_ = ad;

			decorator = new ElementDecorator_Circle( c, 1f, HandleColourChanged, HandleAlphaChanged );

			if (DEBUG_SECTOR)
			{
				Debug.Log( "Init() " + this.DebugDescribe( ) );
			}

			SetMeshDirty( );
		}

		#endregion Setup

		#region creation

		protected override void OnClone< T >( T src )
		{
			Element_Sector s = src as Element_Sector;
			if (s == null)
			{
				throw new System.Exception( gameObject.name + ": Sectors can currently only be cloned as Sectors" );
			}
			Init( s.geometryFactory, s.field, s.depth, s.centre_, s.radius_, s.angleExtentDegrees_, s.angleDirectionDegrees_, s.decorator.colour );
		}

		public override ElementBase Clone( string name )
		{
			return this.Clone< Element_Sector >( name );
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
			int numSectors = GetNumSectors( radius_, angleExtentDegrees_, minSectorEdgeLength_ );

			Mesh mesh = GetMesh( );
			
			if (numSectors > 0)
			{
				Vector3[] verts = new Vector3[numSectors + 1];
				Vector2[] uvs = new Vector2[numSectors + 1];
				Vector3[] normals = new Vector3[numSectors + 1];

				verts[0] = new Vector3( centre_.x, centre_.y, depth );
				uvs[0] = Vector2.zero;
				normals[0] = s_normal;

				float angleStep = Mathf.Deg2Rad * angleExtentDegrees_ / numSectors;

				List<Vector2> perimeterPoints = new List<Vector2>( );

				for (int i = 0; i < numSectors; i++)
				{
					float angle = Mathf.Deg2Rad * angleDirectionDegrees_ + angleStep * i;

					Vector2 perimeterPoint = new Vector2( centre_.x + radius_ * Mathf.Cos( angle ), centre_.y + radius_ * Mathf.Sin( angle ) );
					perimeterPoints.Add( perimeterPoint );

					verts[1 + i] = new Vector3( perimeterPoint.x, perimeterPoint.y, depth );
					uvs[1 + i] = Vector2.zero;
					normals[1 + i] = s_normal;
				}

				List<int> tris = new List<int>( );
				for (int i = 0; i < numSectors - 1; i++)
				{
					tris.Add( 0 );
					tris.Add( 1 + i );
					tris.Add( 2 + i );
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
				DestroySubElements( );

				if (perimeterPoints.Count > 1)
				{
					arcElement_ = geometryFactory.AddCurveToField(
						field,
						name + " Perimeter",
						depth - GeometryHelpers.internalLayerSeparation,
						perimeterPoints,
						false,
						Element2DBase.defaultEdgeWidth,
						Color.cyan // TODO use decorator
						);
					arcElement_.cachedTransform.SetParent( cachedTransform );
				}
				else
				{
					Debug.LogWarning( "Only " + perimeterPoints.Count + " perimeter points" );
				}

				// refactor into loop and function
				Vector2[] edgeEnds = new Vector2[] { verts[0], verts[1] };

				edgeElements_[0] = geometryFactory.AddStraightLineToField(
					field,
					name + " Edge_0",
					depth - GeometryHelpers.internalLayerSeparation,
					edgeEnds,
					Element2DBase.defaultEdgeWidth,
					Color.cyan );// TODO use decorator
				edgeElements_[0].cachedTransform.SetParent( cachedTransform );

				edgeEnds[0] = verts[verts.Length - 1];
				edgeEnds[1] = verts[0];

				edgeElements_[1] = geometryFactory.AddStraightLineToField(
					field,
					name + " Edge_1",
					depth - GeometryHelpers.internalLayerSeparation,
					edgeEnds,
					Element2DBase.defaultEdgeWidth,
					Color.cyan );// TODO use decorator
				edgeElements_[1].cachedTransform.SetParent( cachedTransform );
			}
			else
			{
				mesh.Clear( );
				DestroySubElements( );
			}
			if (DEBUG_SECTOR_VERBOSE)
			{
				Debug.Log( "DoAdjustMesh() " + this.DebugDescribe( ) );
			}
		}

		private void DestroySubElements()
		{
			if (arcElement_ != null)
			{
				GameObject.Destroy( arcElement_.gameObject );
				arcElement_ = null;
			}
			for (int i = 0; i < 2; i++)
			{
				if (edgeElements_[i] != null)
				{
					GameObject.Destroy( edgeElements_[i].gameObject );
					edgeElements_[i] = null;
				}
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
			if (arcElement_ != null)
			{
				arcElement_.SetAlpha( a );
			}
			for (int i = 0; i<2; i++)
			{
				if (edgeElements_[i] != null)
				{
					edgeElements_[i].SetAlpha( a );
				}
			}
		}

		#endregion Non-geometrical Appaarance

		#region IDebugDescribable

		public void DebugDescribe( System.Text.StringBuilder sb )
		{
			sb.Append( "Circle '" ).Append( gameObject.name ).Append(": ");
			sb.Append( "'C = " ).Append(centre_.ToString());
			sb.Append( " R = " ).Append( radius_ );
			sb.Append( " A ext = " ).Append( angleExtentDegrees_);
			sb.Append( " A dir = " ).Append( angleDirectionDegrees_);
			sb.Append( " N = " ).Append( GetNumSectors( radius_, angleExtentDegrees_, minSectorEdgeLength_ ) );
			sb.Append( " d=" ).Append( depth );
		}

		#endregion IDebugDescribable


	}

}
