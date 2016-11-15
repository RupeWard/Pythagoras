using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	/*
		Get angle from parallelogram

		Parallelogram is defined in terms of angle so more efficient to use this than ploygon version 
	*/
	public class AngleProvider_Parallelogram : IAngleProvider
	{ 
		#region private data

	private string parallelogramName_ = "[UNKNOWN PARALLELOGRAM]";
		private int angleNumber_ = 0;
		private GeometryHelpers.EAngleModifier eModifier_ = GeometryHelpers.EAngleModifier.Raw;

		#endregion

		#region setup

		public AngleProvider_Parallelogram ( 
            string pn,
			int an,
			GeometryHelpers.EAngleModifier eam)
		{
			parallelogramName_ = pn;
			angleNumber_ = an;
			eModifier_ = eam;
		}

		#endregion setup

		#region IAngleProvider

		public float GetAngle( ElementList elements )
		{
			float result = 0f;

			Element_Parallelogram parallelogram = elements.GetRequiredElementOfType< Element_Parallelogram >( parallelogramName_ );
			float rawAngle = parallelogram.GetAngleDegrees(angleNumber_);
			result = GeometryHelpers.ModifyAngleDegrees( rawAngle, eModifier_ );
			return result;
		}

		#endregion
	}
}

