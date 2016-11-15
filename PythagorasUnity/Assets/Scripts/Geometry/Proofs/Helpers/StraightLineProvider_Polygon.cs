using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	public class StraightLineProvider_Polygon : IStraightLineProvider
	{
		#region private data

		private string polygonName_ = "[UNKNOWN POLYGON]";
		private int edgeNumber_ = 0;

		#endregion

		#region setup

		public StraightLineProvider_Polygon( 
			string tn,
			int ln)
		{
			polygonName_ = tn;
			edgeNumber_ = ln;
		}

		#endregion setup

		#region IStraightLineProvider

		public Element_StraightLine GetLine( ElementList elements )
		{
			Element_StraightLine result = null;

			ElementPolygonBase polygon = elements.GetRequiredElementOfType< ElementPolygonBase >( polygonName_);
			result = polygon.GetEdgeElement( edgeNumber_ ) as Element_StraightLine;
			if (result == null)
			{
				Debug.LogError( "Polygon " + polygonName_ + " has no edgeElement " + edgeNumber_ );
			}
			return result;
		}

		#endregion
	}
}

