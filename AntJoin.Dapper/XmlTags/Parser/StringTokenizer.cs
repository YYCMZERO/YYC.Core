using System;
using System.Collections;
using System.Text;

namespace AntJoin.Dapper.XmlTags.Parser
{

		
	/// <summary>
	/// A StringTokenizer java like object 
	/// </summary>
	public class StringTokenizer : IEnumerable 
	{
		public void Init(string delimiter,string content){
			_origin = content;
			_delimiters=delimiter;
		}
		private static readonly string _defaultDelim=" \t\n\r\f";
		string _origin = string.Empty;
		string _delimiters = string.Empty;
		bool _returnDelimiters = false;

		/// <summary>
		/// Constructs a StringTokenizer on the specified String, using the
		/// default delimiter set (which is " \t\n\r\f").
		/// </summary>
		/// <param name="str">The input String</param>
		public StringTokenizer(string str) 
		{
			_origin = str;
			_delimiters = _defaultDelim;
			_returnDelimiters = false;
		}


		/// <summary>
		/// Constructs a StringTokenizer on the specified String, 
		/// using the specified delimiter set.
		/// </summary>
		/// <param name="str">The input String</param>
		/// <param name="delimiters">The delimiter String</param>
		public StringTokenizer(string str, string delimiters) 
		{
			_origin = str;
			_delimiters = delimiters;
			_returnDelimiters = false;
		}


		/// <summary>
		/// Constructs a StringTokenizer on the specified String, 
		/// using the specified delimiter set.
		/// </summary>
		/// <param name="str">The input String</param>
		/// <param name="delimiters">The delimiter String</param>
		/// <param name="returnDelimiters">Returns delimiters as tokens or skip them</param>
		public StringTokenizer(string str, string delimiters, bool returnDelimiters) 
		{
			_origin = str;
			_delimiters = delimiters;
			_returnDelimiters = returnDelimiters;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator() 
		{
			return new StringTokenizerEnumerator(this);
		}


		/// <summary>
		/// Returns the number of tokens in the String using
		/// the current deliminter set.  This is the number of times
		/// nextToken() can return before it will generate an exception.
		/// Use of this routine to count the number of tokens is faster
		/// than repeatedly calling nextToken() because the substrings
		/// are not constructed and returned for each token.
		/// </summary>
		public int TokenNumber
		{
			get
			{
				int count = 0;
				int currpos = 0;
				int maxPosition = _origin.Length;

				while (currpos < maxPosition) 
				{
					while (!_returnDelimiters &&
						(currpos < maxPosition) &&
						(_delimiters.IndexOf(_origin[currpos]) >= 0))
					{
						currpos++;
					}

					if (currpos >= maxPosition)
					{
						break;
					}

					int start = currpos;
					while ((currpos < maxPosition) && 
						(_delimiters.IndexOf(_origin[currpos]) < 0))
					{
						currpos++;
					}
					if (_returnDelimiters && (start == currpos) &&
						(_delimiters.IndexOf(_origin[currpos]) >= 0))
					{
						currpos++;
					}
					count++;
				}
				return count;

			}

		}


		private class StringTokenizerEnumerator : IEnumerator 
		{
			private StringTokenizer _stokenizer;
			private int _cursor = 0;
			private string _next = null;
		
			public StringTokenizerEnumerator(StringTokenizer stok) 
			{
				_stokenizer = stok;
			}

			public bool MoveNext() 
			{
				_next = GetNext();
				return _next != null;
			}

			public void Reset() 
			{
				_cursor = 0;
			}

			public object Current 
			{
				get 
				{
					return _next;
				}
			}

			private string GetNext() 
			{
				char c;
				bool isDelim;
			
				if( _cursor >= _stokenizer._origin.Length )
					return null;

				c = _stokenizer._origin[_cursor];
				isDelim = (_stokenizer._delimiters.IndexOf(c) != -1);
			
				if ( isDelim ) 
				{
					_cursor++;
					if ( _stokenizer._returnDelimiters ) 
					{
						return c.ToString();
					}
					return GetNext();
				}

				int nextDelimPos = _stokenizer._origin.IndexOfAny(_stokenizer._delimiters.ToCharArray(), _cursor);
				if (nextDelimPos == -1) 
				{
					nextDelimPos = _stokenizer._origin.Length;
				}

				string nextToken = _stokenizer._origin.Substring(_cursor, nextDelimPos - _cursor);
				_cursor = nextDelimPos;
				return nextToken;
			}

		}
	}

}

