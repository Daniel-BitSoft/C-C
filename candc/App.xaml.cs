using AutoMapper;
using CC.Providers;
using System.Windows;

namespace CC
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static User LoggedInUser { get; set; }

        public static Entities dbcontext = new Entities();
        public static IMapper mapper;

        // providers
        public static AntigensProvider AntigensProvider = new AntigensProvider();
        public static ArrayProvider ArrayProvider = new ArrayProvider();
        public static CCProvider CalControlProvider = new CCProvider();
        public static UserProvider UserProvider = new UserProvider();

        // pages
        public static AntigenPage AntigenPage = new AntigenPage();
        public static ArrayPage arrayPage = new ArrayPage();
        public static CCPage cCPage = new CCPage();
        public static BatchPage batchPage = new BatchPage();
        public static UserManagementPage userMgmtPage = new UserManagementPage();
        public static ReportPage reportPage = new ReportPage();
        public static UserPage userPage = new UserPage();

        protected override void OnStartup(StartupEventArgs e)
        {
            InitializeAutoMapper();
            base.OnStartup(e);
        }

        private void InitializeAutoMapper()
        {
            var mapperConfig = GetMapperDefinition();
            mapper = mapperConfig.CreateMapper();
        }

        public MapperConfiguration GetMapperDefinition()
        {
            return new MapperConfiguration(config =>
            {

            });
        }
    }
}
