using System.Collections.Generic;

namespace AdventureService.ViewModels
{
    public class ItemsViewModel<TEntity> where TEntity : class
    {
        public long Count { get; private set; }
        public IEnumerable<TEntity> Data { get; private set; }
        public ItemsViewModel(long count, IEnumerable<TEntity> data)
        {
            Count = count;
            Data = data;
        }
    }
}