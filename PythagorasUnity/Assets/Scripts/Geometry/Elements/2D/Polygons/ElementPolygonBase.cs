﻿using UnityEngine;
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

		private Element1DBase[] edgeElements_;
		private bool[] showEdges_;

		private Element_Sector[] angleElements_;
		private bool[] showAngles_;

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

		public Element1DBase GetEdgeElement(int n)
		{
			return edgeElements_[ modIndex(n) ];
		}

		protected void SetEdgeElement(int n, Element1DBase e )
		{
			n = modIndex( n );
			if (edgeElements_[n] != null)
			{
				throw new System.Exception( "Element " + name + " already has an edge #" + n +", destroying");
			}
			edgeElements_[n] = e;
			ShowEdgeElement( n, showEdges_[n] );
		}

		public void ShowEdgeElement(int n, bool show)
		{
			Element1DBase edgeElement = GetEdgeElement( n );
			if (edgeElement != null)
			{
				edgeElement.gameObject.SetActive( show );
			}
		}

		public void ShowEdgeElement(int n)
		{
			ShowEdgeElement( n, true );
		}

		public void HideEdgeElement( int n )
		{
			ShowEdgeElement( n, false );
		}

		public void ShowAllEdgeElements(bool show)
		{
			for (int i = 0; i < numVertices_; i++)
			{
				if (edgeElements_[i] != null)
				{
					edgeElements_[i].gameObject.SetActive( show );
				}
			}
		}

		public void ShowAllEdgeElements()
		{
			ShowAllEdgeElements( true );
		}

		public void HideEdgeElements()
		{
			ShowAllEdgeElements( false );
		}

		public void DestroyEdgeElement(int n)
		{
			if (edgeElements_[n] != null)
			{
				GameObject.Destroy( edgeElements_[n].gameObject );
				edgeElements_[n] = null;
			}
		}

		public void DestroyAllEdgeElements()
		{
			for (int i = 0; i < numVertices_; i++)
			{
				DestroyEdgeElement( i );
			}
		}

		void SetShowEdgeElement(int n, bool b)
		{
			if (b != showEdges_[n])
			{
				showEdges_[n] = b;
				ShowEdgeElement( n, b );
			}
		}

		void SetShowEdgeElement( int n )
		{
			SetShowEdgeElement( n, true );
		}

		void SetHideEdgeElement( int n )
		{
			SetShowEdgeElement( n, false );
		}

		#endregion edges

		#region angles

		public Element_Sector GetAngleElement( int n )
		{
			return angleElements_[modIndex( n )];
		}

		protected void SetAngleElement( int n, Element_Sector a )
		{
			n = modIndex( n );
			if (angleElements_[n] != null)
			{
				throw new System.Exception( "Element " + name + " already has an angle #" + n + ", destroying" );
			}
			angleElements_[n] = a;
			ShowAngleElement( n, showAngles_[n] );
		}

		public void ShowAngleElement( int n, bool show )
		{
			Element_Sector angleElement = GetAngleElement( n );
			if (angleElement != null)
			{
				angleElement.gameObject.SetActive( show );
			}
		}

		public void ShowAngleElement( int n )
		{
			ShowAngleElement( n, true );
		}

		public void HideAngleElement( int n )
		{
			ShowAngleElement( n, false );
		}

		public void ShowAllAngleElements( bool show )
		{
			for (int i = 0; i < numVertices_; i++)
			{
				if (angleElements_[i] != null)
				{
					angleElements_[i].gameObject.SetActive( show );
				}
			}
		}

		public void ShowAllAngleElements( )
		{
			ShowAllAngleElements( true );
		}

		public void HideAngleElements( )
		{
			ShowAllAngleElements( false );
		}

		public void DestroyAngleElement(int n)
		{
			if (angleElements_[n] != null)
			{
				GameObject.Destroy( angleElements_[n].gameObject );
				angleElements_[n] = null;
			}
		}

		public void DestroyAllAngleElements( )
		{
			for (int i = 0; i < numVertices_; i++)
			{
				DestroyAngleElement( i );
			}
		}

		// TODO whether this is called should depend only on whether the edges are changed
		protected void CreateAngleElement(int i)
		{
			Element_StraightLine[] lines = new Element_StraightLine[2];

			lines[0] = GetEdgeElement( i ) as Element_StraightLine;
			lines[1] = GetEdgeElement( modIndex( i + 1 ) ) as Element_StraightLine;

			if (lines[0] != null && lines[1] != null)
			{
				Element_Sector angleElement = GetAngleElement( i );
				if (angleElement != null)
				{
					angleElement.SetContainingLines( lines );
				}
				else
				{
					angleElement = geometryFactory.AddSectorBetweenLines(
						name + " Angle_" + i.ToString( ),
						GeometryHelpers.internalLayerSeparation,
						lines,
						0.2f,
						Color.red );
					angleElement.cachedTransform.SetParent( cachedTransform );
					angleElement.gameObject.tag = GeometryHelpers.Tag_SubElement;
					angleElement.SetAngleMarker( );
					SetAngleElement( i, angleElement );
				}
			}
			else
			{
				if (DEBUG_POLYGONBASE)
				{
					Debug.LogWarning( "Null line" );
				}
				if (GetAngleElement( i )!= null)
				{
					DestroyAngleElement( i );
				}
			}
		}

		protected void SetAllAngleElements()
		{
			for (int i = 0; i < numVertices_; i++)
			{
				CreateAngleElement( i );
			}
		}

		void SetShowAngleElement( int n, bool b )
		{
			if (b != showAngles_[n])
			{
				showAngles_[n] = b;
				ShowAngleElement( n, b );
			}
		}

		void SetShowAngleElement( int n )
		{
			SetShowAngleElement( n, true );
		}

		void SetHideAngleElement( int n )
		{
			SetShowAngleElement( n, false );
		}

		#endregion angles

		#region subelements

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

		public void ShowAllSubElements(bool b)
		{
			ShowAllAngleElements( b );
			ShowAllEdgeElements( b );
		}

		public void ShowAllSubElements( )
		{
			ShowAllSubElements( true );
		}

		public void HideAllSubElements( )
		{
			ShowAllSubElements( false);
		}

		#endregion subelements

		#region Non-geometrical Appaarance

		public override void SetAlpha( float a )
		{
			base.SetAlpha( a );
			for (int i = 0; i < NumVertices; i++)
			{
				Element1DBase edgeElement = GetEdgeElement( i ); 
				if (edgeElement != null)
				{
					edgeElement.SetAlpha( a );
				}

				Element_Sector angleElement = GetAngleElement( i );
				if (angleElement != null)
				{
					angleElement.SetAlpha( a );
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
			edgeElements_ = new Element1DBase[numVertices_];
			showEdges_ = new bool[numVertices_];
			angleElements_ = new Element_Sector[numVertices_];
			showAngles_ = new bool[numVertices_];
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
			e.FindAndDestroyAllSubElements( );
			return clone;
		}

		#endregion Creation

	}
}
