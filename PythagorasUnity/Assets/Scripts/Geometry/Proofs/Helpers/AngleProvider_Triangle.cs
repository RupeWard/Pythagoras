using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	public class AngleProvider_Triangle : IAngleProvider
	{
		public enum EAngleModifier
		{
			Raw,
			Complementary, // 90 - x
			Supplementary, // 180 - x
			Explementary, // 360 - x
		}

		#region private data

		private string triangleName_ = "[UNKNOWN TRIANGLE]";
		private int angleNumber_ = 0;
		private EAngleModifier eModifier_ = EAngleModifier.Raw;

		#endregion

		#region setup

		public AngleProvider_Triangle( 
			string tn,
			int an,
			EAngleModifier eam)
		{
			triangleName_ = tn;
			angleNumber_ = an;
			eModifier_ = eam;
		}

		#endregion setup

		#region IAngleProvider

		public float GetAngle( ElementList elements )
		{
			float result = 0f;

			Element_Triangle triangle = elements.GetRequiredElementOfType<Element_Triangle>( triangleName_ );
			float rawAngle = triangle.GetInternalAngleDegrees( angleNumber_ );
			switch (eModifier_)
			{
				case EAngleModifier.Raw:
					{
						result = rawAngle;
						break;
					}
				case EAngleModifier.Complementary:
					{
						result = 90f -rawAngle;
						break;
					}
				case EAngleModifier.Supplementary:
					{
						result = 180f - rawAngle;
						break;
					}
				case EAngleModifier.Explementary:
					{
						result = 360f - rawAngle;
						break;
					}
			}
			return result;
		}

		#endregion
	}
}

