using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Geometry
{
	public class ElementListDefinition : RJWard.Core.IDebugDescribable
	{
		static public bool DEBUG_ELEMENTLISTDEFN = true;

		#region private data

		private Dictionary<string, System.Type> elements_ = null;

		#endregion private data

		#region Setup

		public ElementListDefinition(Dictionary< string, System.Type > d)
		{
			elements_ = d;
		}

		public ElementListDefinition (ElementListDefinition other)
		{
			elements_ = new Dictionary<string, System.Type>( );
			foreach (string n in other.elements_.Keys)
			{
				elements_.Add( n, other.elements_[n] );
			}
		}

		#endregion Setup

		public int NumElements
		{
			get { return elements_.Count; }
		}

		#region Validators

		public bool Validate( ElementList elemntList, bool allowExcess)
		{
			bool success = false;

			System.Text.StringBuilder sb = null;
			if (DEBUG_ELEMENTLISTDEFN)
			{
				sb = new System.Text.StringBuilder( );
				sb.Append( "Validating list " );
				elemntList.DebugDescribe( sb );
				sb.Append( " against Defn " );
				this.DebugDescribe( sb );
				sb.Append( " with allowExcess=" + allowExcess );
			}

			if (elemntList.NumElements < NumElements)
			{
				if (DEBUG_ELEMENTLISTDEFN)
				{
					sb.Append( "\nINVALID because list is too small" );
				}
			}
			else if (!allowExcess && elemntList.NumElements > NumElements)
			{
				if (DEBUG_ELEMENTLISTDEFN)
				{
					sb.Append( "\nINVALID because list is too large" );
				}
			}
			else
			{
				ElementList elementListCopy = new ElementList( elemntList );

				success = true;

				foreach (string n in elements_.Keys)
				{
					ElementBase el = elementListCopy.PopElement( n );
					if (el == null)
					{
						success = false;
						if (DEBUG_ELEMENTLISTDEFN)
						{
							sb.Append( "\nINVALID because list does not contain '" + n + "'" );
						}
					}
					else if (el.GetType() != elements_[n])
					{
						success = false;
						if (DEBUG_ELEMENTLISTDEFN)
						{
							sb.Append( "\nINVALID because '"+n+"' in list is type "+el.GetType().ToString()+" insyead of " + elements_[n].ToString() );
						}
					}
				}

				if (success && !allowExcess)
				{
					if (elementListCopy.NumElements > 0)
					{
						success = false;
						if (DEBUG_ELEMENTLISTDEFN)
						{
							sb.Append( "\nINVALID because list has excess. Remaining list = ");
							elementListCopy.DebugDescribe( sb );
						}
					}
				}
			}
			if (DEBUG_ELEMENTLISTDEFN)
			{
				Debug.Log( sb.ToString( ) );
			}

			return success;
		}

		#endregion Validators

		#region IDebugDescribable

		public void DebugDescribe(System.Text.StringBuilder sb)
		{
			sb.Append( elements_.Count + " element defns " );
			if (elements_.Count > 0)
			{
				foreach (KeyValuePair< string, System.Type > kvp in elements_)
				{
					sb.Append( " (" ).Append( kvp.Key ).Append( " " ).Append( kvp.Value.ToString( ) ).Append(")");
				}
			}
		}

		#endregion IDebugDescribable

	}
}
