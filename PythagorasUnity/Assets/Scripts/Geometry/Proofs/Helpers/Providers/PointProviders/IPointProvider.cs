using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	public interface IPointProvider
	{
		Vector2 GetPoint( ElementList elementList );
	}
}

