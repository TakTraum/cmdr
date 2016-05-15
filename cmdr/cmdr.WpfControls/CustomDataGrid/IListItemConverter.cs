
namespace cmdr.WpfControls.CustomDataGrid
{
    // taken from http://blog.functionalfun.net/2009/02/how-to-databind-to-selecteditems.html
    // Author: Samuel Jack, 2009 Creative Commons Attribution 2.0

    /// <summary>
    /// Converts items in the Master list to Items in the target list, and back again.
    /// </summary>
    public interface IListItemConverter
    {
        /// <summary>
        /// Converts the specified master list item.
        /// </summary>
        /// <param name="masterListItem">The master list item.</param>
        /// <returns>The result of the conversion.</returns>
        object Convert(object masterListItem);

        /// <summary>
        /// Converts the specified target list item.
        /// </summary>
        /// <param name="targetListItem">The target list item.</param>
        /// <returns>The result of the conversion.</returns>
        object ConvertBack(object targetListItem);
    }
}
