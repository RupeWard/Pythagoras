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

		private static readonly float rightAngleTolerance = 0.1f;

		private static readonly float minRadius = 0.05f;

		#region private data

		private Vector2 centre_;
		private float radius_ = 0f;
		private float angleExtentDegrees_ = 0f;
		private float angleDirectionDegrees_ = 0f;

		private float minSectorEdgeLength_ = 0.02f;

		private bool isAngleMarker = false;

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
			decorator.Apply( );

			if (DEBUG_SECTOR)
			{
				Debug.Log( "Init() " + this.DebugDescribe( ) );
			}

			SetMeshDirty( );
		}

		public void Init(
			GeometryFactory gf, Field f, float d, // for base
			Element_StraightLine[] lines,
			float r,
			Color c )
		{
			base.Init( gf, f, d );

			if (false == Element_StraightLine.Intersection( lines[0], lines[1], ref centre_))
			{
				throw new System.Exception( "StraightLines passed to Element_Sector.Init are parallel. " + lines[0].DebugDescribe( ) + " " + lines[1].DebugDescribe( ) );
			}
			radius_ = r;
			if (radius_ < minRadius)
			{
				if (DEBUG_SECTOR)
				{
					Debug.LogWarning( "Trying to create a sector that's too small, reducing radius from " + r + " to " + minRadius );
				}
				radius_ = minRadius;
			}

			Vector2 line0direction = -1f * lines[0].GetDirection( );
			float angleDirectionLine0 = Mathf.Rad2Deg * Mathf.Atan2( line0direction.y, line0direction.x ); // angle of line 0

			Vector2 line1direction = lines[1].GetDirection( );
			float angleDirectionLine1 = Mathf.Rad2Deg * Mathf.Atan2( line1direction.y, line1direction.x ); // angle of line 1

			angleDirectionDegrees_ = angleDirectionLine1;
			
			while (angleDirectionLine0 < angleDirectionDegrees_)
			{
				angleDirectionLine0 += 360f;
			}

			angleExtentDegrees_ = angleDirectionLine0 - angleDirectionDegrees_;

			decorator = new ElementDecorator_Circle( c, 1f, HandleColourChanged, HandleAlphaChanged );
			decorator.Apply( );

			if (DEBUG_SECTOR)
			{
				Debug.Log( "Init() " + this.DebugDescribe( ) );
			}

			SetMeshDirty( );
		}

		public void SetAngleMarker()
		{
			isAngleMarker = true;
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
			bool doRightAngle = (isAngleMarker && RJWard.Core.CSharpExtensions.EqualsApprox( angleExtentDegrees_, 90f, rightAngleTolerance ));

			int numSectors = (doRightAngle)?(2):( GetNumSectors( radius_, angleExtentDegrees_, minSectorEdgeLength_ ));

			Mesh mesh = GetMesh( );
			
			if (numSectors > 0)
			{
				Vector3[] verts = new Vector3[numSectors + 2];
				Vector2[] uvs = new Vector2[numSectors + 2];
				Vector3[] normals = new Vector3[numSectors + 2];

				verts[0] = new Vector3( centre_.x, centre_.y, depth );
				uvs[0] = Vector2.zero;
				normals[0] = s_normal;

				float angleStep = Mathf.Deg2Rad * angleExtentDegrees_ / numSectors;

				List<Vector2> perimeterPoints = new List<Vector2>( );

				if (doRightAngle)
				{
					float d = radius_ / Mathf.Sqrt( 2 );

					float angle0 = Mathf.Deg2Rad * angleDirectionDegrees_;
					Vector2 perimeterPoint0 = new Vector2( centre_.x + d * Mathf.Cos( angle0 ), centre_.y + d * Mathf.Sin( angle0 ) );

					verts[1] = new Vector3( perimeterPoint0.x, perimeterPoint0.y, depth );
					uvs[1] = Vector2.zero;
					normals[1] = s_normal;

					float angle2 = angle0 + Mathf.Deg2Rad * angleExtentDegrees_;
					Vector2 perimeterPoint2 = new Vector2( centre_.x + d * Mathf.Cos( angle2 ), centre_.y + d * Mathf.Sin( angle2 ) );

					verts[3] = new Vector3( perimeterPoint2.x, perimeterPoint2.y, depth );
					uvs[3] = Vector2.zero;
					normals[3] = s_normal;

					Vector2 perimeterPoint1 = perimeterPoint2 + (perimeterPoint0 - centre_);

					verts[2] = new Vector3( perimeterPoint1.x, perimeterPoint1.y, depth );
					uvs[2] = Vector2.zero;
					normals[2] = s_normal;

					perimeterPoints.Add( perimeterPoint0 );
					perimeterPoints.Add( perimeterPoint1 );
					perimeterPoints.Add( perimeterPoint2 );
				}
				else
				{
					for (int i = 0; i <= numSectors; i++)
					{
						float angle = Mathf.Deg2Rad * angleDirectionDegrees_ + angleStep * i;

						Vector2 perimeterPoint = new Vector2( centre_.x + radius_ * Mathf.Cos( angle ), centre_.y + radius_ * Mathf.Sin( angle ) );
						perimeterPoints.Add( perimeterPoint );

						verts[1 + i] = new Vector3( perimeterPoint.x, perimeterPoint.y, depth );
						uvs[1 + i] = Vector2.zero;
						normals[1 + i] = s_normal;
					}
				}


				List<int> tris = new List<int>( );
				for (int i = 0; i < numSectors; i++)
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

				// TODO dependent on whether we want to show it
				if (perimeterPoints.Count > 1)
				{
					if (arcElement_ != null)
					{
						arcElement_.SetPoints( perimeterPoints );
						ShowArc( ); 
					}
					else
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
				}
				else
				{
					HideArc( );
					if (DEBUG_SECTOR)
					{
						Debug.LogWarning( "Only " + perimeterPoints.Count + " perimeter points" );
					}
				}

				// TODO dependent on whether we want to show them

				// refactor into loop and function
				Vector2[] edgeEnds = new Vector2[2];// { verts[0], verts[1] };

				for (int i = 0; i < 2; i++)
				{
					if (i == 0)
					{
						edgeEnds[0] = verts[0];
						edgeEnds[1] = verts[1];
					}
					else
					{
						edgeEnds[0] = verts[verts.Length - 1];
						edgeEnds[1] = verts[0];
					}
					if (edgeElements_[i] != null)
					{
						edgeElements_[i].SetEnds( edgeEnds );
					}
					else
					{
						edgeElements_[i] = geometryFactory.AddStraightLineToField(
							field,
							name + " Edge_"+i,
							depth - GeometryHelpers.internalLayerSeparation,
							edgeEnds,
							Element2DBase.defaultEdgeWidth,
							Color.cyan );// TODO use decorator
						edgeElements_[i].cachedTransform.SetParent( cachedTransform );
					}
				}
			}
			else
			{
				mesh.Clear( );
				DestroyAllSubElements( );
			}
			if (DEBUG_SECTOR_VERBOSE)
			{
				Debug.Log( "DoAdjustMesh() " + this.DebugDescribe( ) );
			}
		}

		public void DestroyArc()
		{
			if (arcElement_ != null)
			{
				GameObject.Destroy( arcElement_.gameObject );
				arcElement_ = null;
			}
		}

		public void DestroyEdge(int n)
		{
			if (edgeElements_[n] != null)
			{
				GameObject.Destroy( edgeElements_[n].gameObject );
				edgeElements_[n] = null;
			}
		}

		public void DestroyBothEdges()
		{
			for (int i = 0; i < 2; i++)
			{
				DestroyEdge( i );
			}
		}

		private void DestroyAllSubElements()
		{
			DestroyArc( );
			DestroyBothEdges( );
		}

		public void ShowArc( bool b )
		{
			if (arcElement_ != null)
			{
				arcElement_.gameObject.SetActive( b );
			}
		}

		public void ShowArc( )
		{
			ShowArc( true );
		}

		public void HideArc( )
		{
			ShowArc( false );
		}

		public void ShowEdge( int n, bool b )
		{
			if (edgeElements_[n] != null)
			{
				edgeElements_[n].gameObject.SetActive( b );
			}
		}

		public void ShowEdge( int n)
		{
			ShowEdge( n, true );
		}

		public void HideEdge( int n )
		{
			ShowEdge( n, false );
		}

		public void ShowBothEdges( bool b)
		{
			for (int i = 0; i < 2; i++)
			{
				ShowEdge( i, b );
			}
		}

		public void ShowBothEdges( )
		{
			ShowBothEdges( true );
		}

		public void HideBothEdges( )
		{
			ShowBothEdges( false );
		}

		public void ShowAllSubElements( bool b)
		{
			ShowArc( b );
			ShowBothEdges( b );
		}

		public void ShowAllSubElements( )
		{
			ShowAllSubElements( true );
		}

		public void HideAllSubElements( )
		{
			ShowAllSubElements( false);
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
