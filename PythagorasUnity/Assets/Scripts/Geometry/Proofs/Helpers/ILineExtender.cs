using UnityEngine;
using System.Collections;

namespace RJWard.Geometry
{
	public interface ILineExtender 
	{
		bool ExtendLine( ElementList elementList );
		bool ExtendLine( ElementList elementList, Element_StraightLine line ); // provide the line to extend directly (element list may be used to obtain ancillary elements)
	}
}
