using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	public class PointProvider_PolygonVertex : IPointProvider
	{
		#region private data

		private IElementProvider polygonProvider_ = null;
		private int pointNumber_ = 0;

		#endregion

		#region setup

		public PointProvider_PolygonVertex( 
			string tn,
			int pn)
		{
			polygonProvider_ = new ElementProvider_Name( tn );
			pointNumber_ = pn;
		}

		public PointProvider_PolygonVertex(
			IElementProvider iep,
			int pn )
		{
			polygonProvider_ = iep;
			pointNumber_ = pn;
		}

		#endregion setup

		#region IPointProvider

		public Vector2 GetPoint( ElementList elements )
		{
			ElementPolygonBase polygon = polygonProvider_.GetElement< ElementPolygonBase >( elements);
			if (pointNumber_ >= polygon.NumVertices )
			{
				Debug.LogWarning( "In PointeProvider_Polygon, pointNumber ( " + pointNumber_ + ") >= numPoints ( " + polygon.NumVertices + " ), adjusting..." );
				pointNumber_ = pointNumber_ % polygon.NumVertices;
			}

			return polygon.GetVertex( pointNumber_ );
		}

		#endregion IPointProvider
	}
}

