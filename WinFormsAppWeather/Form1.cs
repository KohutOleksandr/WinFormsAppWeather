using System.Data.SQLite;
using System.Data;

namespace WinFormsAppWeather
{
    public partial class Form1 : Form
    {
        private DataTable table;
        private SQLiteDataAdapter adapter;
        private SQLiteConnection sqlite_conn;
        public Form1()
        {
            InitializeComponent();
            this.BackColor = Color.Red;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.ForeColor = Color.Black;
            sqlite_conn = new SQLiteConnection("Data Source=databaseWeatherInCherkasy.db; Version = 3; New = True; Compress = True; ");

            sqlite_conn.Open();

            table = new DataTable();
            sqlite_conn.Close();
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            UpdateDataGridView("SELECT date, time, temp, pressure, weather_main FROM cherkasy");
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            UpdateDataGridView($"SELECT date, time, temp, pressure, weather_main FROM cherkasy WHERE date == \"{currentDate}\"");
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            string currentDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            UpdateDataGridView($"SELECT date, time, temp, pressure, weather_main FROM cherkasy WHERE date == \"{currentDate}\"");
        }


        private void UpdateDataGridView(string query)
        {
            table.Clear();
            adapter = new SQLiteDataAdapter(query, sqlite_conn);
            adapter.Fill(table);
            dataGridView1.DataSource = table;
            sqlite_conn.Close();
        }
    }
}