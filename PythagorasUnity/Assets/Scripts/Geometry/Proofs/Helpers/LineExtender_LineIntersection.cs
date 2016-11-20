using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	public class LineExtender_LineIntersection: ILineExtender
	{
		#region private data

		private string lineName_ = string.Empty;
		private IStraightLineProvider intersectingLineProvider_ = null;

		#endregion

		#region setup

		public LineExtender_LineIntersection( 
			string ln,
			IStraightLineProvider ilp)
		{
			lineName_ = ln;
			intersectingLineProvider_ = ilp;
		}

		public LineExtender_LineIntersection(
			IStraightLineProvider ilp )
		{
			intersectingLineProvider_ = ilp;
		}


		#endregion setup

		#region ILineExtender

		public bool ExtendLine( ElementList elementList )
		{
			if (lineName_.Length == 0)
			{
				throw new System.Exception( "LineExtender_LineIntersection with name lenght zero can only be used on a specified line, not on an ElementList" );
			}
			Element_StraightLine line = elementList.GetRequiredElementOfType< Element_StraightLine >( lineName_ );			
			return ExtendLine( elementList, line );
		}

		public bool ExtendLine( ElementList elementList, Element_StraightLine line )
		{
			bool result = false;

			Element_StraightLine intersectingLine = intersectingLineProvider_.GetLine( elementList );
			Vector2 intersection = Vector2.zero;
			if ( StraightLineFormula.GetIntersection(line.GetFormula(), intersectingLine.GetFormula(), ref intersection))
			{
				Vector2[] ends = line.GetEnds( );
				if (Vector2.Distance( ends[0], intersection) < Vector2.Distance(ends[1], intersection))
				{
					ends[0] = intersection;
				}
				else
				{
					ends[1] = intersection;
				}
				line.SetEnds( ends );
				result = true;
			}
			return result;
		}

		#endregion ILineExtender
	}
}

