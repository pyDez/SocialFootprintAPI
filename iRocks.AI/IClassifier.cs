using iRocks.DataLayer;
using System;
using System.Collections.Generic;

namespace iRocks.AI
{
    public interface IClassifier
    {
        Category Classify(Post post, IEnumerable<Category> Categories);
    }
}
