using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
namespace OutlawHess
{
    static class Column
    {
        /// <summary>
        /// Creates a column heading in the data grid with a databinding
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="binding"></param>
        public static void AddColumn(String columnName, String binding, DataGrid dataGrid)
        {
            //create new data grid text column
            var column = new DataGridTextColumn();
            //set header/column name
            column.Header = columnName;
            //sets binding
            column.Binding = new Binding(binding);
            //add to data grid
            dataGrid.Columns.Add(column);
        }
    }
}
