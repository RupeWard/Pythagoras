using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	// NOTE: Currently adds nothing to base but probably will later 
	// Also used for Curve
	public class ElementDecorator_StraightLine : ElementDecorator1DBase
	{
		#region Setup

		public ElementDecorator_StraightLine( 
			Color c, float a, System.Action< Color > cca, System.Action< float > aca, float w, System.Action<float> wca )
			: base(c,a,cca,aca, w, wca)
		{
		}

		public ElementDecorator_StraightLine(
			Color c, float a, float w )
			: base( c, a, w )
		{
		}

		#endregion Setup

	}
}
