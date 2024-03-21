using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections;
using System.Linq;
using System.Data;
using System;

namespace DataGridViewControl
{
    class DataGridViewControl
    {
        private readonly BindingSource bindingSource = new BindingSource();

        private SortOrder sortOrder = SortOrder.Ascending;
        private string sortColumn = "";

        private DataTable pageData;
        private DataTable gridData;

        private readonly Settings dataGridViewSettings;

        public DataGridViewControl(Settings Settings)
        {
            this.dataGridViewSettings = Settings;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            if (dataGridViewSettings.DataGridView == null) return;

            if (dataGridViewSettings.BindingNavigator != null)
            {
                dataGridViewSettings.BindingNavigator.BindingSource = bindingSource;
                bindingSource.CurrentChanged += new EventHandler(UpdateTable);
                bindingSource.ResetBindings(false);
            }

            if (dataGridViewSettings.SearchTextBox is ToolStripTextBox toolStripTextBox)
            { toolStripTextBox.KeyPress += new KeyPressEventHandler(Search_KeyPress); }

            else if (dataGridViewSettings.SearchTextBox is TextBox textBox)
            { textBox.KeyPress += new KeyPressEventHandler(Search_KeyPress); }

            dataGridViewSettings.DataGridView.ColumnHeaderMouseClick += DatabaseGrid_ColumnHeaderMouseClick;
        }

        private void UpdateTable(object sender, EventArgs e)
        {
            if (dataGridViewSettings.BindingNavigator != null)
            {
                int offset = (int)(bindingSource.Current ?? 0);
                bool hasRows = pageData.Rows.Count > 0;

                dataGridViewSettings.DataGridView.DataSource = hasRows ? pageData.AsEnumerable()
                .Skip(offset).Take(dataGridViewSettings.PageSize).CopyToDataTable() : null;
            }

            SetSortIndicator(sortColumn);
        }

        public void SetDataSource(DataTable dataTable)
        {
            if (dataGridViewSettings.DataGridView == null) return;

            gridData = dataTable;
            pageData = dataTable;

            if (dataGridViewSettings.BindingNavigator != null)
            {
                bindingSource.DataSource = new PageOffsetList(dataTable.Rows.Count, dataGridViewSettings.PageSize);
            }
            else dataGridViewSettings.DataGridView.DataSource = dataTable;
        }

        private void SetSortIndicator(string columnName)
        {
            foreach (DataGridViewColumn column in dataGridViewSettings.DataGridView.Columns)
            {
                bool isAsc = sortOrder == SortOrder.Ascending;

                if (column.Name == columnName)
                    column.HeaderCell.SortGlyphDirection = isAsc
                    ? SortOrder.Ascending : SortOrder.Descending;

                else column.HeaderCell.SortGlyphDirection = SortOrder.None;
            }
        }

        private void DatabaseGrid_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && dataGridViewSettings.Sortable)
            {
                string columnName = dataGridViewSettings.DataGridView.Columns[e.ColumnIndex].Name;

                if (columnName != sortColumn) sortOrder = SortOrder.Ascending;
                pageData = ToggleSortOrder(pageData, columnName);

                UpdateTable(sender, e);
                SetSortIndicator(columnName);
                sortColumn = columnName;
            }
        }

        private DataTable ToggleSortOrder(DataTable data, string columnName)
        {
            DataTable sortedDataTable = data.Clone();

            if (sortOrder == SortOrder.Ascending)
                sortedDataTable = data.AsEnumerable()
                .OrderBy(row => row.Field<object>(columnName))
                .CopyToDataTable();

            else
                sortedDataTable = data.AsEnumerable()
                .OrderByDescending(row => row.Field<object>(columnName))
                .CopyToDataTable();

            sortOrder = (sortOrder == SortOrder.Ascending) ?
                SortOrder.Descending : SortOrder.Ascending;

            return sortedDataTable;
        }

        private void Search_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            { Search(sender, EventArgs.Empty); }
        }

        public void SearchValue(string val)
        {
            if (dataGridViewSettings.DataGridView == null) return;
            Search(val, EventArgs.Empty);
        }

        private void Search(object sender, EventArgs e)
        {
            string searchValue = string.Empty;

            if (sender is ToolStripTextBox toolStripTextBox)
            { searchValue = toolStripTextBox.Text; }

            else if (sender is TextBox textBox)
            { searchValue = textBox.Text; }

            else if (sender is string str)
            { searchValue = str; }

            else return;

            EnumerableRowCollection<DataRow> rows = gridData.AsEnumerable().Where(r => r
            .ItemArray.Any(c => c.ToString().ToLower().Contains(searchValue.ToLower())));

            pageData = searchValue.Length > 0 ?
            rows.Any() ? rows.CopyToDataTable() :
            gridData.Clone() : gridData.Copy();

            bindingSource.DataSource = new PageOffsetList(pageData.Rows.Count, dataGridViewSettings.PageSize);
            bindingSource.Position = 0;
        }


        public class Settings
        {
            public BindingNavigator BindingNavigator { get; set; }
            public DataGridView DataGridView { get; set; }
            public bool Sortable { get; set; } = false;
            public object SearchTextBox { get; set; }
            public int PageSize { get; set; } = 100;
        }

        private class PageOffsetList : IListSource
        {
            private readonly int pageSize;
            private readonly int count;

            public PageOffsetList(int count, int pageSize)
            {
                this.pageSize = pageSize;
                this.count = count;
            }
            public bool ContainsListCollection { get; protected set; }

            public IList GetList()
            {
                var pageOffsets = new List<int>();
                for (int offset = 0; offset < count; offset += pageSize)
                    pageOffsets.Add(offset);
                return pageOffsets;
            }
        }
    }
}
