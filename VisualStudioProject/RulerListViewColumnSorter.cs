using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FpgaLcdUtils
{
  internal class RulerListViewColumnSorter  : IComparer
  {
    public interface ILVColumns
    {
      public int GetColumnValue(int ix);
    }
    /// <summary>
    /// Specifies the column to be sorted
    /// </summary>
    private int activeColumn;
    //   private SortOrder OrderOfSort;
    /// <summary>
    /// Case insensitive comparer object
    /// </summary>
    private CaseInsensitiveComparer ObjectCompare;
    /// <summary>
    /// Specifies the order in which to sort (i.e. 'Ascending').
    /// </summary>

    int[] sortOrder = new int[] { 1 };

    /// <summary>
    /// Class constructor.  Initializes various elements
    /// </summary>
    public RulerListViewColumnSorter(int ixCountOfColumns)
    {
      // Initialize the column to '0'
      activeColumn = 0;

      // Initialize the sort order to 'none'
      //     OrderOfSort = SortOrder.None;

      // Initialize the CaseInsensitiveComparer object
      ObjectCompare = new CaseInsensitiveComparer();
      sortOrder = new int[ixCountOfColumns];
      for (int i = 0; i < sortOrder.Length; i++) sortOrder[i] = 1;

    }

    public void SetSort(int ixColumn)
    {
      if (ixColumn < 0 && ixColumn >= sortOrder.Length) ixColumn = 0;
      if (this.activeColumn == ixColumn) sortOrder[ixColumn] = -sortOrder[ixColumn];
      this.activeColumn = ixColumn;
    }
    public void SetSort(int ixColumn, bool isUp)
    {
      if (ixColumn < 0 && ixColumn >= sortOrder.Length) ixColumn = 0;
      if (isUp) sortOrder[ixColumn] = 1; else sortOrder[ixColumn] = -1;
      this.activeColumn = ixColumn;
    }

    const int UP = 0;
    const int DOWN = 1;
    const int UP_ACTIVE = 2;
    const int DOWN_ACTIVE = 3;
    public int GetImageIx(int ixColumn)
    {
      if (ixColumn < 0 && ixColumn >= sortOrder.Length) ixColumn = 0;
      if (ixColumn == activeColumn)
        return sortOrder[ixColumn] == 1 ? UP_ACTIVE : DOWN_ACTIVE;
      else
        return sortOrder[ixColumn] == 1 ? UP : DOWN;
    }



    /// <summary>
    /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
    /// </summary>
    /// <param name="x">First object to be compared</param>
    /// <param name="y">Second object to be compared</param>
    /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
    public int Compare(object x, object y)
    {
      int compareResult;
      ListViewItem? listviewX, listviewY;

      // Cast the objects to be compared to ListViewItem objects
      listviewX = (ListViewItem)x;
      listviewY = (ListViewItem)y;
      ILVColumns? ilvcX = listviewX?.Tag as ILVColumns;
      ILVColumns? ilvcY = listviewY?.Tag as ILVColumns;
      int ix = ilvcX!=null ? ilvcX.GetColumnValue(activeColumn) : 0;
      int iy = ilvcY != null ? ilvcY.GetColumnValue(activeColumn) : 0;
      compareResult = ix<iy ? -1 : (ix==iy ? 0 : 1);
      return compareResult * sortOrder[activeColumn];
    }
  }
}
