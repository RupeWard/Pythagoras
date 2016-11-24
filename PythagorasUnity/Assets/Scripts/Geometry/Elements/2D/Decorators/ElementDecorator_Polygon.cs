using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	public class ElementDecorator_Polygon : ElementDecorator2DBase
	{

		#region subelement decorators

		private ElementDecorator_Circle defaultAngleDecorator_ = null;
		public ElementDecorator_Circle defaultAngleDecorator
		{
			get
			{
				if (defaultAngleDecorator_ == null)
				{
					defaultAngleDecorator_ = new ElementDecorator_Circle( colour, alpha );
				}
				return defaultAngleDecorator_;
			}
			set
			{
//				Debug.LogError( "Assigning to non-null defaultAngleDecorator" );
				defaultAngleDecorator_ = value;
			}
		}

		#endregion subelement decorators

		#region Setup

		public ElementDecorator_Polygon( 
			Color c, float a, System.Action< Color > cca, System.Action< float > aca)
			: base(c,a,cca,aca)
		{
		}

		#endregion Setup

		#region setters

		#endregion setters

	}
}
