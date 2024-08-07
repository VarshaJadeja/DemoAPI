using Newtonsoft.Json;

namespace Demo.Entities.ViewModels;

public class PaginatedItemsViewModel<T> : ResultViewModel<List<T>> where T : class
{
    [JsonProperty(PropertyName = "page_index")]
    public int PageIndex { get; private set; }

    [JsonProperty(PropertyName = "page_size")]
    public int PageSize { get; private set; }

    public long Count { get; private set; }

    public PaginatedItemsViewModel(int status, string message, List<T> data, int pageIndex, int pageSize, long count)
        : base(status, message, data)
    {
        this.PageIndex = pageIndex;
        this.PageSize = pageSize;
        this.Count = count;
    }
}
