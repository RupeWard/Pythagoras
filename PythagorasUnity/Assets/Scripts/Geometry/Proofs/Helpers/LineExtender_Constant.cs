using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	public class LineExtender_Constant: ILineExtender
	{
		#region private data

		private string lineName_ = string.Empty;
		private Vector2 extensions = Vector2.zero;

		#endregion

		#region setup

		public LineExtender_Constant( 
			string ln,
			Vector2 e)
		{
			lineName_ = ln;
			extensions = e;
		}

		public LineExtender_Constant(
			Vector2 e )
		{
			extensions = e;
		}


		#endregion setup

		#region ILineExtender

		public bool ExtendLine( ElementList elementList )
		{
			if (lineName_.Length == 0)
			{
				throw new System.Exception( "LineExtender_Constant with name lenght zero can only be used on a specified line, not on an ElementList" );
			}
			Element_StraightLine line = elementList.GetRequiredElementOfType< Element_StraightLine >( lineName_ );			
			return ExtendLine( line );
		}

		public bool ExtendLine( Element_StraightLine line )
		{
			bool result = false;
			Vector2[] ends = line.GetEnds( );

			Vector2[] newEnds = new Vector2[] { ends[0], ends[1] };

            if (extensions.x > 0f)
			{
				newEnds[0] = ends[0] + (ends[0] - ends[1]) * extensions.x;
				result = true;
			}
			if (extensions.y > 0f)
			{
				newEnds[1] = newEnds[1] - (ends[0] - ends[1]) * extensions.y;
				result = true;
			}
			if (result)
			{
				line.SetEnds( newEnds );
			}
			return result;
		}

		#endregion ILineExtender
	}
}

