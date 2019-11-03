using PortalToWork.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PortalToWork.ViewModels
{
    public class JobsViewModel:BaseViewModel
    {
        public ObservableCollection<Job> Jobs { get; set; }
        public Command LoadJobsCommand { get; set; }

        public JobsViewModel()
        {
            Title = "Loading Jobs...";
            Jobs = new ObservableCollection<Job>();
            LoadJobsCommand = new Command(async () => await ExecuteLoadJobsCommand());
        }

        async Task ExecuteLoadJobsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Jobs.Clear();
                var trackers = await DataStore.GetItemsAsync(true);
                foreach (var tracker in trackers)
                {
                    Jobs.Add(tracker);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async void AddItemAsync(Job j)
        {
            Jobs.Add(j);
            await DataStore.AddItemAsync(j);
        }

        public async void ClearJobsAsync()
        {
            Jobs.Clear();
            await DataStore.ClearItemsAsync();
        }
    }
}
