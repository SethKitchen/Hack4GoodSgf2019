using System;

using PortalToWork.Models;

namespace PortalToWork.ViewModels
{
    public class ItemDetailViewModel : BaseViewModel
    {
        public Job Job { get; set; }
        public ItemDetailViewModel(Job job = null)
        {
            Title = job?.JobTitle;
            Job = job;
        }
    }
}
