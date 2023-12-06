using System.Text.Json;
using System;
using System.Data.SQLite;
using System.Reflection.PortableExecutable;
using System.Xml.Linq;
using System.Runtime.ConstrainedExecution;

namespace WinFormsAppWeather
{
    public class Program
    {
        private static readonly HttpClient client = new HttpClient();
        private static string url = $"https://api.openweathermap.org/data/2.5/forecast?appid={tokenAPI.getToken()}&q=Cherkasy&cnt=5&units=metric";
        
        [STAThread]
        async static Task Main(string[] args)
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());


            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection();
            CreateTable(sqlite_conn);
            await getWeather(sqlite_conn);

            //ReadWeatherData(sqlite_conn);
            sqlite_conn.Close();
        }

        static SQLiteConnection CreateConnection()
        {

            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            sqlite_conn = new SQLiteConnection("Data Source=databaseWeatherInCherkasy.db; Version = 3; New = True; Compress = True; ");
            // Open the connection:
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
                //Console.WriteLine($"weather date : {cherkasy["date"]} ");
                //Console.WriteLine($"weather time : {cherkasy["time"]} ");
                //cherkasy["date_and_time"] = Convert.ToString(weatherInfo?.dt_txt);

                //Console.WriteLine($"weather temp : {weatherInfo?.main?.temp}");
                cherkasy["temp"] = Convert.ToString(weatherInfo?.main?.temp).Replace(',', '.');
                //Console.WriteLine($"weather pressure : {weatherInfo?.main?.pressure}");
                cherkasy["pressure"] = Convert.ToString(weatherInfo?.main?.pressure);
                //Console.WriteLine($"weather wind speed : {weatherInfo?.wind?.speed}");
                cherkasy["wind_speed"] = Convert.ToString(weatherInfo?.wind?.speed).Replace(',', '.');


                foreach (var weatherInfoWeather in weatherInfo?.weather)
                {
                    //Console.WriteLine($"  weather main : {weatherInfoWeather?.main}");
                    cherkasy["weather_main"] = Convert.ToString(weatherInfoWeather?.main);
                    //Console.WriteLine($"  weather description : {weatherInfoWeather?.description}");
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

        static void ReadWeatherData(SQLiteConnection conn)
        {

            SQLiteCommand sqlite_cmd;

            Console.WriteLine("Enter date(YYYY-MM-DD)");

            string date = Console.ReadLine();
            string Createsql = $"SELECT date, time, temp, weather_main FROM cherkasy WHERE date == \"{date}\"";

            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            PrintResult(sqlite_cmd);
            //sqlite_cmd.ExecuteNonQuery();

        }

        static void PrintResult(SQLiteCommand sqlite_cmd)
        {
            SQLiteDataReader sqlite_datareader = sqlite_cmd.ExecuteReader();

            if (sqlite_datareader.HasRows)
            {
                for (int i = 0; i < sqlite_datareader.FieldCount; i++)
                {
                    string columnName = sqlite_datareader.GetName(i);
                    Console.Write($"{columnName,+7}  ");
                }
                Console.WriteLine();

                while (sqlite_datareader.Read())
                {
                    for (int i = 0; i < sqlite_datareader.FieldCount; i++)
                    {
                        var myreader = sqlite_datareader.GetValue(i);
                        Console.Write($"{myreader,+9}");
                    }
                    Console.WriteLine("\n");
                }
            }
            else
            {
                Console.WriteLine("Data not found!");
            }

        }
    }
}