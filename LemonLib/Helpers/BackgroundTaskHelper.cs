using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace LemonLib.Helpers
{
    public class BackgroundTaskHelper
    {
        public static async Task<bool> RegisterAsync(string name, string entryPoint, IBackgroundTrigger trigger, Action onCompleted, Action<uint> onProgress = null)
        {
            BackgroundTaskRegistration registration = await RegisterAsync(name, entryPoint, trigger);
            if (registration == null)
            {
                return false;
            }
            registration.Completed += (s, a) =>
            {
                onCompleted();
            };
            if (onProgress != null)
            {
                registration.Progress += (s, a) =>
                {
                    onProgress(a.Progress);
                };
            }
            return true;
        }

        public static async Task<BackgroundTaskRegistration> RegisterAsync(string name, string entryPoint, IBackgroundTrigger trigger)
        {
            if (!(await Unregister(name)))
                return null;

            BackgroundTaskBuilder builder = new BackgroundTaskBuilder()
            {
                Name = name,
                TaskEntryPoint = entryPoint
            };

            builder.SetTrigger(trigger);

            BackgroundTaskRegistration registration = builder.Register();
            return registration;
        }

        public static async Task<bool> Unregister(string name)
        {
            RemoveAccess();
            var access = await RequestAccessAsync();
            if (access == BackgroundAccessStatus.Denied)
                return false;
            foreach (var t in GetAllTasks())
            {
                if (t.Value.Name == name)
                {
                    t.Value.Unregister(true);
                    return true;
                }
            }
            return false;
        }

        public static void RemoveAccess()
        {
            BackgroundExecutionManager.RemoveAccess();
        }

        public static async Task<BackgroundAccessStatus> RequestAccessAsync()
        {
            return await BackgroundExecutionManager.RequestAccessAsync();
        }

        public static IReadOnlyDictionary<Guid, IBackgroundTaskRegistration> GetAllTasks()
        {
            return BackgroundTaskRegistration.AllTasks;
        }
    }
}
