using iRocks.DataLayer;
using NClassifier;
using System;
using System.Collections.Generic;
using System.Linq;

namespace iRocks.AI.Entities
{
    public class PredictiveClassifier : IClassifier
    {
        private ITrainableClassifier _classifier;
        private IPostRepository _postRepository;
        public PredictiveClassifier(ITrainableClassifier classifier, IPostRepository postRepository)
        {
            _classifier = classifier;
            _postRepository = postRepository;
        }

        public Category Classify(Post post, IEnumerable<Category> Categories)
        {
            //_Classifier.Classify(input, Categories);

            List<Tuple<Category, double>> categoriesRate = ClassifyRecursive(post, Categories);
            Category bestCategory = null;
            var tuple = categoriesRate.Where(c => c.Item1.CategoryId != 1).OrderByDescending(c => c.Item2).FirstOrDefault();
            if (tuple != null)
                bestCategory = tuple.Item1;
            if (bestCategory == null)
            {
                GetMajorCategoryRecursive(post, Categories);
            }

            if (bestCategory == null)
            {
                bestCategory = Categories.Where(c => c.CategoryId == 1).FirstOrDefault();
            }

            return bestCategory;
        }
        private List<Tuple<Category, double>> ClassifyRecursive(Post post, IEnumerable<Category> categories)
        {
            List<Tuple<Category, double>> categoriesRate = new List<Tuple<Category, double>>();
            if (post.IsProvidedBy(Provider.Facebook))
            {
                if (!String.IsNullOrWhiteSpace(post.FacebookDetail.Message))
                    categoriesRate.Add(_classifier.Classify(post.FacebookDetail.Message, categories));
                if (!String.IsNullOrWhiteSpace(post.FacebookDetail.LinkName))
                    categoriesRate.Add(_classifier.Classify(post.FacebookDetail.LinkName, categories));
                if (post.FacebookDetail.ChildPublication != null)
                {
                    return categoriesRate.Concat(ClassifyRecursive(post.FacebookDetail.ChildPublication.Post, categories)).ToList();
                }
            }
            if (post.IsProvidedBy(Provider.Twitter))
            {
                if (!String.IsNullOrWhiteSpace(post.TwitterDetail.Text))
                    categoriesRate.Add(_classifier.Classify(post.TwitterDetail.Text, categories));
                if (post.TwitterDetail.RetweetedPublication != null)
                {
                    return categoriesRate.Concat(ClassifyRecursive(post.TwitterDetail.RetweetedPublication.Post, categories)).ToList();
                }
            }
            return categoriesRate;
        }

        private Category GetMajorCategoryRecursive(Post post, IEnumerable<Category> categories)
        {
            Category bestCategory = null;
            if (post.IsProvidedBy(Provider.Facebook))
            {
                if (post.FacebookDetail.ChildPublication != null)
                {
                    bestCategory = GetMajorCategoryRecursive(post.FacebookDetail.ChildPublication.Post, categories);
                }
            }
            if (post.IsProvidedBy(Provider.Twitter))
            {
                if (post.TwitterDetail.RetweetedPublication != null)
                {
                    bestCategory = GetMajorCategoryRecursive(post.TwitterDetail.RetweetedPublication.Post, categories);
                }
            }


            if (bestCategory == null)
            {
                //recupération des post de l'auteur de la publication afin de connaitre sa catégorie de prédilection
                // TO DO eviter de faire des appels en base?
                var posts = _postRepository.Select(new { AppUserId = post.AppUserId });
                var bestGroup = posts.Where(p => p.CategoryId != 1).GroupBy(p => p.CategoryId).OrderByDescending(g => g.Count()).FirstOrDefault();
                if (bestGroup != null)
                {
                    var bestCategoryId = bestGroup.Key;
                    bestCategory = categories.Where(c => c.CategoryId == bestCategoryId).FirstOrDefault();
                }
            }
            return bestCategory;
        }
    }
}
