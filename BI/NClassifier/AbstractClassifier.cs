#region Copyright (c) 2004, Ryan Whitaker
/*********************************************************************************
'
' Copyright (c) 2004 Ryan Whitaker
'
' This software is provided 'as-is', without any express or implied warranty. In no 
' event will the authors be held liable for any damages arising from the use of this 
' software.
' 
' Permission is granted to anyone to use this software for any purpose, including 
' commercial applications, and to alter it and redistribute it freely, subject to the 
' following restrictions:
'
' 1. The origin of this software must not be misrepresented; you must not claim that 
' you wrote the original software. If you use this software in a product, an 
' acknowledgment (see the following) in the product documentation is required.
'
' This product uses software written by the developers of NClassifier
' (http://nclassifier.sourceforge.net).  NClassifier is a .NET port of the Nick
' Lothian's Java text classification engine, Classifier4J 
' (http://classifier4j.sourceforge.net).
'
' 2. Altered source versions must be plainly marked as such, and must not be 
' misrepresented as being the original software.
'
' 3. This notice may not be removed or altered from any source distribution.
'
'********************************************************************************/
#endregion

using iRocks.DataLayer;
using System;
using System.Collections.Generic;

namespace NClassifier
{
	public abstract class AbstractClassifier : IClassifier
	{
		protected double cutoff = IClassifierConstants.DEFAULT_CUTOFF;

		/// <summary>
		/// Gets or sets the value which determines the minimum probability that should be classified as a match.
		/// </summary>
		/// <remarks>
		/// Throws an ArgumentOutOfRangeException if the cutoff is not equal to or greater than 0 or less than or equal to 1.
		/// </remarks>
		public double MatchCutoff 
		{
			get
			{
				return cutoff; 
			}
			set 
			{ 
				if (cutoff > 1 || cutoff < 0)
					throw new ArgumentOutOfRangeException("Cutoff must be equal to or greater than 0 and less than or equal to 1.");
				cutoff = value;
			}
		}

		public abstract Tuple<Category, double> Classify(string input, IEnumerable<Category> Categories = null);

		public bool IsMatch(string input)
		{
			try
			{
				double matchProbability = Classify(input).Item2;
				return IsMatch(matchProbability);
			}
			catch (Exception ex)
			{
				throw new ClassifierException("AbstractClassifier.IsMatch(string input)", ex);
			}
		}

		public bool IsMatch(double matchProbability)
		{
			return (matchProbability >= cutoff);
		}
	}
}