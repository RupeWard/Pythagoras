using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
	StraightLine element

	Defined by its 2 ends and a width
*/

namespace RJWard.Geometry
{
	public class Element_StraightLine : ElementBase, RJWard.Core.IDebugDescribable
	{
		public static readonly bool DEBUG_STRAIGHTLINE = true;
		public static readonly bool DEBUG_STRAIGHTLINE_VERBOSE = false;

		#region private data

		private List< Vector2 > ends_ = new List< Vector2 >( 2 ) { Vector3.zero, Vector3.zero }; // the vertices
		private float width_ = 0.1f;

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

		public Vector2[] modEnds = new Vector2[2];
		public float modWidth = 0f;

		override protected void CheckIfModded( )
		{
			bool modded = false;
			for (int i = 0; i < 2; i++)
			{
				if (modEnds[i] != ends_[i])
				{
					ends_[i] = modEnds[i];
					modded = true;
				}
			}
			if (modWidth != width_)
			{
				width_ = modWidth;
				modded = true;
			}
			if (modded)
			{
				if (DEBUG_STRAIGHTLINE)
				{
					Debug.Log( "Modded " + gameObject.name );
				}
				SetDirty( );
			}
		}

		protected override void SetModdingValues( )
		{
#if UNITY_EDITOR
			for (int i = 0; i < 2; i++)
			{
				modEnds[i] = ends_[i];
			}
			modWidth = width_;
#endif
		}

#endif

		#endregion in-editor modding 

		#region Setup

		public void Init( Field f, float d, Vector2[] es, float w, Color c )
		{
			if (es.Length != 2)
			{
				throw new System.Exception( "es.Length should be 2, not " + es.Length + " on trying to Init " + gameObject.name );
			}

			base.Init( f, d );

			for (int i = 0; i < 2; i++)
			{
				ends_[i] = es[i];
			}
			width_ = w;

			if (DEBUG_STRAIGHTLINE)
			{
				Debug.Log( "Init() " + this.DebugDescribe( ) );
			}

			SetColour( c );
			SetDirty( );
		}

		Vector2 GetDirection( )
		{
			return ends_[1] - ends_[0];
		}

		// Computes the 4 vertices from the baseline, height, and angle
		private Vector2[] GetVertices( )
		{
			Vector2[] pts = new Vector2[4];

			Vector2 direction = GetDirection( );

			Vector3 perp = Vector3.Cross( direction, s_normal ).normalized;
			Vector2 perp2 = perp;

			float offset = 0.5f * width_;
			pts[0] = ends_[0] + offset * perp2;
			pts[1] = ends_[1] + offset * perp2;
			pts[2] = ends_[1] - offset * perp2;
			pts[3] = ends_[0] - offset * perp2;

			return pts;
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
			Init( s.field, s.depth, s.ends_.ToArray( ), s.width_, s.colour );
		}

		public override ElementBase Clone( string name )
		{
			return this.Clone< Element_StraightLine >( name );
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
			for (int i = 0; i < 2; i++)
			{
				modEnds[i] = ends_[i];
			}
			modWidth = width_;
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

		#endregion geometry helpers

		#region Non-geometrical Appaarance

		override protected void HandleColourChanged( )
		{
			cachedMaterial.SetColor( "_Color", colour );
		}

		override protected void HandleAlphaChanged(  )
		{
			cachedMaterial.SetFloat( "_Alpha", alpha );
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

			sb.Append( " width=" ).Append( width_ );
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
