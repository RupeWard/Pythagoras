using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	/*
		Get angle from parallelogram

		Parallelogram is defined in terms of angle so more efficient to use this than ploygon version 
	*/
	public class AngleProvider_Parallelogram : IAngleProvider, IElementProvider
	{
		#region private data

		private IElementProvider parallelogramProvider_ = null;
		private int angleNumber_ = 0;
		private GeometryHelpers.EAngleModifier eModifier_ = GeometryHelpers.EAngleModifier.Raw;

		#endregion

		#region setup

		public AngleProvider_Parallelogram ( 
            string pn,
			int an,
			GeometryHelpers.EAngleModifier eam)
		{
			parallelogramProvider_ = new ElementProvider_Name( pn );
			angleNumber_ = an;
			eModifier_ = eam;
		}

		public AngleProvider_Parallelogram(
			IElementProvider iep,
			int an,
			GeometryHelpers.EAngleModifier eam )
		{
			parallelogramProvider_ = iep;
			angleNumber_ = an;
			eModifier_ = eam;
		}


		#endregion setup

		#region IAngleProvider

		public float GetAngle( ElementList elements )
		{
			float result = 0f;

			Element_Parallelogram parallelogram = parallelogramProvider_.GetElement< Element_Parallelogram >( elements );
			float rawAngle = parallelogram.GetAngleDegrees(angleNumber_);
			result = GeometryHelpers.ModifyAngleDegrees( rawAngle, eModifier_ );
			return result;
		}

		#endregion

		#region IElementProvider

		public T GetElement<T>( ElementList elements ) where T : ElementBase
		{
			return parallelogramProvider_.GetElement<Element_Parallelogram>( elements ).GetAngleElement( angleNumber_) as T;
		}

		#endregion IElementProvider

	}
}

