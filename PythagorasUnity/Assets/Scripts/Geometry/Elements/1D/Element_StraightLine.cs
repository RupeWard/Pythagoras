using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
	StraightLine element

	Defined by its 2 ends 
*/

namespace RJWard.Geometry
{
	public class Element_StraightLine: Element1DBase, RJWard.Core.IDebugDescribable
	{
		public static readonly bool DEBUG_STRAIGHTLINE = false;
		public static readonly bool DEBUG_STRAIGHTLINE_VERBOSE = false;

		#region private data

		private List< Vector2 > ends_ = new List< Vector2 >( ) { Vector3.zero, Vector3.zero }; // the vertices

		#endregion private data

		#region properties

		// These return COPIES
		public Vector2[] GetEnds(bool reverse)
		{
			Vector2[] result = new Vector2[2];
			if (reverse)
			{
				result[0] = ends_[0];
				result[1] = ends_[1];
			}
			else
			{
				result[0] = ends_[1];
				result[1] = ends_[0];
			}
			return result;
		}

		public Vector2[] GetEnds( )
		{
			return GetEnds( false );
		}

		public Vector2[] GetEndsReversed( )
		{
			return GetEnds( true );
		}

		public float length
		{
			get { return Vector2.Distance( ends_[0], ends_[1] );  }
		}

		#endregion properties

		#region in-editor modding 

#if UNITY_EDITOR
		// for in-editor modification

		public Vector2[] modEnds = new Vector2[] { new Vector2( ), new Vector2( ) };
		public float modWidth = 0f;

		override protected void CheckIfModded( )
		{
			for (int i = 0; i < 2; i++)
			{
				if (Vector2.Distance( modEnds[i], ends_[i]) > Mathf.Epsilon)
				{
					if (SetEnd( i, modEnds[i] ))
					{
						if (DEBUG_STRAIGHTLINE)
						{
							Debug.Log( "Modded " + gameObject.name +" end #"+i);
						}
					}
				}
			}
			if (!Mathf.Approximately( modWidth, decorator1D.width))
			{
				if (SetWidth(modWidth))
				{
					if (DEBUG_STRAIGHTLINE)
					{
						Debug.Log( "Modded " + gameObject.name + " width" );
					}
				}
			}
		}

		protected override void SetModdingValues( )
		{
#if UNITY_EDITOR
			for (int i = 0; i < 2; i++)
			{
				modEnds[i] = ends_[i];
			}
			modWidth = decorator1D.width;
#endif
		}

#endif

		#endregion in-editor modding 

		#region Setup

		public void Init( GeometryFactory gf, Field f, float d, Vector2[] es, float w, Color c )
		{
			InitHelper( gf, f, d, es, null );

			decorator = new ElementDecorator_StraightLine( c, 1f, HandleColourChanged, HandleAlphaChanged, w, HandleWidthChanged );
			decorator.Apply( );

			if (DEBUG_STRAIGHTLINE)
			{
				Debug.Log( "Init() " + this.DebugDescribe( ) );
			}

			SetMeshDirty( );
		}

		// For when we want to use a shared decorator
		public void Init( GeometryFactory gf, Field f, float d, Vector2[] es, ElementDecorator1DBase dec, Material mat )
		{
			InitHelper( gf, f, d, es, mat );

			if (dec == null)
			{
				Debug.LogWarning( "null decorator passed to init" );
			}
			decorator = dec;
//			decorator1D.AddActions( HandleColourChanged, HandleAlphaChanged, HandleWidthChanged );
			decorator1D.AddActions( null, null, HandleWidthChanged ); // no colour/alpha changers needed as handled in full by owner (eg curve)

			if (DEBUG_STRAIGHTLINE)
			{
				Debug.Log( "Init() with decorator " + this.DebugDescribe( ) );
			}

			SetMeshDirty( );
		}

		private void InitHelper(GeometryFactory gf, Field f, float d, Vector2[] es, Material mat)
		{
			if (es.Length != 2)
			{
				throw new System.Exception( "es.Length should be 2, not " + es.Length + " on trying to Init " + gameObject.name );
			}

			base.Init( gf, f, d, mat );

			SetEnds( es );
		}

		public Vector2 GetDirection( )
		{
			return ends_[1] - ends_[0];
		}

		/*
		public float GetAngleDegrees()
		{
			Vector2 direction = GetDirection( );
			return Mathf.Rad2Deg * Mathf.Atan2( direction.y, direction.x );
		}
		*/

		// Computes the 4 vertices from the ends and width
		private Vector2[] GetVertices( )
		{
			Vector2[] pts = new Vector2[4];

			Vector2 direction = GetDirection( );

			Vector3 perp = Vector3.Cross( direction, s_normal ).normalized;
			Vector2 perp2 = perp;

			float offset = 0.5f * decorator1D.width;

			pts[0] = ends_[0] + offset * perp2;
			pts[1] = ends_[1] + offset * perp2;
			pts[2] = ends_[1] - offset * perp2;
			pts[3] = ends_[0] - offset * perp2;

			return pts;
		}

		public bool SetEnds(Vector2[] vs)
		{
			if (vs.Length != 2)
			{
				throw new System.ArgumentException( "SetEnds must be passed 2 ends not " + vs.Length );
			}
			return SetEnds( vs[0], vs[1] );
		}

		public bool SetEnds(Vector2 v0, Vector2 v1)
		{
			bool changed = false;
			changed |= SetEnd( 0, v0 );
			changed |= SetEnd( 1, v1 );
			return changed;
		}

		public bool SetEnd(int n, Vector2 v)
		{
			if (n < 0 || n > 1)
			{
				throw new System.ArgumentException( "Can't set end " + n );
			}
			bool changed = false;
			if (Vector2.Distance(ends_[n], v) > Mathf.Epsilon)
			{
				ends_[n] = v;
				changed = true;
				SetMeshDirty( );
			}
			return changed;
		}

		public bool SetWidth( float f )
		{
			bool changed = false;
			if (f < 0)
			{
				if (DEBUG_STRAIGHTLINE)
				{
					Debug.LogWarning( "SetWidth given width < 0, adjusting" );
				}
				f = 0;
			}
			if (!Mathf.Approximately(f, decorator1D.width))
			{
				decorator1D.width = f;
				changed = true;
				// No need to set mesh dirty, this is handled by decorator 
			}
			return changed;
		}

		#endregion Setup

		#region  creation

		protected override void OnClone< T >( T src )
		{
			Element_StraightLine s = src as Element_StraightLine;
			if (s == null)
			{
				throw new System.Exception( gameObject.name + ": StraightLines can currently only be cloned as StraightLines" );
			}
			Init( s.geometryFactory, s.field, s.depth, s.ends_.ToArray( ), s.decorator1D.width, s.decorator1D.colour );
		}

		public override ElementBase Clone( string name )
		{
			return this.Clone< Element_StraightLine >( name );
		}

		private void OnDestroy()
		{
			// in case it's a shared decorator
			// decorator1D.RemoveActions( HandleColourChanged, HandleAlphaChanged, HandleWidthChanged );
			if (decorator1D != null)
			{
				decorator1D.RemoveActions( null, null, HandleWidthChanged );
			}
		}

		#endregion creation

		#region Mesh

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

			Vector2[] pts = GetVertices( );

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

			if (DEBUG_STRAIGHTLINE_VERBOSE)
			{
				Debug.Log( "DoAdjustMesh() " + this.DebugDescribe( ) );
			}
		}

		#endregion Mesh

		#region geometry helpers

		protected void HandleWidthChanged( float w)
		{
			if (DEBUG_STRAIGHTLINE_VERBOSE)
			{
				if (decorator != null)
				{
					Debug.Log( name + ": Width changed to " + w + " in decorator [" + decorator.DebugDescribe( ) + "]" );
				}
				else
				{
					Debug.LogWarning( name + ": Width changed to " + w + " but null decorator " );
				}
			}
			SetMeshDirty( );
		}

		public Vector2 GetEndFurthestFrom(Vector2 v)
		{
			Vector2 result = ends_[0];
			if (Vector2.Distance(ends_[1], v ) > Vector2.Distance(ends_[0], v))
			{
				result = ends_[1];
			}
			return result;
		}

		static public bool Intersection( Element_StraightLine line0, Element_StraightLine line1, ref Vector2 intersection)
		{
			return GeometryHelpers.LineIntersectionPoint( line0.GetEnds(), line1.GetEnds(), ref intersection );
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
			sb.Append( "StraightLine '" ).Append( gameObject.name ).Append( "': " );
			for (int i = 0; i < ends_.Count; i++)
			{
				sb.Append( ends_[i] ).Append( " " );
			}

			sb.Append( " width=" ).Append( decorator1D.width );
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
