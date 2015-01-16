namespace BingDailyPicture.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Annotations;
    using Services;
    using TaskScheduler = Services.TaskScheduler;

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        readonly LockScreenManager lockScreenManager;
        ImageSource image;
        bool isUpdating;

        public MainWindowViewModel()
        {
            lockScreenManager = new LockScreenManager();

            UpdateNow = new DelegateCommand(ApplyLatestPictureNow);
            ScheduleTask = new DelegateCommand(Schedule);
        }
        
        void Schedule(object o)
        {
            TaskScheduler.Install();
        }

        async void ApplyLatestPictureNow(object o)
        {
            try
            {
                IsUpdating = true;

                await lockScreenManager.RunAsync();

                var uiContext = System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext();

                await Task.Factory.StartNew(delegate
                {
                    var picturePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "LockScreen.jpg");
                    var bitmap = new BitmapImage(new Uri(picturePath));
                    Image = bitmap;
                }, CancellationToken.None, TaskCreationOptions.None, uiContext);
            }
            finally
            {
                IsUpdating = false;
            }

        }

        public async Task ApplyLatestPicture()
        {
            await lockScreenManager.RunAsync();
        }

        public ICommand UpdateNow { get; set; }

        public ICommand ScheduleTask { get; set; }

        public bool IsUpdating
        {
            get { return isUpdating; }
            set
            {
                if (value.Equals(isUpdating)) return;
                isUpdating = value;
                OnPropertyChanged();
            }
        }

        public ImageSource Image
        {
            get { return image; }
            private set
            {
                if (Equals(value, image)) return;
                image = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}