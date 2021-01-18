using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PathwayGames.Extensions
{
    public static class ObservableCollectionExtensions
    {
        public static void AddRange<T>(this ObservableCollection<T> observableCollection, IEnumerable<T> rangeList)
        {
            foreach (T item in rangeList)
                observableCollection.Add(item);
        }
    }
}