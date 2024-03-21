using System.Windows.Forms;
using System.Data;
using System;

namespace DataGridViewControl
{
    public partial class Index : Form
    {
        readonly DataGridViewControl dataGridViewControl;
        public Index()
        {
            InitializeComponent();

            // Initializing DataGridView Control
            dataGridViewControl = new DataGridViewControl(
            new DataGridViewControl.DataGridViewSettings
            {
                DataGridView = myDataGridView,          // Mandatory
                BindingNavigator = myBindingNavigator,  // Optional
                SearchTextBox = mySearchTextBox,        // Optional
                Sortable = true,                        // Optional
                PageSize = 50                           // Optional
            });

            // Configuring my DataGridView after DataBinding
            myDataGridView.DataBindingComplete += (sender, e) =>
            {
                myDataGridView.Rows[0].Selected = false;
                foreach (DataGridViewColumn col in myDataGridView.Columns)
                {
                    col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    col.SortMode = DataGridViewColumnSortMode.Programmatic;
                    col.MinimumWidth = 65;
                    col.FillWeight = 100;
                }
            };

            // Searching when the search button is clicked
            mySearchButton.Click += (sender, e) =>
            {
                dataGridViewControl.SearchValue(mySearchTextBox.Text);
            };

            // Example DataTable
            DataTable dataTableExample = CreateExampleDataTable();
            dataGridViewControl.SetDataSource(dataTableExample);
        }

        static DataTable CreateExampleDataTable()
        {
            int dataTableLength = 10000;
            DataTable dataTable = new DataTable("Example");

            dataTable.Columns.Add("ID", typeof(string));
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Columns.Add("Birth", typeof(DateTime));
            dataTable.Columns.Add("Salary", typeof(decimal));

            string[] names = { "Alice", "Bob", "Charlie", "David", "Eve", "Frank", "Grace", "Henry", "Ivy", "Jack" };

            Random random = new Random();
            for (int i = 0; i < dataTableLength; i++)
            {
                string id = (i+1).ToString("00000");
                string name = names[random.Next(names.Length)];
                DateTime birth = new DateTime(random.Next(1950, 2000), random.Next(1, 13), random.Next(1, 29));
                decimal salary = random.Next(100000, 500001) / 100.0m;

                dataTable.Rows.Add(id, name, birth, salary);
            }

            return dataTable;
        }
    }
}
