using System.Data.SQLite;
using System.Data;

namespace WinFormsAppWeather
{
    public partial class Form1 : Form
    {
        private DataTable table = null;
        private SQLiteDataAdapter adapter = null;
        private SQLiteConnection sqlite_conn = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.ForeColor = Color.Black;
            sqlite_conn = new SQLiteConnection("Data Source=databaseWeatherInCherkasy.db; Version = 3; New = True; Compress = True; ");

            sqlite_conn.Open();

            adapter = new SQLiteDataAdapter("SELECT date, time, temp, weather_main FROM cherkasy", sqlite_conn);

            table = new DataTable();

            //adapter.Fill(table);

            //dataGridView1.DataSource = table;
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            table.Clear();
            adapter = new SQLiteDataAdapter("SELECT date, time, temp, weather_main FROM cherkasy", sqlite_conn);
            adapter.Fill(table);
            dataGridView1.DataSource = table;
            sqlite_conn.Close();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            table.Clear();
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");
            adapter = new SQLiteDataAdapter($"SELECT date, time, temp, weather_main FROM cherkasy WHERE date == \"{currentDate}\"", sqlite_conn);
            adapter.Fill(table);
            dataGridView1.DataSource = table;
            sqlite_conn.Close();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            table.Clear();
            string currentDate = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            adapter = new SQLiteDataAdapter($"SELECT date, time, temp, weather_main FROM cherkasy WHERE date == \"{currentDate}\"", sqlite_conn);
            adapter.Fill(table);
            dataGridView1.DataSource = table;
            sqlite_conn.Close();
        }
    }
}