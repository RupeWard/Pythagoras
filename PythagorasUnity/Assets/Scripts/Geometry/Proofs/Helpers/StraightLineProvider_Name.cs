using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Geometry
{
	public class StraightLineProvider_Name: ElementProvider_Name, IStraightLineProvider
	{
		#region setup

		public StraightLineProvider_Name( string ln) : base( ln )
		{
		}

		#endregion setup

		#region IStraightLineProvider

		public Element_StraightLine GetLine( ElementList elements )
		{
			return elements.GetRequiredElementOfType< Element_StraightLine>( elementName);
		}

		#endregion
	}
}

