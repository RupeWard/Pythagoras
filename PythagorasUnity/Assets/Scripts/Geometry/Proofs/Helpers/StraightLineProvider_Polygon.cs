using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Geometry
{
	public class StraightLineProvider_Polygon : IStraightLineProvider
	{
		#region private data

		private string polygonName_ = "[UNKNOWN POLYGON]";
		private int edgeNumber_ = 0;
		private List<ILineExtender> lineExtenders_ = new List<ILineExtender>( );

		#endregion

		#region setup

		public StraightLineProvider_Polygon( 
			string pn,
			int en)
		{
			Init( pn, en );
		}

		public StraightLineProvider_Polygon(
			string pn,
			int en,
			List< ILineExtender > le)
		{
			Init( pn, en );
			lineExtenders_.AddRange( le );
		}

		public StraightLineProvider_Polygon(
			string pn,
			int en,
			ILineExtender le )
		{
			Init( pn, en );
			lineExtenders_.Add( le );
		}

		private void Init( string pn,int en)
		{
			polygonName_ = pn;
			edgeNumber_ = en;
		}

		#endregion setup

		#region IStraightLineProvider

		public Element_StraightLine GetLine( ElementList elements )
		{
			Element_StraightLine line = null;

			ElementPolygonBase polygon = elements.GetRequiredElementOfType< ElementPolygonBase >( polygonName_);
			line = polygon.GetEdgeElement( edgeNumber_ ) as Element_StraightLine;
			if (line == null)
			{
				Debug.LogError( "Polygon " + polygonName_ + " has no edgeElement " + edgeNumber_ );
			}
			else
			{
				for (int i = 0; i < lineExtenders_.Count; i++)
				{
					lineExtenders_[i].ExtendLine( elements, line );
				}
			}
			return line;
		}

		#endregion
	}
}

