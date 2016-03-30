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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NClassifier.Bayesian
{
    /// <summary>
    /// An implementation of IClassifer based on Bayes' algorithm.
    /// </summary>
    public class BayesianClassifier : AbstractClassifier, ITrainableClassifier
    {
        #region Fields
        IWordsDataSource _wordsData;
        ITokenizer _tokenizer;
        IStopWordProvider _stopWordProvider;
        ICategoryRepository _categoryRepository;
        bool _isCaseSensitive = false;
        #endregion

        #region Properties
        public bool IsCaseSensitive { get { return _isCaseSensitive; } set { _isCaseSensitive = value; } }
        public IWordsDataSource WordsDataSource { get { return _wordsData; } }
        public ITokenizer Tokenizer { get { return _tokenizer; } }
        public IStopWordProvider StopWordProvider { get { return _stopWordProvider; } }
        #endregion

        #region Constructors
        public BayesianClassifier(ICategoryRepository cr, IWordProbabilityRepository wpr) : this(new DbWordsDataSource(wpr), new DefaultTokenizer(DefaultTokenizer.BREAK_ON_WORD_BREAKS), cr) { }

        public BayesianClassifier(IWordsDataSource wd, ICategoryRepository cr) : this(wd, new DefaultTokenizer(DefaultTokenizer.BREAK_ON_WORD_BREAKS), cr) { }

        public BayesianClassifier(IWordsDataSource wd, ITokenizer tokenizer, ICategoryRepository cr) : this(wd, tokenizer, new DefaultStopWordProvider(), cr) { }

        public BayesianClassifier(IWordsDataSource wd, ITokenizer tokenizer, IStopWordProvider swp, ICategoryRepository cr)
        {
            if (wd == null)
                throw new ArgumentNullException("IWordsDataSource cannot be null.");
            _wordsData = wd;

            if (tokenizer == null)
                throw new ArgumentNullException("ITokenizer cannot be null.");
            _tokenizer = tokenizer;

            if (swp == null)
                throw new ArgumentNullException("IStopWordProvider cannot be null.");
            _stopWordProvider = swp;

            if (cr == null)
                throw new ArgumentNullException("ICategoryRepository cannot be null.");
            _categoryRepository = cr;
        }
        #endregion

        public bool IsMatch(Category category, string input)
        {
            return IsMatch(category, _tokenizer.Tokenize(input));
        }

        public override Tuple<Category, double> Classify(string input, IEnumerable<Category> Categories = null)
        {
            if (input == null)
                throw new ArgumentNullException("Input cannot be null.");

            if (Categories == null)
                Categories = _categoryRepository.Select();

            Dictionary<Category, double> probabilities = Classify(Categories.ToArray(), _tokenizer.Tokenize(input));

            var bestProb = probabilities.OrderByDescending(x => x.Value).FirstOrDefault();
            if (bestProb.Value < cutoff)
                return new Tuple<Category, double>(Categories.Where(c => c.CategoryId == 1).Single(), cutoff);

            return new Tuple<Category, double>(bestProb.Key, bestProb.Value);
        }

        public Dictionary<Category, double> Classify(Category[] categories, string[] words)
        {
            Dictionary<Category, double> res = new Dictionary<Category, double>();
            WordProbability[] wps = CalcWordsProbability(categories, words);
            var wpsByCategory = wps.GroupBy(p => p.CategoryId);
            foreach (var group in wpsByCategory)
            {
                var proba = NormalizeSignificance(CalculateOverallProbability(group.ToArray()));
                var category = categories.Where(c => c.CategoryId == group.First().CategoryId).First();
                res.Add(category, proba);
            }
            return res;
        }

        public double Classify(Category category, string input)
        {
            return Classify(category, _tokenizer.Tokenize(input));
        }
        public double Classify(Category category, string[] words)
        {
            if (category == null)
                throw new ArgumentNullException("Category cannot be null.");

            //CheckCategoriesSupported(category.Label);

            return Classify(new Category[] { category }, words).Where(pair => pair.Key.CategoryId == category.CategoryId).First().Value;
        }

        /*public void TeachMatch(string input)
        {
            TeachMatch(Categories.POSITIVE_CATEGORY, input);
        }

        public void TeachNonMatch(string input)
        {
            TeachNonMatch(Categories.POSITIVE_CATEGORY, input);
        }*/

        public Task TeachMatchAsync(Category category, string input)
        {
            return Task.Factory.StartNew(() =>
            {
                if (category == null)
                    throw new ArgumentNullException("Category cannot be null.");
                if (input == null)
                    throw new ArgumentNullException("Input cannot be null.");

                //CheckCategoriesSupported(category);

                TeachMatch(category, _tokenizer.Tokenize(input));
                //Teach non match for all the ohter categories
                var categories = _categoryRepository.Select();
                foreach (var otherCategory in categories)
                {
                    if (otherCategory.CategoryId != category.CategoryId)
                    {
                        TeachNonMatch(otherCategory, _tokenizer.Tokenize(input));
                    }
                }

            });
        }

        public Task TeachNonMatchAsync(Category category, string input)
        {
            return Task.Factory.StartNew(() =>
            {
                if (category == null)
                    throw new ArgumentNullException("Category cannot be null.");
                if (input == null)
                    throw new ArgumentNullException("Input cannot be null.");

                //CheckCategoriesSupported(category);

                TeachNonMatch(category, _tokenizer.Tokenize(input));
            });
        }

        public bool IsMatch(Category category, string[] input)
        {
            if (category == null)
                throw new ArgumentNullException("Category cannot be null.");
            if (input == null)
                throw new ArgumentNullException("Input cannot be null.");

            CheckCategoriesSupported(category.Label);

            double matchProbability = Classify(category, input);

            return (matchProbability >= cutoff);
        }

        public void TeachMatch(Category category, string[] words)
        {
            bool categorize = false;
            if (_wordsData is ICategorizedWordsDataSource)
                categorize = true;
            for (int i = 0; i <= words.Length - 1; i++)
            {
                if (IsClassifiableWord(words[i]))
                {
                    if (categorize)
                        ((ICategorizedWordsDataSource)_wordsData).AddMatch(category, TransformWord(words[i]));
                    //else
                    //_wordsData.AddMatch(TransformWord(words[i]));
                }
            }
        }

        public void TeachNonMatch(Category category, string[] words)
        {
            bool categorize = false;
            if (_wordsData is ICategorizedWordsDataSource)
                categorize = true;
            for (int i = 0; i <= words.Length - 1; i++)
            {
                if (IsClassifiableWord(words[i]))
                {
                    if (categorize)
                        ((ICategorizedWordsDataSource)_wordsData).AddNonMatch(category, TransformWord(words[i]));
                    //else
                    //_wordsData.AddNonMatch(TransformWord(words[i]));
                }
            }
        }

        /// <summary>
        /// Allows transformations to be done to the given word.
        /// </summary>
        /// <param name="word">The word to transform.</param>
        /// <returns>The transformed word.</returns>
        public string TransformWord(string word)
        {
            if (word != null)
            {
                if (!_isCaseSensitive)
                    return word.ToLower();
                else
                    return word;
            }
            else
                throw new ArgumentNullException("Word cannot be null.");
        }

        public double CalculateOverallProbability(WordProbability[] wps)
        {
            if (wps == null || wps.Length == 0)
                return IClassifierConstants.NEUTRAL_PROBABILITY;

            // we need to calculate xy/(xy + z) where z = (1 - x)(1 - y)

            // first calculate z and xy
            double z = 0d;
            double xy = 0d;
            for (int i = 0; i < wps.Length; i++)
            {
                if (z == 0)
                    z = (1 - wps[i].Probability);
                else
                    z = z * (1 - wps[i].Probability);

                if (xy == 0)
                    xy = wps[i].Probability;
                else
                    xy = xy * wps[i].Probability;
            }

            double numerator = xy;
            double denominator = xy + z;

            return numerator / denominator;
        }

        private WordProbability[] CalcWordsProbability(Category[] categories, string[] words)
        {
            if (categories == null && categories.Length > 0)
                throw new ArgumentNullException("Category cannot be null.");

            bool categorize = false;
            if (_wordsData is ICategorizedWordsDataSource)
                categorize = true;

            //CheckCategoriesSupported(category.Label);

            if (words == null)
                return new WordProbability[0];
            else
            {
                if (categorize)
                {
                    return ((ICategorizedWordsDataSource)_wordsData).GetWordsProbabilities(categories.ToList(), words.ToList().Where(w => IsClassifiableWord(w)).ToList()).ToArray();

                }
                return null;

            }
        }

        private void CheckCategoriesSupported(string category)
        {
            // if the category is not the default
            //Modification by PY Dez: Adding check on all existing category

            var categories = _categoryRepository.Select(new { Label = category });

            if (!categories.Any())
                if (!(_wordsData is ICategorizedWordsDataSource))
                    throw new ArgumentException("Word Data Source does not support non-default categories.");
        }

        private bool IsClassifiableWord(string word)
        {
            if (word == null || word == string.Empty || _stopWordProvider.IsStopWord(word))
                return false;
            else
                return true;
        }

        public static double NormalizeSignificance(double sig)
        {
            if (IClassifierConstants.UPPER_BOUND < sig)
                return IClassifierConstants.UPPER_BOUND;
            else if (IClassifierConstants.LOWER_BOUND > sig)
                return IClassifierConstants.LOWER_BOUND;
            else
                return sig;
        }
    }
}