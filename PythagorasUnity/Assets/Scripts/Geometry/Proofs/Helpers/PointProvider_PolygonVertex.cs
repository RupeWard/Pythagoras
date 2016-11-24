using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	public class PointProvider_PolygonVertex : IPointProvider
	{
		#region private data

		private string polygonName_ = "[UNKNOWN POLYGON]";
		private int pointNumber_ = 0;

		#endregion

		#region setup

		public PointProvider_PolygonVertex( 
			string tn,
			int pn)
		{
			polygonName_ = tn;
			pointNumber_ = pn;
		}

		#endregion setup

		#region IPointProvider

		public Vector2 GetPoint( ElementList elements )
		{
			ElementPolygonBase polygon = elements.GetRequiredElementOfType< ElementPolygonBase >( polygonName_);
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

