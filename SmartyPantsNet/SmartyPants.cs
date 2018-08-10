// SmartyPants - Smart typography for web sites
//
// .NET port (c) 2017 Andy Graulund
// <http://andygraulund.com/>
//
// Derived from:
//
// PHP SmartyPants
// Copyright (c) 2004-2016 Michel Fortin
// <https://michelf.ca/>
//
// Original SmartyPants
// Copyright (c) 2003-2004 John Gruber
// <https://daringfireball.net/>
//

using System.Text.RegularExpressions;

namespace SmartyPantsNet
{
	public class SmartyPants
	{
		public const string SMARTYPANTSLIB_VERSION = "1.0.2";

		//// Presets ////

		public const string ATTR_DO_NOTHING             =  "0"; // SmartyPants does nothing at all
		public const string ATTR_EM_DASH                =  "1"; // "--" for em-dashes; no en-dash support
		public const string ATTR_LONG_EM_DASH_SHORT_EN  =  "2"; // "---" for em-dashes; "--" for en-dashes
		public const string ATTR_SHORT_EM_DASH_LONG_EN  =  "3"; // "--" for em-dashes; "---" for en-dashes
		public const string ATTR_STUPEFY                = "-1"; // "--" for em-dashes; "---" for en-dashes

		// The default preset: ATTR_EM_DASH
		public const string ATTR_DEFAULT = ATTR_EM_DASH;

		//// Configuration Variables ////

		// Options to specify which transformations to make:
		public int DoNothing   = 0; // disable all transforms
		public int DoQuotes    = 0;
		public int DoBackticks = 0; // 1 => double only, 2 => double & single
		public int DoDashes    = 0; // 1, 2, or 3 for the three modes described above
		public int DoEllipses  = 0;
		public int DoStupefy   = 0;

		// Smart quote characters:
		// Opening and closing smart double-quotes.
		public string SmartDoubleQuoteOpen  = "\u201c";
		public string SmartDoubleQuoteClose = "\u201d";
		public string SmartSingleQuoteOpen  = "\u2018";
		public string SmartSingleQuoteClose = "\u2019"; // Also apostrophe.

		// ``Backtick quotes''
		public string BacktickDoubleQuoteOpen  = "\u201c"; // replacement for ``
		public string BacktickDoubleQuoteClose = "\u201d"; // replacement for ''
		public string BacktickSingleQuoteOpen  = "\u2018"; // replacement for `
		public string BacktickSingleQuoteClose = "\u2019"; // replacement for ' (also apostrophe)

		// Other punctuation
		public string EmDash = "\u2014";
		public string EnDash = "\u2013";
		public string Ellipsis = "\u2026";


		/// <summary>
		///  Initialize a parser with certain attributes.
		///
		///  Parser attributes:
		///  0 : do nothing
		///  1 : set all
		///  2 : set all, using old school en- and em- dash shortcuts
		///  3 : set all, using inverted old school en and em- dash shortcuts
		///
		///  q : quotes
		///  b : backtick quotes (``double'' only)
		///  B : backtick quotes (``double'' and `single')
		///  d : dashes
		///  D : old school dashes
		///  i : inverted old school dashes
		///  e : ellipses
		/// </summary>
		public SmartyPants(string attr = ATTR_DEFAULT)
		{
			if (attr == "0")
			{
				DoNothing   = 1;
			}
			else if (attr == "1")
			{
				// Do everything, turn all options on.
				DoQuotes    = 1;
				DoBackticks = 1;
				DoDashes    = 1;
				DoEllipses  = 1;
			}
			else if (attr == "2")
			{
				// Do everything, turn all options on, use old school dash shorthand.
				DoQuotes    = 1;
				DoBackticks = 1;
				DoDashes    = 2;
				DoEllipses  = 1;
			}
			else if (attr == "3")
			{
				// Do everything, turn all options on, use inverted old school dash shorthand.
				DoQuotes    = 1;
				DoBackticks = 1;
				DoDashes    = 3;
				DoEllipses  = 1;
			}
			else if (attr == "-1")
			{
				// Special "stupefy" mode.
				DoStupefy   = 1;
			}
			else
			{
				var chars = attr.ToCharArray();
				foreach (char c in chars)
				{
					if      (c == 'q') { DoQuotes    = 1; }
					else if (c == 'b') { DoBackticks = 1; }
					else if (c == 'B') { DoBackticks = 2; }
					else if (c == 'd') { DoDashes    = 1; }
					else if (c == 'D') { DoDashes    = 2; }
					else if (c == 'i') { DoDashes    = 3; }
					else if (c == 'e') { DoEllipses  = 1; }
					else { /* Unknown attribute option, ignore. */ }
				}
			}
		}


		public string Educate(string t)
		{

			if (DoNothing != 0)
			{
				return t;
			}

			t = ShieldEscapes(t);

			if (DoDashes != 0)
			{
				if (DoDashes == 1)
				{
					t = EducateDashes(t);
				}

				else if (DoDashes == 2)
				{
					t = EducateDashesOldSchool(t);
				}

				else if (DoDashes == 3)
				{
					t = EducateDashesOldSchoolInverted(t);
				}
			}

			if (DoEllipses != 0)
			{
				t = EducateEllipses(t);
			}

			// Note: backticks need to be processed before quotes.
			if (DoBackticks != 0)
			{
				t = EducateBackticks(t);

				if (DoBackticks == 2)
				{
					t = EducateSingleBackticks(t);
				}
			}

			if (DoQuotes != 0)
			{
				t = EducateQuotes(t);
			}

			if (DoStupefy != 0)
			{
				t = StupefyEntities(t);
			}

			t = UnshieldEscapes(t);

			return t;
		}


		/// <summary>
		/// Returns the input string, with "educated" curly quote HTML entities.
		///
		/// Example input:  "Isn't this fun?"
		/// Example output: &#8220;Isn&#8217;t this fun?&#8221;
		/// </summary>
		protected string EducateQuotes(string text)
		{
			var dqOpen  = SmartDoubleQuoteOpen;
			var dqClose = SmartDoubleQuoteClose;
			var sqOpen  = SmartSingleQuoteOpen;
			var sqClose = SmartSingleQuoteClose;

			// Punctuation character class
			var punctClass = "[!\"#\\$\\%'()*+,-.\\/:;<=>?\\@\\[\\\\\\]\\^_`{|}~]";

			// Special case if the very first character is a quote
			// followed by punctuation at a non-word-break. Close the quotes by brute force:
			text = new Regex("^'(?=" + punctClass + "\\B)").Replace(text, sqClose);
			text = new Regex("^\"(?=" + punctClass + "\\B)").Replace(text, dqClose);

			// Special case for double sets of quotes, e.g.:
			//   <p>He said, "'Quoted' words in a larger quote."</p>
			text = new Regex("\"'(?=\\w)").Replace(text, dqOpen + sqOpen);
			text = new Regex("'\"(?=\\w)").Replace(text, sqOpen + dqOpen);

			// Special case for decade abbreviations (the '80s):
			text = new Regex("'(?=\\d{2}s)").Replace(text, sqClose);

			var closeClass = "[^ \t\r\n\\[\\{\\(\\-]";

			// Get most opening single quotes:
			text = new Regex("(\\s|--|\u2013|\u2014)'(?=\\w)")
				.Replace(text, "$1" + sqOpen);

			// Single closing quotes:
			text = new Regex("(" + closeClass + ")?'(?(1)|(?=\\s|s\\b))", RegexOptions.IgnoreCase)
				.Replace(text, "$1" + sqClose);

			// Any remaining single quotes should be opening ones:
			text = text.Replace("'", sqOpen);

			// Get most opening double quotes:
			text = new Regex("(\\s|--|\u2013|\u2014)\"(?=\\w)")
				.Replace(text, "$1" + dqOpen);

			// Double closing quotes:
			text = new Regex("(" + closeClass + ")?\"(?(1)|(?=\\s))")
				.Replace(text, "$1" + dqClose);

			// Any remaining quotes should be opening ones.
			text = text.Replace("\"", dqOpen);

			return text;
		}


		/// <summary>
		/// Returns the input string, with ``backticks'' -style double quotes
		/// translated into HTML curly quote entities.
		///
		/// Example input:  ``Isn't this fun?''
		/// Example output: &#8220;Isn't this fun?&#8221;
		/// </summary>
		protected string EducateBackticks(string text)
		{
			text = text.Replace("``", BacktickDoubleQuoteOpen);
			text = text.Replace("''", BacktickDoubleQuoteClose);

			return text;
		}


		/// <summary>
		/// Returns the input string, with `backticks' -style single quotes
		/// translated into HTML curly quote entities.
		///
		/// Example input:  `Isn't this fun?'
		/// Example output: &#8216;Isn&#8217;t this fun?&#8217;
		/// </summary>
		protected string EducateSingleBackticks(string text)
		{
			text = text.Replace("`", BacktickSingleQuoteOpen);
			text = text.Replace("'", BacktickSingleQuoteClose);

			return text;
		}


		/// <summary>
		/// Returns the input string, with each instance of "--" translated to an em-dash HTML entity.
		/// </summary>
		protected string EducateDashes(string text)
		{
			text = text.Replace("--", EmDash);

			return text;
		}


		/// <summary>
		/// Returns the input string, with each instance of "--" translated to
		/// an en-dash HTML entity, and each "---" translated to
		/// an em-dash HTML entity.
		/// </summary>
		protected string EducateDashesOldSchool(string text)
		{
			text = text.Replace("---", EmDash);
			text = text.Replace("--",  EnDash);

			return text;
		}


		/// <summary>
		/// Returns the input string, with each instance of "--" translated to
		/// an em-dash HTML entity, and each "---" translated to
		/// an en-dash HTML entity. Two reasons why: First, unlike the
		/// en- and em-dash syntax supported by
		/// EducateDashesOldSchool(), it's compatible with existing
		/// entries written before SmartyPants 1.1, back when "--" was
		/// only used for em-dashes.  Second, em-dashes are more
		/// common than en-dashes, and so it sort of makes sense that
		/// the shortcut should be shorter to type. (Thanks to Aaron
		/// Swartz for the idea.)
		/// </summary>
		protected string EducateDashesOldSchoolInverted(string text)
		{
			text = text.Replace("---", EnDash);
			text = text.Replace("--",  EmDash);

			return text;
		}


		/// <summary>
		/// Returns the input string, with each instance of "..." translated to
		/// an ellipsis HTML entity. Also converts the case where
		/// there are spaces between the dots.
		///
		/// Example input:  Huh...?
		/// Example output: Huh&#8230;?
		/// </summary>
		protected string EducateEllipses(string text)
		{
			text = text.Replace("...", Ellipsis);
			text = text.Replace(". . .",  Ellipsis);

			return text;
		}


		/// <summary>
		/// Returns the input string, with each SmartyPants HTML entity translated to its ASCII counterpart.
		///
		/// Example input:  &#8220;Hello &#8212; world.&#8221;
		/// Example output: "Hello -- world."
		/// </summary>
		protected string StupefyEntities(string text)
		{
			text = text.Replace(SmartDoubleQuoteOpen, "\"");
			text = text.Replace(SmartDoubleQuoteClose, "\"");
			text = text.Replace(SmartSingleQuoteOpen, "'");
			text = text.Replace(SmartSingleQuoteClose, "'");
			text = text.Replace(EnDash, "-");
			text = text.Replace(EmDash, "--");
			text = text.Replace(Ellipsis, "...");

			return text;
		}


		/// <summary>
		/// Returns the input string, with after processing the following backslash
		/// escape sequences. This is useful if you want to force a "dumb"
		/// quote or other character to appear.
		///
		///  Escape  Value
		///  ------  -----
		///  \\      &#92;
		///  \"      &#34;
		///  \'      &#39;
		///  \.      &#46;
		///  \-      &#45;
		///  \`      &#96;
		/// </summary>
		protected string ShieldEscapes(string text)
		{
			text = text.Replace("\\\\", "%%SMARTYPANTS_ESCAPED_BACKSLASH%%");
			text = text.Replace("\\\"", "%%SMARTYPANTS_ESCAPED_DOUBLE_QUOTE%%");
			text = text.Replace("\\'", "%%SMARTYPANTS_ESCAPED_SINGLE_QUOTE%%");
			text = text.Replace("\\.", "%%SMARTYPANTS_ESCAPED_PERIOD%%");
			text = text.Replace("\\-", "%%SMARTYPANTS_ESCAPED_HYPHEN%%");
			text = text.Replace("\\`", "%%SMARTYPANTS_ESCAPED_GRAVE_ACCENT%%");

			return text;
		}


		protected string UnshieldEscapes(string text)
		{
			text = text.Replace("%%SMARTYPANTS_ESCAPED_BACKSLASH%%", "\\");
			text = text.Replace("%%SMARTYPANTS_ESCAPED_DOUBLE_QUOTE%%", "\"");
			text = text.Replace("%%SMARTYPANTS_ESCAPED_SINGLE_QUOTE%%", "'");
			text = text.Replace("%%SMARTYPANTS_ESCAPED_PERIOD%%", ".");
			text = text.Replace("%%SMARTYPANTS_ESCAPED_HYPHEN%%", "-");
			text = text.Replace("%%SMARTYPANTS_ESCAPED_GRAVE_ACCENT%%", "`");

			return text;
		}
	}
}
