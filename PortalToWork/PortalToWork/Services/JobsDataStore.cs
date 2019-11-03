using PortalToWork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalToWork.Services
{
    public class JobsDataStore : IDataStore<Job>
    {
        List<Job> jobs;

        public JobsDataStore()
        {
            jobs = new List<Job>();
        }

        public async Task<bool> AddItemAsync(Job item)
        {
            jobs.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> ClearItemsAsync()
        {
            jobs.Clear();
            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldJob = jobs.Where((Job arg) => arg.JobID == id).FirstOrDefault();
            jobs.Remove(oldJob);

            return await Task.FromResult(true);
        }

        public async Task<Job> GetItemAsync(string id)
        {
            return await Task.FromResult(jobs.FirstOrDefault(s => s.JobID == id));
        }

        public async Task<IEnumerable<Job>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(jobs);
        }

        public async Task<bool> UpdateItemAsync(Job item)
        {
            var oldJob = jobs.Where((Job arg) => arg.JobID == item.JobID).FirstOrDefault();
            int index = jobs.IndexOf(oldJob);
            if (index > -1)
            {
                jobs[index] = item;
            }

            return await Task.FromResult(true);
        }
    }
}
