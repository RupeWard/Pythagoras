using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Geometry
{
	public class ElementProvider_Name: IElementProvider
	{
		#region private data

		private string elementName_ = "[UNKNOWN ELEMENT]";
		public string elementName
		{  get { return elementName_; } }

		#endregion private data

		#region setup

		public ElementProvider_Name( 
			string en)
		{
			elementName_ = en;
		}

		#endregion setup

		#region IElementProvider

		public T GetElement< T >( ElementList elements ) where T : ElementBase
		{
			return elements.GetRequiredElementOfType< T >( elementName_);
		}

		#endregion
	}
}

