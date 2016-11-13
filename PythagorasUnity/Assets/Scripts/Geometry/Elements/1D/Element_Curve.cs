﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
	Curve element

	Consists of a number of straightline elements

	Defined by list of points and whether it's closed
*/

namespace RJWard.Geometry
{
	public class Element_Curve: Element1DBase, RJWard.Core.IDebugDescribable
	{
		public static readonly bool DEBUG_CURVE = true;
		public static readonly bool DEBUG_CURVE_VERBOSE = false;

		#region private data

		private List< Element_StraightLine > segments_ = new List< Element_StraightLine >( ); // the segments

		private List<Vector2> points_ = new List<Vector2>( );	// the points
		bool closed_ = true;									// extra segment created if closed

		#endregion private data

		#region in-editor modding 

#if UNITY_EDITOR
		// for in-editor modification

		public float modWidth = 0f;

		override protected void CheckIfModded( )
		{
			bool modded = false;
			if (modWidth != decorator1D.width)
			{
				decorator1D.width = modWidth;
				modded = true;
			}
			if (modded)
			{
				if (DEBUG_CURVE)
				{
					Debug.Log( "Modded " + gameObject.name );
				}
				SetMeshDirty( );
			}
		}

		protected override void SetModdingValues( )
		{
#if UNITY_EDITOR
			modWidth = decorator1D.width;
#endif
		}

#endif

		#endregion in-editor modding 

		#region Setup

		public void Init( 
			GeometryFactory gf, Field f, float d,  // for base
			List< Vector2 > pts, 
			bool cl, 
			float w, 
			Color c )
		{
			if (pts.Count < 2)
			{
				throw new System.Exception( "vs.Length should be > 1, not " + pts.Count + " on trying to Init " + gameObject.name );
			}

			base.Init( gf, f, d );

			points_ = new List<Vector2>( pts );

			decorator = new ElementDecorator_StraightLine( c, 1f, HandleColourChanged, HandleAlphaChanged, w, HandleWidthChanged );

			if (DEBUG_CURVE)
			{
				Debug.Log( "Init() " + this.DebugDescribe( ) );
			}

			SetMeshDirty( );
		}

		public void InitOpen( GeometryFactory gf, Field f, float d, List<Vector2> pts, bool cl, float w, Color c )
		{
			Init( gf, f, d, pts, false, w, c );
		}

		public void InitClosed( GeometryFactory gf, Field f, float d, List<Vector2> pts, bool cl, float w, Color c )
		{
			Init( gf, f, d, pts, true, w, c );
		}

		#endregion Setup

		#region  creation

		protected override void OnClone< T >( T src )
		{
			Element_Curve c = src as Element_Curve;
			if (c == null)
			{
				throw new System.Exception( gameObject.name + ": Curves can currently only be cloned as Curves" );
			}
			Init( c.geometryFactory, c.field, c.depth, c.points_, c.closed_, c.decorator1D.width, c.decorator1D.colour );
		}

		public override ElementBase Clone( string name )
		{
			return this.Clone< Element_Curve >( name );
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

			// TODO only destroy/create as needed
			for (int i = 0; i < segments_.Count; i++)
			{
				if (segments_[i] != null)
				{
					GameObject.Destroy( segments_[i].gameObject );
				}
			}
			segments_.Clear( );

			Vector2[] segmentEnds = new Vector2[2];

			for (int i=0; i< points_.Count - 1; i++)
			{
				segmentEnds[0] = points_[i];
				segmentEnds[1] = points_[i + 1];

				Element_StraightLine segment = geometryFactory.AddLineSegmentToCurve( this, name + " Segment_" + i, segmentEnds );
				segments_.Add( segment );
			}

			if (closed_)
			{
				segmentEnds[0] = segmentEnds[1];
				segmentEnds[1] = points_[0];

				Element_StraightLine segment = geometryFactory.AddLineSegmentToCurve( this, name + " Segment_" + points_.Count, segmentEnds );
				segments_.Add( segment );
			}

			if (DEBUG_CURVE_VERBOSE)
			{
				Debug.Log( "DoAdjustMesh() " + this.DebugDescribe( ) );
			}
		}

		#endregion Mesh

		#region geometry helpers

		protected void HandleWidthChanged( float w)
		{
			SetMeshDirty( );
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
			sb.Append( "Curve '" ).Append( gameObject.name ).Append( "': " );
			sb.Append( " N = " ).Append( points_.Count );
			sb.Append( (closed_)?(" (closed)"):(" (open)"));
			sb.Append( " width=" ).Append( decorator1D.width );
			sb.Append( " d=" ).Append( depth );

		}

		#endregion IDebugDescribable
	}

}