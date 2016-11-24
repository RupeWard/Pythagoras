using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	public interface IElementProvider
	{
//		ElementBase GetElement( ElementList elementList );

		T GetElement< T >( ElementList elementList ) where T : ElementBase;
    }
}

