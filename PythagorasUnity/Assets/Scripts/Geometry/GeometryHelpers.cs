using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	public static class GeometryHelpers 
	{
		public enum EAngleModifier
		{
			Raw,
			Complementary, // 90 - x
			Supplementary, // 180 - x
			Explementary, // 360 - x
		}

		public static float ModifyAngle(float rawAngle, EAngleModifier eModifier)
		{
			float result = rawAngle;
			switch (eModifier)
			{
				case EAngleModifier.Raw:
					{
						result = rawAngle;
						break;
					}
				case EAngleModifier.Complementary:
					{
						result = 90f - rawAngle;
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
	}
}
