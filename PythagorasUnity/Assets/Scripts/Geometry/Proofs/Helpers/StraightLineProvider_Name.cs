using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Geometry
{
	public class StraightLineProvider_Name: IStraightLineProvider
	{
		#region private data

		private string lineName_ = "[UNKNOWN POLYGON]";

		#endregion

		#region setup

		public StraightLineProvider_Name( 
			string ln)
		{
			lineName_ = ln;
		}

		#endregion setup

		#region IStraightLineProvider

		public Element_StraightLine GetLine( ElementList elements )
		{
			return elements.GetRequiredElementOfType< Element_StraightLine>( lineName_ );
		}

		#endregion
	}
}

