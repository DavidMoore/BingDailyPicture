namespace BingDailyPicture
{
    using System;
    using System.Diagnostics;
    using Microsoft.Win32.TaskScheduler;

    class TaskScheduler
    {
        const string taskPath = "BingDailyPicture";

        internal static void Install()
        {
            // Get the service on the local machine
            using (var service = new TaskService())
            {
                // Create a new task definition and assign properties
                var existingTask = service.GetTask(taskPath);

                using (TaskDefinition task = existingTask != null ? existingTask.Definition : service.NewTask())
                {
                    task.RegistrationInfo.Description = "Sets the lock screen picture to the Bing picture of the day";

                    task.Triggers.Clear();

                    // Schedule for midnight, then check every 12 hours (i.e. lunch time and midnight each day)
                    var trigger = new TimeTrigger(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0));
                    trigger.Repetition.Interval = TimeSpan.FromHours(12);
                    task.Triggers.Add(trigger);

                    task.Actions.Clear();
                    task.Actions.Add(new ExecAction(Process.GetCurrentProcess().MainModule.FileName, "/silent"));

                    task.Settings.RunOnlyIfNetworkAvailable = true;
                    task.Settings.StartWhenAvailable = true;
                    task.Settings.WakeToRun = false;
                    task.Principal.RunLevel = TaskRunLevel.Highest;

                    if (existingTask == null)
                    {
                        service.RootFolder.RegisterTaskDefinition(taskPath, task);
                    }
                    else
                    {
                        existingTask.RegisterChanges();
                    }
                }
            }
        }

    }
}