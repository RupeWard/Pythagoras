using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
	Parallelogram element

	Defined by a baseline, the height, and one of the internal angles

	TODO: explain the winding order stuff
	TODO: handle co-linear case (not bothering with yet as not expecting to encounter it)
*/

namespace RJWard.Geometry
{
	public class Element_Parallelogram : ElementPolygonBase, RJWard.Core.IDebugDescribable
	{
		public static bool DEBUG_PARALLELOGRAM = true;
		public static bool DEBUG_PARALLELOGRAM_VERBOSE = false;

		#region private data

		private List< Vector2 > baseVertices_ = new List< Vector2 >( 2 ) { Vector3.zero, Vector3.zero };
		private float height_ = 0f;
		private float angleDegrees_ = 90f;

		#endregion private data

		#region properties

		public float height
		{
			get { return height_; }
		}

		public float angleDegrees
		{
			get { return angleDegrees_; }
		}

		public float GetAngleDegrees(int n)
		{
			float result = angleDegrees_;
			if ( n % 2 == 1)
			{
				result = GeometryHelpers.ModifyAngleDegrees( result, GeometryHelpers.EAngleModifier.Supplementary );
			}
			return result;
        }

		#endregion properties

		#region editor modding

#if UNITY_EDITOR
		// for in-editor modification

		public Vector2[] modBaseVertices = new Vector2[2];
		public float modHeight = 0f;
		public float modAngleDegrees = 0f;

		override protected void CheckIfModded( )
		{
			bool modded = false;
			for (int i = 0; i < 2; i++)
			{
				if (Vector2.Distance( modBaseVertices[i], baseVertices_[i]) > Mathf.Epsilon)
				{
					baseVertices_[i] = modBaseVertices[i];
					modded = true;
				}
			}
			if (!Mathf.Approximately(modHeight, height_))
			{
				height_ = modHeight;
				modded = true;
			}
			if (!Mathf.Approximately(modAngleDegrees, angleDegrees_))
			{
				angleDegrees_ = modAngleDegrees;
				modded = true;
			}
			if (modded)
			{
				if (DEBUG_PARALLELOGRAM)
				{
					Debug.Log( "Modded " + gameObject.name );
				}
				SetMeshDirty( );
			}
		}

		protected override void SetModdingValues( )
		{
#if UNITY_EDITOR
			for (int i = 0; i < 2; i++)
			{
				modBaseVertices[i] = baseVertices_[i];
			}
			modHeight = height_;
			modAngleDegrees = angleDegrees_;
#endif
		}

#endif
		#endregion editor modding
		
		#region geometric properties

		public float BaseLength( )
		{
			return Vector2.Distance( baseVertices_[0], baseVertices_[1] );
		}

		public float Area( )
		{
			return BaseLength( ) * height_;
		}

		#endregion geometric properties

		#region Setup

		public void Init( 
			GeometryFactory gf, Field f, float d, // for base 
			Vector2[] bl, 
			float h, 
			float a, 
			Color c )
		{
			if (bl.Length != 2)
			{
				throw new System.Exception( "bl.Length should be 2, not " + bl.Length.ToString( ) + " when trying to init " + gameObject.name );
			}

			base.Init( gf, f, d, 4 );

			for (int i = 0; i < 2; i++)
			{
				baseVertices_[i] = bl[i];
			}
			SetHeight( h );
			SetAngleDegrees(a);

			decorator = new ElementDecorator_Parallelogram( c, 1f, HandleColourChanged, HandleAlphaChanged );
			decorator.Apply( );

			if (DEBUG_PARALLELOGRAM)
			{
				Debug.Log( "Init() " + this.DebugDescribe( ) );
			}

			SetMeshDirty( );
		}

		public void InitSquare( 
			GeometryFactory gf, Field f, float d, // for base
			Vector2[] bl, 
			Color c )
		{
			if (bl.Length != 2)
			{
				throw new System.Exception( "bl.Length should be 2, not " + bl.Length.ToString( ) + " when trying to init " + gameObject.name );
			}

			Init(gf, f, d, bl, Vector2.Distance( bl[0], bl[1] ), 90f, c );
		}

		private void SetEdgeElements( Vector2[] vertices)
		{
			for (int i = 0; i< 4; i++)
			{
				Element_StraightLine edgeElement = GetEdgeElement( i ) as Element_StraightLine;
				if (edgeElement == null)
				{
					edgeElement = geometryFactory.AddStraightLineToField(
					  field,
					  name + " Edge_" + i.ToString( ),
					  depth-GeometryHelpers.internalLayerSeparation,
					  new Vector2[]
					  {
						vertices[i],
						vertices[modIndex(i+1)]
					  },
					  Element2DBase.defaultEdgeWidth,
					  Color.cyan /* TODO: decorator.colour*/ );
					edgeElement.cachedTransform.SetParent( cachedTransform );
					edgeElement.gameObject.tag = GeometryHelpers.Tag_SubElement;
					SetEdgeElement( i, edgeElement );
				}
				else
				{
					edgeElement.SetEnds( vertices[i], vertices[modIndex( i + 1 )] );
				}                
			}
		}

		public void ChangeBaseline( int n ) // n = 1,2,3 for the 3 other sides (in order retrieved by GetVertices)
		{
			if (n < 0 || n > 3)
			{
				throw new System.Exception( "In ChangeBaseline on " + gameObject.name + ", n should be in [0,3], not " + n );
			}
			if (n == 0)
			{
				if (DEBUG_PARALLELOGRAM)
				{
					Debug.Log( "ChangeBaseline to " + n + " on " + gameObject.name + " does nothing" );
				}
				return;
			}
			Vector2[] vertices = GetVertices( );
			if (n == 2)
			{
				baseVertices_[0] = vertices[2];
				baseVertices_[1] = vertices[3];
			}
			else
			{
				float area = Area( );
				if (n == 1)
				{
					baseVertices_[0] = vertices[1];
					baseVertices_[1] = vertices[2];
				}
				else
				{
					baseVertices_[0] = vertices[3];
					baseVertices_[1] = vertices[0];
				}
				angleDegrees_ = 180f - angleDegrees_;
				height_ = area / BaseLength( );
			}
			SetMeshDirty( );
		}

		#endregion Setup

		#region creation

		protected override void OnClone< T >( T src )
		{
			Element_Parallelogram p = src as Element_Parallelogram;
			if (p == null)
			{
				throw new System.Exception( gameObject.name + ": Parallelograms (" + this.GetType( ).ToString( ) + ") can currently only be cloned from Parallelograms, not " + src.GetType( ).ToString( ) );
			}
			Init( p.geometryFactory, p.field, p.depth, p.baseVertices_.ToArray( ), p.height_, p.angleDegrees_, p.decorator.colour );
		}

		public override ElementBase Clone( string name )
		{
			return this.Clone< Element_Parallelogram >( name );
		}


		#endregion creation

		#region Mesh

		// Computes the 4 vertices from the baseline, height, and angle
		override protected Vector2[] GetVertices( )
		{
			Vector2[] pts = new Vector2[4];

			pts[0] = baseVertices_[0];
			pts[1] = baseVertices_[1];

			// TODO: is this the simplest way...?

			Vector2 baseLine = pts[1] - pts[0];
			Vector3 perp = Vector3.Cross( baseLine, s_normal ).normalized;
			Vector2 perp2 = perp;
			float offset = 0f;

			if (!Mathf.Approximately( angleDegrees_, 90f ))
			{
				offset = height_ / Mathf.Tan( Mathf.Deg2Rad * angleDegrees_ );
			}

			pts[2] = pts[1] + perp2 * height_ - baseLine.normalized * offset;
			pts[3] = pts[0] + perp2 * height_ - baseLine.normalized * offset;

			return pts;
		}

		private static readonly Vector2[] s_uvs = new Vector2[] // UVs for the 4 vertices (for if/when texture is added to the material/shader)
			{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(1f, 1f),
			new Vector2(0f, 1f),
			};

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
			Vector2[] vertices = GetVertices( );

			Mesh mesh = GetMesh( );

			Vector3[] verts = new Vector3[4];
			Vector2[] uvs = new Vector2[4];
			Vector3[] normals = new Vector3[4];

			for (int i = 0; i < 4; i++)
			{
				verts[i] = new Vector3( vertices[i].x, vertices[i].y, depth );
				uvs[i] = s_uvs[i];
				normals[i] = s_normal;
			}

			mesh.vertices = verts;
			mesh.triangles = new int[6] { 0, 1, 2, 2, 3, 0 };
			mesh.uv = uvs;
			mesh.normals = normals;

			mesh.RecalculateBounds( );
			mesh.Optimize( );

			SetEdgeElements( vertices );

			if (DEBUG_PARALLELOGRAM_VERBOSE)
			{
				Debug.Log( "DoAdjustMesh() " + this.DebugDescribe( ) );
			}
		}

		#endregion Mesh

		#region Non-geometrical Appaarance

		protected override void HandleColourChanged( Color c)
		{
			cachedMaterial.SetColor( "_Color", c );
		}

		protected override void HandleAlphaChanged( float a )
		{
			cachedMaterial.SetFloat( "_Alpha", a );
		}

		#endregion Non-geometrical Appaarance

		#region shapeChangers

		public void SetHeight( float h )
		{
			if (!Mathf.Approximately(h, height_))
			{
				height_ = h;
				ShowAllEdgeElements( height_ > Mathf.Epsilon );
				SetMeshDirty( );
			}
		}

		public void SetAngleDegrees( float a )
		{
			if (!Mathf.Approximately(a, angleDegrees_))
			{
				angleDegrees_ = a;
				SetMeshDirty( );
			}
		}

		#endregion shapeChangers

		#region IDebugDescribable

		public void DebugDescribe( System.Text.StringBuilder sb )
		{
			sb.Append( "Parallelogram '" ).Append( gameObject.name ).Append( "': " );
			for (int i = 0; i < baseVertices_.Count; i++)
			{
				sb.Append( baseVertices_[i] ).Append( " " );
			}

			sb.Append( " height=" ).Append( height_ );
			sb.Append( " angle=" ).Append( angleDegrees_ );
			sb.Append( " d=" ).Append( depth );

			sb.Append( " pts=" );
			Vector2[] pts = GetVertices( );
			for (int i = 0; i < pts.Length; i++)
			{
				sb.Append( pts[i].ToString( ) );
			}

		}

		#endregion IDebugDescribable

	}

}
