using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	/*
		Base class for polygons (2D elements with straight edges)
	*/
	abstract public class ElementPolygonBase : Element2DBase
	{
		static public bool DEBUG_POLYGONBASE = true;

		#region private data 

		private int numVertices_ = 0;

		private Element1DBase[] edges_;
		 
		#endregion private data 

		#region properties

		public int NumVertices
		{
			get { return numVertices_;  }
		}

		protected int modIndex(int n)
		{
			if (n < 0 || n > numVertices_ - 1)
			{
				while (n < 0)
				{
					n += numVertices_;
				}
				while (n > numVertices_ - 1)
				{
					n -= numVertices_;
				}
			}
			return n;
		}
		#endregion properties

		#region vertices

		// Must return an array of length numVertices_
		abstract protected Vector2[] GetVertices( );

		public Vector2 GetVertex( int n )
		{
			return GetVertices()[modIndex( n)];
		}

		#endregion vertices

		#region edges

		public Element1DBase GetEdge(int n)
		{
			return edges_[ modIndex(n) ];
		}

		protected void SetEdge(int n, Element1DBase e )
		{
			n = modIndex( n );
			if (edges_[n] != null)
			{
				throw new System.Exception( "Element " + name + " already has an edge #" + n +", destroying");
			}
			edges_[n] = e;
		}

		public void ShowEdge(int n, bool show)
		{
			GetEdge( n ).gameObject.SetActive( show );
		}

		public void ShowEdge(int n)
		{
			ShowEdge( n, true );
		}

		public void HideEdge( int n )
		{
			ShowEdge( n, false );
		}

		public void ShowAllEdges(bool show)
		{
			for (int i = 0; i < numVertices_; i++)
			{
				if (edges_[i] != null)
				{
					edges_[i].gameObject.SetActive( show );
				}
			}
		}

		public void ShowAllEdges()
		{
			ShowAllEdges( true );
		}

		public void HideEdges()
		{
			ShowAllEdges( false );
		}

		private void DestroyAllEdges()
		{
			for (int i = 0; i < numVertices_; i++)
			{
				if (edges_[i] != null)
				{
					GameObject.Destroy( edges_[i].gameObject );
					edges_[i] = null;
				}
			}
		}

		private void FindAndDestroyAllSubElements( )
		{
			foreach (Transform tr in cachedTransform)
			{
				if (tr.tag == GeometryHelpers.Tag_SubElement)
				{
					GameObject.Destroy( tr.gameObject );
				}
			}
		}

		#endregion edges

		#region Non-geometrical Appaarance

		public override void SetAlpha( float a )
		{
			base.SetAlpha( a );
			for (int i = 0; i < NumVertices; i++)
			{
				Element1DBase edge = GetEdge( i ); // may (should?) be possible to do this wthout needing to check ... timing!
				if (edge != null)
				{
					edge.SetAlpha( a );
				}
			}
		}

		#endregion Non-geometrical Appaarance

		#region Setup

		// Call this from derived classes' Init functions
		protected void Init( GeometryFactory gf, Field f, float d, int n )
		{
			base.Init( gf, f, d );
			numVertices_ = n;
			edges_ = new Element1DBase[numVertices_];
		}

		protected override void Init( GeometryFactory gf, Field f, float d )
		{
			throw new System.InvalidOperationException( "Shouldn't be calling this, derived classes must use ElementPolygonBase.Init( GeometryFactory gf, Field f, float d, int n ) so as to set numVertices" );
		}

		#endregion Setup

		#region Creation

		override public T Clone<T>( string name, float d )
		{
			T clone = base.Clone<T>( name, d );
			ElementPolygonBase e = clone as ElementPolygonBase;
			if (e == null)
			{
				throw new System.Exception( "Cloning " + name + " doesn't give an ElementPolygonBase" );
			}
			// Cloning process instantiates from the source sobject so edges get copied but not set up. Need to find and destroy them.
			// TODO possible just destroy all children on cloning? Will there ever be subobjects that we want to keep?
			e.FindAndDestroyAllSubElements( );
			return clone;
		}

		#endregion Creation

	}
}
