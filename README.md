# DataGridViewControl
`DataGridViewControl` is a C# class that can be used as a control for a `DataGridView`, offering functionalities such as pagination, searching and sorting of data.

## Features
- **Pagination**: The class allows pagination of the data displayed in the `DataGridView`, facilitating navigation between large data sets.
- **Searching**: It is possible to perform searches in the data displayed in the `DataGridView`, filtering the results based on a search term.
- **Sorting**: The data can be sorted by the columns of the `DataGridView`, allowing for an organized view of the data.

## Usage
To use the `DataGridViewControl`, follow the steps below:

### Instalation
Instantiate the `DataGridViewControl` class, passing a `DataGridViewSettings` object in the constructor
```cs
var settings = new DataGridViewSettings
{
    DataGridView = dataGridView1, // Mandatory
    BindingNavigator = bindingNavigator1, // Optional
    SearchTextBox = toolStripTextBox1, // Optional
    Sortable = true, // Optional
    PageSize = 50 // Optional
};

var dataGridViewControl = new DataGridViewControl(settings);
```

### Set Data Source
Set the data source of the `DataGridViewControl` using the `SetDataSource` method.
```cs
dataGridViewControl.SetDataSource(dataTable);
```

### Searching
Perform searches in the data using the `SearchValue` method.
```cs
dataGridViewControl.SearchValue("search term");
```

### Sorting
If Sortable option is true, the data can be sorted by clicking on the columns of the DataGridView.

## Example Usage
```cs
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
```
