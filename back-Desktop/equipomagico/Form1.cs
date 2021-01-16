using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;


using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;

namespace equipomagico
{
    public partial class Form1 : Form
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string ApplicationName = "Google Sheets API .NET Quickstart";


        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "V9iDF5mja5pSbqW29Gx1VEbyWoLhncbXqHRhgtfX"
          ,   BasePath = "https://sala-magica.firebaseio.com/quiensoy/ "
        };

        IFirebaseClient client;

        public Form1()
        {


            InitializeComponent();


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            client = new  FireSharp.FirebaseClient(config);

            if (client != null)
            {
                MessageBox.Show("Conexión con Base de Datos Exitosa");
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var data = new Data
            {
                id = textBox1.Text,
                Name = textBox2.Text,
                Address = textBox3.Text,
                Age=  textBox4.Text

            };


            SetResponse response = await client.SetTaskAsync("Information/"+textBox1.Text,data);
            Data result = response.ResultAs<Data>();

            MessageBox.Show("Insertado Corresto" + result.id);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define request parameters.
            String spreadsheetId = "1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms";
            String range = "Class Data!A2:E";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(spreadsheetId, range);

            // Prints the names and majors of students in a sample spreadsheet:
            // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;

            String nombres = ""; 
            if (values != null && values.Count > 0)
            {
                nombres += "Name, Major";
           
                Console.WriteLine("Name, Major");
                foreach (var row in values)
                {
                    // Print columns A and E, which correspond to indices 0 and 4.
                    Console.WriteLine("{0}, {1}", row[0], row[4]);
                    nombres += row[0] + "," + row[2];


                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }

            MessageBox.Show(nombres);
            Console.Read();
        }
    }
}
