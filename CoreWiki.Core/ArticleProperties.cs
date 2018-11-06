using System;
using System.Collections.Generic;
using System.Text;

namespace CoreWiki.Core
{

	/// <summary>
	/// A collection of various properties that determine interactions available with an article
	/// </summary>
	[Flags]
	public enum ArticleProperties
	{

		// Magnus10 cheered 100 bits on November 6, 2018
		// xenoph cheered 100 bits on November 6, 2018


		CommentsEnabled = 1,
		TopicCanBeRenamed = 2


	}
}
