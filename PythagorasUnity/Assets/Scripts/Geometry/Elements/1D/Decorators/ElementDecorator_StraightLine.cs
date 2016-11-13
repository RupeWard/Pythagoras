using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	public class ElementDecorator_StraightLine : ElementDecorator1DBase
	{
		#region Setup

		public ElementDecorator_StraightLine( 
			Color c, float a, System.Action< Color > cca, System.Action< float > aca, float w, System.Action<float> wca )
			: base(c,a,cca,aca, w, wca)
		{
		}

		#endregion Setup

	}
}
