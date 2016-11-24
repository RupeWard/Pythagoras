using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	// NOTE: Currently adds nothing to base but probably will later 
	// Also used for Sector
	public class ElementDecorator_Circle : ElementDecorator2DBase
	{
		#region Setup

		public ElementDecorator_Circle(
			Color c, float a, System.Action< Color > cca, System.Action< float > aca)
			: base(c,a,cca,aca)
		{
		}

		public ElementDecorator_Circle(
			Color c, float a  )
			: base( c, a )
		{
		}

		#endregion Setup

		#region setters

		#endregion setters
	}
}
