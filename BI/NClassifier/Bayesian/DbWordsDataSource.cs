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

using System;
using System.Linq;
using System.Diagnostics;
using System.Data;
using System.Data.Odbc;
using iRocks.DataLayer;
using System.Collections.Generic;

namespace NClassifier.Bayesian
{
	public class DbWordsDataSource : ICategorizedWordsDataSource
	{
        private IWordProbabilityRepository wordProbabilityRepository;
        public DbWordsDataSource(IWordProbabilityRepository aWordProbabilityRepository)
        {
            this.wordProbabilityRepository = aWordProbabilityRepository;
        }

        public WordProbability GetWordProbability(Category category, string word)
		{
			WordProbability wp = new WordProbability();

			try
			{
                var res = wordProbabilityRepository.Select(new { Word = word, CategoryId = category.CategoryId }, SQLKeyWord.And).FirstOrDefault();
				if(res!=null)
				{
                    wp = new WordProbability(res);
				}
			}
			catch (Exception ex)
			{
				throw new WordsDataSourceException("Problem obtaining WordProbability from database.", ex);
			}
			Debug.WriteLine("GetWordProbability() WordProbability loaded [" + wp + "]");

			return wp;
		}

        public IEnumerable<WordProbability> GetWordsProbabilities(List<Category> categories, List<string> words)
        {
            List<WordProbability> wp = new List<WordProbability>();

            try
            {
                //a virer
                if (words.Count > 2000)
                    wp = new List<WordProbability>();
                var res = wordProbabilityRepository.Select(new { Word = words, CategoryId = categories.Select(c=>c.CategoryId).ToList() }, SQLKeyWord.And);
                if (res != null)
                {
                    res.ToList().ForEach(p => wp.Add(new WordProbability(p)));
                }
            }
            catch (Exception ex)
            {
                throw new WordsDataSourceException("Problem obtaining WordProbability from database.", ex);
            }
            //Debug.WriteLine("GetWordProbability() WordProbability loaded [" + wp + "]");

            return wp;
        }


        /*public WordProbability GetWordProbability(string word)
		{
			return GetWordProbability(Categories.POSITIVE_CATEGORY, word);
		}*/

        private void UpdateWordProbability(Category category, string word, bool isMatch)
		{

			// truncate word at 255 characters
			if (word.Length > 255)
				word = word.Substring(0, 255);

			try
			{
                // see if the word exists in the table
                var res = wordProbabilityRepository.Select(new { Word = word, CategoryId = category.CategoryId }, SQLKeyWord.And).FirstOrDefault();


                if (res==null) // word is not in table, so insert the word
				{
                    res = new iRocks.DataLayer.WordProbability()
                    {
                        CategoryId=category.CategoryId,
                        Word = word
                    };
				}
                if (isMatch)
                    ++res.Matches;
                else
                    ++res.NonMatches;
                if (res.IsNew)
                    wordProbabilityRepository.Insert(res);
                else
                    wordProbabilityRepository.Update(res);
			}
			catch (Exception ex)
			{
				throw new WordsDataSourceException("Problem updating WordProbability.", ex);
			}
		}

        public void AddMatch(Category category, string word)
		{
			if (category == null)
				throw new ArgumentNullException("Category cannot be null.");
			UpdateWordProbability(category, word, true);
		}

		/*public void AddMatch(string word)
		{
            UpdateWordProbability(Categories.POSITIVE_CATEGORY, word, true);
		}*/

        public void AddNonMatch(Category category, string word)
		{
			if (category == null)
				throw new ArgumentNullException("Category cannot be null.");
			UpdateWordProbability(category, word, false);
		}

		/*public void AddNonMatch(string word)
		{
			UpdateWordProbability(Categories.POSITIVE_CATEGORY, word, false);
		}*/

		
	}
}