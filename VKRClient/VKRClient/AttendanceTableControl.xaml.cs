using VKRClient.Models;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace VKRClient
{
    public partial class AttendanceTableControl : UserControl
    {
        public ObservableCollection<AttendanceData> AttendanceCollection { get; set; } = new ObservableCollection<AttendanceData>();

        public AttendanceTableControl()
        {
            InitializeComponent();
        }

        public void LoadTable(List<AttendanceData> AttendanceData)
        {
            AttendanceCollection.Clear();

            foreach (var Data in AttendanceData)
            {
                AttendanceCollection.Add(Data);
            }

            DataContext = this;
        }
    }
}
