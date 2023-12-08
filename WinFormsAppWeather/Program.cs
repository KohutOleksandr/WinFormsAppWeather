using System.Text.Json;
using System;
using System.Data.SQLite;
using System.Reflection.PortableExecutable;
using System.Xml.Linq;
using System.Runtime.ConstrainedExecution;
using System.Diagnostics;

namespace WinFormsAppWeather
{
    public class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static string url = $"https://api.openweathermap.org/data/2.5/forecast?appid={tokenAPI.getToken()}&q=Cherkasy&cnt=20&units=metric";
        
        [STAThread]
        async static Task Main(string[] args)
        {
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection();
            CreateTable(sqlite_conn);
            await getWeather(sqlite_conn);

            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());

            sqlite_conn.Close();
        }

        static SQLiteConnection CreateConnection()
        {

            SQLiteConnection sqlite_conn;
            sqlite_conn = new SQLiteConnection("Data Source=databaseWeatherInCherkasy.db; Version = 3; New = True; Compress = True; ");
            
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return sqlite_conn;
        }
        static void CreateTable(SQLiteConnection conn)
        {

            SQLiteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE IF NOT EXISTS cherkasy (" +
                "dt INTEGER PRIMARY KEY NOT NULL ," +
                " date TEXT, " +
                "time TEXT," +
                " temp REAL," +
                "pressure REAL," +
                "wind_speed REAL," +
                "weather_main TEXT," +
                "weather_description TEXT)";

            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();

        }
        async static Task getWeather(SQLiteConnection conn)
        {
            
            var responseString = await client.GetStringAsync(url);
            
            WeatherForecast? weatherForecast =
               JsonSerializer.Deserialize<WeatherForecast>(responseString);
            

            Dictionary<string, string> cherkasy = new Dictionary<string, string>()
            {
                { "dt", String.Empty},
                { "date", String.Empty},
                { "time", String.Empty},
                {"temp", String.Empty},
                { "pressure", String.Empty},
                { "wind_speed", String.Empty},
                { "weather_main", String.Empty},
                {"weather_description", String.Empty }
            };

            foreach (var weatherInfo in weatherForecast?.list)
            {
                Console.WriteLine(weatherInfo?.dt);
                cherkasy["dt"] = Convert.ToString(weatherInfo?.dt);

                string[] dateTimeParts = Convert.ToString(weatherInfo?.dt_txt).Split(' ');
                cherkasy["date"] = dateTimeParts[0];
                cherkasy["time"] = dateTimeParts[1];
                cherkasy["temp"] = Convert.ToString(weatherInfo?.main?.temp).Replace(',', '.');
                cherkasy["pressure"] = Convert.ToString(weatherInfo?.main?.pressure);
                cherkasy["wind_speed"] = Convert.ToString(weatherInfo?.wind?.speed).Replace(',', '.');


                foreach (var weatherInfoWeather in weatherInfo?.weather)
                {
                    cherkasy["weather_main"] = Convert.ToString(weatherInfoWeather?.main);
                    cherkasy["weather_description"] = Convert.ToString(weatherInfoWeather?.description);
                }

                SQLiteCommand sqlite_cmd;
                sqlite_cmd = conn.CreateCommand();

                sqlite_cmd.CommandText = $"INSERT INTO cherkasy (" +
                $"dt, date, time, temp, pressure, wind_speed, weather_main, weather_description) " +
                $"SELECT {cherkasy["dt"]},'{cherkasy["date"]}','{cherkasy["time"]}', {cherkasy["temp"]}, {cherkasy["pressure"]}, {cherkasy["wind_speed"]}, '{cherkasy["weather_main"]}', '{cherkasy["weather_description"]}' WHERE NOT EXISTS ( SELECT * FROM cherkasy WHERE dt = {cherkasy["dt"]}); ";


                sqlite_cmd.ExecuteNonQuery();
            }

        }

        
    }
}