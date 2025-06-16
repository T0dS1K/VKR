using System.Text;
using System.Windows;
using VKRClient.Models;
using System.Net.Http.Json;

namespace VKRClient
{
    public partial class MainWindow : Window
    {
        private List<AttendanceStorage> AttendanceStorage = new List<AttendanceStorage>();
        private int TotalWeeks;
        private int MaxLength;
        private int PageTable;
        private string? Groop;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Groop = GroopName.Text.ToString().ToUpper();

                if (string.IsNullOrWhiteSpace(Groop))
                {
                    return;
                }

                var Response = await App.Client.PostAsJsonAsync("Admin/GetAttendance", Groop);

                if (!Response.IsSuccessStatusCode)
                {
                    if (await JWTManager.JWTUpdate())
                    {
                        Response = await App.Client.PostAsJsonAsync("Admin/GetAttendance", Groop);
                    } 
                }

                var AttendanceSimpled = await Response.Content.ReadFromJsonAsync<List<dynamic>>();

                if (AttendanceSimpled != null && AttendanceSimpled.Count > 0)
                {
                    PageTable = 0;
                    MaxLength = 0;
                    TotalWeeks = 0;
                    GroopName.Clear();
                    AttendanceStorage.Clear();
                    InfoBox.Content = $"({Groop}) Неделя 1";

                    foreach (var Data in AttendanceSimpled)
                    {
                        AttendanceStorage.Add(new AttendanceStorage
                        {
                            FIO = Data.GetProperty("fio").GetString(),
                            Attendance = ConvertStringToMatrix(Data.GetProperty("attendance").GetString())
                        });
                    }

                    EditAttendanceString();
                    GetSelectedWeek();
                }
            }
            catch
            {
                InfoBox.Content = "ЧТО-ТО ПОШЛО НЕ ТАК";
            }
        }

        private void PrevWeek_Click(object sender, RoutedEventArgs e)
        {
            if (PageTable > 0)
            {
                InfoBox.Content = $"({Groop}) Неделя {PageTable--}";
                GetSelectedWeek();
            }
        }

        private void NextWeek_Click(object sender, RoutedEventArgs e)
        {
            if (PageTable < TotalWeeks - 1)
            {
                InfoBox.Content = $"({Groop}) Неделя {++PageTable + 1}";
                GetSelectedWeek();
            }
        }

        private void GetSelectedWeek()
        {
            List<AttendanceData> AttendanceData = new List<AttendanceData>();

            foreach (var Data in AttendanceStorage)
            {
                AttendanceData.Add(new AttendanceData(
                    Data.FIO ?? string.Empty,
                    Data.Attendance?[PageTable, 0] ?? string.Empty,
                    Data.Attendance?[PageTable, 1] ?? string.Empty,
                    Data.Attendance?[PageTable, 2] ?? string.Empty,
                    Data.Attendance?[PageTable, 3] ?? string.Empty,
                    Data.Attendance?[PageTable, 4] ?? string.Empty,
                    Data.Attendance?[PageTable, 5] ?? string.Empty
                ));
            }

            AttendanceTable.LoadTable(AttendanceData);
        }

        private string[,] ConvertStringToMatrix(string input)
        {
            int index = 0;
            string[] data = input.Split('|');

            if (TotalWeeks == 0)
            {
                TotalWeeks = (int)Math.Ceiling((double)data.Length / 6);
            }

            string[,] matrix = new string[TotalWeeks, 6];

            for (int i = 0; i < TotalWeeks; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (index < data.Length - 1)
                    {
                        matrix[i, j] = data[index++];

                        if (matrix[i, j].Length > MaxLength)
                        {
                            MaxLength = matrix[i, j].Length;
                        }
                    }
                    else
                    {
                        matrix[i, j] = string.Empty;
                    }
                }
            }
            return matrix;
        }

        private void EditAttendanceString()
        {
            foreach (AttendanceStorage item in AttendanceStorage)
            {
                for (int i = 0; i < TotalWeeks; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        if (item?.Attendance != null)
                        {
                            string text = item.Attendance[i, j];
                            if (!string.IsNullOrEmpty(text))
                            {
                                item.Attendance[i, j] = text.PadLeft(MaxLength, '0');
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < TotalWeeks; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    bool DayOff = true;
                    bool[] ZeroPos = new bool[MaxLength];

                    for (int charPos = 0; charPos < MaxLength; charPos++)
                    {
                        ZeroPos[charPos] = true;

                        foreach (var Data in AttendanceStorage)
                        {
                            string? Cell = Data.Attendance?[i, j];

                            if (string.IsNullOrEmpty(Cell) || charPos >= Cell.Length || Cell[charPos] != '0')
                            {
                                ZeroPos[charPos] = false;
                                break;
                            }
                        }
                    }

                    foreach (var Data in AttendanceStorage)
                    {
                        StringBuilder SB = new StringBuilder();
                        string? originalCell = Data.Attendance?[i, j];

                        if (string.IsNullOrEmpty(originalCell))
                        {
                            continue;
                        }

                        for (int charPos = 0; charPos < originalCell.Length; charPos++)
                        {
                            if (!(charPos < MaxLength && ZeroPos[charPos] && originalCell[charPos] == '0'))
                            {
                                SB.Append(originalCell[charPos]);
                            }

                        }

                        string newCell = SB.ToString();

                        if (string.IsNullOrEmpty(newCell) || newCell.All(c => c == '0'))
                        {
                            Data.Attendance![i, j] = "0";
                        }
                        else
                        {
                            Data.Attendance![i, j] = newCell;
                        }
                    }

                    foreach (var Data in AttendanceStorage)
                    {
                        if (Data.Attendance?[i, j] != "0")
                        {
                            DayOff = false;
                            break;
                        }
                    }

                    if (DayOff)
                    {
                        foreach (var Data in AttendanceStorage)
                        {
                            Data.Attendance![i, j] = "";
                        }
                    }
                }
            }
        }
    }
}