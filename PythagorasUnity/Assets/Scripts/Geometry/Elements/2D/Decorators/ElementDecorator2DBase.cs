using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	abstract public class ElementDecorator2DBase : ElementDecoratorBase
	{
		#region Setup

		protected ElementDecorator2DBase( 
			Color c, float a, System.Action< Color > cca, System.Action< float > aca)
			: base(c,a,cca,aca)
		{
		}

		#endregion Setup

		#region setters

		#endregion setters

	}
}
