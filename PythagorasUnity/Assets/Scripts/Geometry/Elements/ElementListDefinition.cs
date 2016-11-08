using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Geometry
{
	/* 
		Stores a list of names and types of element

		Used to specify requirements: ElementList can be validated against ElementListDefinitions
	*/
	public class ElementListDefinition : RJWard.Core.IDebugDescribable
	{
		static public bool DEBUG_ELEMENTLISTDEFN = true;

		#region private data

		private Dictionary<string, System.Type> elements_ = new Dictionary<string, System.Type>();
		private string name_ = "[UNNAMED ElementList]";

		#endregion private data

		#region properties

		public int NumElements
		{
			get { return elements_.Count; }
		}

		public string name
		{
			get { return name_; }
		}

		#endregion properties

		#region Setup

		public ElementListDefinition(string n, Dictionary< string, System.Type > d)
		{
			name_ = n;
			elements_ = d;
		}

		public ElementListDefinition (string n, ElementListDefinition other)
		{
			name_ = n;
			CopyElementsFrom( other );
		}

		public ElementListDefinition( ElementListDefinition other )
		{
			name_ = other.name + " (CLONE)";
			CopyElementsFrom( other );
		}

		private void CopyElementsFrom( ElementListDefinition other )
		{
			foreach (string n in other.elements_.Keys)
			{
				elements_.Add( n, other.elements_[n] );
			}
		}

		#endregion Setup

		#region Validators

		/* Validate an ElementList against this definition

			returns true if element list contains all elements in this definition

			allowExcess - if false, list cannot contain elements NOT in this definition
		*/
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
				ElementList elementListCopy = new ElementList( elemntList ); // Work with a tempoorary copy of list

				success = true; // preconditions met. Assume success until we find otherwise

				foreach (string n in elements_.Keys)
				{
					ElementBase el = elementListCopy.PopElement( n );
					if (el == null) // element with name n not in list
					{
						success = false;
						if (DEBUG_ELEMENTLISTDEFN)
						{
							sb.Append( "\nINVALID because list does not contain '" + n + "'" );
						}
						else
						{
							break; // No need to continue if not debugging
						}
					}
					else if (el.GetType() != elements_[n]) // element with name n is of wrong type
					{
						success = false;
						if (DEBUG_ELEMENTLISTDEFN)
						{
							sb.Append( "\nINVALID because '"+n+"' in list is type "+el.GetType().ToString()+" insyead of " + elements_[n].ToString() );
						}
						else
						{
							break; // No need to continue if not debugging
						}
					}
				}

				// Now check for excess if required
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
				if (success)
				{
					sb.Append( "\nList is VALID" );
				}
				else
				{
					sb.Append( "\nList is NOT valid" );
				}
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
					sb.Append( " ( '" ).Append( kvp.Key ).Append( "', " ).Append( kvp.Value.ToString( ) ).Append(")");
				}
			}
		}

		#endregion IDebugDescribable

	}
}
