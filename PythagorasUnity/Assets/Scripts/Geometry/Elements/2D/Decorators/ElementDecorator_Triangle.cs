using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	// NOTE: Currently adds nothing to base but probably will later 
	public class ElementDecorator_Triangle : ElementDecorator2DBase
	{
		#region Setup

		public ElementDecorator_Triangle( 
			Color c, float a, System.Action< Color > cca, System.Action< float > aca)
			: base(c,a,cca,aca)
		{
		}

		#endregion Setup

		#region setters

		#endregion setters
	}
}
