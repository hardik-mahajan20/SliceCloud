namespace SliceCloud.Repository.ViewModels;

public class TableSectionViewModel
{
    public IEnumerable<TableViewModel>? Tables { get; set; }
    public IEnumerable<SectionViewModel>? Sections { get; set; }
}
