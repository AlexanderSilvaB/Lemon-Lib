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
        public static async Task<BackgroundTaskRegistration> RegisterAsync(string name, string entryPoint, IBackgroundTrigger trigger)
        {
            BackgroundExecutionManager.RemoveAccess();
            var access = await BackgroundExecutionManager.RequestAccessAsync();
            if (access == BackgroundAccessStatus.Denied)
                return null;
            foreach (var t in BackgroundTaskRegistration.AllTasks)
            {
                if (t.Value.Name == name)
                    t.Value.Unregister(true);
            }

            BackgroundTaskBuilder builder = new BackgroundTaskBuilder()
            {
                Name = name,
                TaskEntryPoint = entryPoint
            };

            builder.SetTrigger(trigger);

            BackgroundTaskRegistration registration = builder.Register();
            return registration;
        }
    }
}
