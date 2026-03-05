namespace SliceCloud.Repository.ViewModels;

public class MenuViewModel
{
    public IEnumerable<CategoryViewModel>? Categories { get; set; }

    public PaginatedList<ItemViewModel>? ItemsPaginated { get; set; }

}
