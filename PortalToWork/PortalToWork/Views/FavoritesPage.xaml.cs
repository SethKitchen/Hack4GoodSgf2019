using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PortalToWork.Models;
using PortalToWork.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PortalToWork.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class FavoritesPage : ContentPage
    {
        JobsViewModel viewModel;

        public FavoritesPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new JobsViewModel();

        }

        public async Task<RootObject> GetAllJobs()
        {
            string link = "https://jobs.api.sgf.dev/";
            using (var httpClient = new HttpClient { BaseAddress = new Uri(link) })
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await httpClient.GetAsync("api/job?api_token=h2iv8WsrWurhG0SlRBXDZWUgOG7noUYbn7wlRuJHiFsDsmtelN7PDbu1tuHNO7wzY9XEheAlc1wyjLkW").ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (!string.IsNullOrWhiteSpace(json) && json != "ERROR")
                    {
                        var results = JsonConvert.DeserializeObject<RootObject>(json);
                        return results;

                    }
                }
            }
            return new RootObject();
        }

        private Tuple<double, double> GetBikeTimeAndDistance(string address)
        {
            string origin = Preferences.Get("myLocation", "");
            string destination = address;
            string url = "https://maps.googleapis.com/maps/api/distancematrix/json?origins=" + origin + "&destinations=" + destination + "&mode=bicycling&language=en-US&key=" + Constants.GoogleMapsApiKey;
            try
            {
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        var json = reader.ReadToEnd();
                        var results = JsonConvert.DeserializeObject<RootObject2>(json);
                        if (results != null && results.rows != null && results.rows.Count > 0 && results.rows[0].elements != null && results.rows[0].elements.Count > 0 && results.rows[0].elements[0] != null)
                        {
                            string duration = results.rows[0].elements[0].duration.text;
                            string[] durationSplit = duration.Replace(" hours ", " ").Replace(" hrs ", " ").Replace(" hour ", " ").Replace(" hr ", " ").Replace("mins", "").Replace("min", "").Split(' ');
                            if (durationSplit.Length == 2 && durationSplit[1] != "")
                            {
                                duration = (60 * double.Parse(durationSplit[0]) + double.Parse(durationSplit[1])) + "";
                            }
                            else
                            {
                                duration = durationSplit[0];
                            }
                            string distance = results.rows[0].elements[0].distance.text.Replace("kms", "").Replace("mis", "").Replace("fts", "").Replace("ms", "").Replace("km", "").Replace("mi", "").Replace("ft", "").Replace("m", "");
                            return new Tuple<double, double>(double.Parse(duration), double.Parse(distance));
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return new Tuple<double, double>(double.MaxValue, double.MaxValue);
        }

        private Tuple<double, double> GetDriveTimeAndDistance(string address)
        {
            string origin = Preferences.Get("myLocation", "");
            string destination = address;
            string url = "https://maps.googleapis.com/maps/api/distancematrix/json?origins=" + origin + "&destinations=" + destination + "&mode=driving&language=en-US&key=" + Constants.GoogleMapsApiKey;
            try
            {
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        var json = reader.ReadToEnd();
                        var results = JsonConvert.DeserializeObject<RootObject2>(json);
                        if (results != null && results.rows != null && results.rows.Count > 0 && results.rows[0].elements != null && results.rows[0].elements.Count > 0 && results.rows[0].elements[0] != null)
                        {
                            string duration = results.rows[0].elements[0].duration.text;
                            string[] durationSplit = duration.Replace(" hours ", " ").Replace(" hrs ", " ").Replace(" hour ", " ").Replace(" hr ", " ").Replace("mins", "").Replace("min", "").Split(' ');
                            if (durationSplit.Length == 2 && durationSplit[1] != "")
                            {
                                duration = (60 * double.Parse(durationSplit[0]) + double.Parse(durationSplit[1])) + "";
                            }
                            else
                            {
                                duration = durationSplit[0];
                            }
                            string distance = results.rows[0].elements[0].distance.text.Replace("kms", "").Replace("mis", "").Replace("fts", "").Replace("ms", "").Replace("km", "").Replace("mi", "").Replace("ft", "").Replace("m", "");
                            return new Tuple<double, double>(double.Parse(duration), double.Parse(distance));
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return new Tuple<double, double>(double.MaxValue, double.MaxValue);
        }

        private Tuple<double, double> GetBusTimeAndDistance(string address)
        {
            string origin = Preferences.Get("myLocation", "");
            string destination = address;
            string url = "https://maps.googleapis.com/maps/api/distancematrix/json?origins=" + origin + "&destinations=" + destination + "&mode=transit&language=en-US&key=" + Constants.GoogleMapsApiKey;
            try
            {
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        var json = reader.ReadToEnd();
                        var results = JsonConvert.DeserializeObject<RootObject2>(json);
                        if (results != null && results.rows != null && results.rows.Count > 0 && results.rows[0].elements != null && results.rows[0].elements.Count > 0 && results.rows[0].elements[0] != null)
                        {
                            string duration = results.rows[0].elements[0].duration.text;
                            string[] durationSplit = duration.Replace(" hours ", " ").Replace(" hrs ", " ").Replace(" hour ", " ").Replace(" hr ", " ").Replace("mins", "").Replace("min", "").Split(' ');
                            if (durationSplit.Length == 2 && durationSplit[1] != "")
                            {
                                duration = (60 * double.Parse(durationSplit[0]) + double.Parse(durationSplit[1])) + "";
                            }
                            else
                            {
                                duration = durationSplit[0];
                            }
                            string distance = results.rows[0].elements[0].distance.text.Replace("kms", "").Replace("mis", "").Replace("fts", "").Replace("ms", "").Replace("km", "").Replace("mi", "").Replace("ft", "").Replace("m", "");
                            return new Tuple<double, double>(double.Parse(duration), double.Parse(distance));
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return new Tuple<double, double>(double.MaxValue, double.MaxValue);
        }

        private Tuple<double, double> GetWalkTimeAndDistance(string address)
        {
            string origin = Preferences.Get("myLocation", "");
            string destination = address;
            string url = "https://maps.googleapis.com/maps/api/distancematrix/json?origins=" + origin + "&destinations=" + destination + "&mode=walking&language=en-US&key=" + Constants.GoogleMapsApiKey;
            try
            {
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        var json = reader.ReadToEnd();
                        var results = JsonConvert.DeserializeObject<RootObject2>(json);
                        if (results != null && results.rows != null && results.rows.Count > 0 && results.rows[0].elements != null && results.rows[0].elements.Count > 0 && results.rows[0].elements[0] != null)
                        {
                            string duration = results.rows[0].elements[0].duration.text;
                            string[] durationSplit = duration.Replace(" hours ", " ").Replace(" hrs ", " ").Replace(" hour ", " ").Replace(" hr ", " ").Replace("mins", "").Replace("min", "").Split(' ');
                            if (durationSplit.Length == 2 && durationSplit[1] != "")
                            {
                                duration = (60 * double.Parse(durationSplit[0]) + double.Parse(durationSplit[1])) + "";
                            }
                            else
                            {
                                duration = durationSplit[0];
                            }
                            string distance = results.rows[0].elements[0].distance.text.Replace("kms", "").Replace("mis", "").Replace("fts", "").Replace("ms", "").Replace("km", "").Replace("mi", "").Replace("ft", "").Replace("m", "");
                            return new Tuple<double, double>(double.Parse(duration), double.Parse(distance));
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return new Tuple<double, double>(double.MaxValue, double.MaxValue);
        }

        private async void GetJobs()
        {
            List<string> jobIDs = new List<string>();
            int numFavorites = Preferences.Get("favorites", 0);
            if (numFavorites > 0)
            {
                for(int i=0; i<numFavorites; i++)
                {
                    jobIDs.Add(Preferences.Get("favorite" + (i + 1), "-1"));
                }

                var unparsedJobs = await GetAllJobs();
                viewModel.ClearJobsAsync();
                List<Datum> orderedUnparsedJobs = unparsedJobs.data.OrderBy(x => DateTime.Parse(x.date_posted)).Reverse().ToList();
                for (int i = 0; i < orderedUnparsedJobs.Count; i++)
                {
                    string jobID = orderedUnparsedJobs[i].job_id+"";
                    if (jobIDs.Contains(jobID))
                    {
                        var lat = (orderedUnparsedJobs[i].locations.data[0] as JObject).GetValue("lat");
                        var longi = (orderedUnparsedJobs[i].locations.data[0] as JObject).GetValue("lng");
                        var address = lat + "," + longi;
                        var bikeTime = Preferences.Get("jobBikeTimeFromId" + orderedUnparsedJobs[i].id, double.MaxValue);
                        if (bikeTime == double.MaxValue)
                        {
                            var toCheck = orderedUnparsedJobs[i].locations.data[0];
                            Tuple<double, double> BikeTimeAndDistance = GetBikeTimeAndDistance(address);
                            Preferences.Set("jobBikeTimeFromId" + orderedUnparsedJobs[i].id, BikeTimeAndDistance.Item1);
                            Preferences.Set("jobBikeDistanceFromId" + orderedUnparsedJobs[i].id, BikeTimeAndDistance.Item2);
                            bikeTime = BikeTimeAndDistance.Item1;
                        }
                        var driveTime = Preferences.Get("jobDriveTimeFromId" + orderedUnparsedJobs[i].id, double.MaxValue);
                        if (driveTime == double.MaxValue)
                        {
                            var toCheck = orderedUnparsedJobs[i].locations.data[0];
                            Tuple<double, double> DriveTimeAndDistance = GetDriveTimeAndDistance(address);
                            Preferences.Set("jobDriveTimeFromId" + orderedUnparsedJobs[i].id, DriveTimeAndDistance.Item1);
                            Preferences.Set("jobDriveDistanceFromId" + orderedUnparsedJobs[i].id, DriveTimeAndDistance.Item2);
                            driveTime = DriveTimeAndDistance.Item1;
                        }
                        var walkTime = Preferences.Get("jobWalkTimeFromId" + orderedUnparsedJobs[i].id, double.MaxValue);
                        if (walkTime == double.MaxValue)
                        {
                            var toCheck = orderedUnparsedJobs[i].locations.data[0];
                            Tuple<double, double> WalkTimeAndDistance = GetWalkTimeAndDistance(address);
                            Preferences.Set("jobWalkTimeFromId" + orderedUnparsedJobs[i].id, WalkTimeAndDistance.Item1);
                            Preferences.Set("jobWalkTimeFromId" + orderedUnparsedJobs[i].id, WalkTimeAndDistance.Item2);
                            walkTime = WalkTimeAndDistance.Item1;
                        }
                        var busTime = Preferences.Get("jobBusTimeFromId" + orderedUnparsedJobs[i].id, double.MaxValue);
                        if (busTime == double.MaxValue)
                        {
                            var toCheck = orderedUnparsedJobs[i].locations.data[0];
                            Tuple<double, double> BusTimeAndDistance = GetBusTimeAndDistance(address);
                            Preferences.Set("jobBusTimeFromId" + orderedUnparsedJobs[i].id, BusTimeAndDistance.Item1);
                            Preferences.Set("jobBusTimeFromId" + orderedUnparsedJobs[i].id, BusTimeAndDistance.Item2);
                            busTime = BusTimeAndDistance.Item1;
                        }
                        Job j = new Job();
                        j.BikeTime = bikeTime + "";
                        j.BusTime = busTime + "";
                        j.DriveTime = driveTime + "";
                        j.WalkTime = walkTime + "";
                        j.EmployerName = orderedUnparsedJobs[i].employer.name;
                        if (orderedUnparsedJobs[i].job_type != null && orderedUnparsedJobs[i].job_type.Contains("full"))
                        {
                            j.JobType = JobTypes.FullTime;
                        }
                        else if (orderedUnparsedJobs[i].job_type != null && orderedUnparsedJobs[i].job_type.Contains("part"))
                        {
                            j.JobType = JobTypes.PartTime;
                        }
                        else
                        {
                            j.JobType = JobTypes.Seasonal;
                        }
                        j.Index = i + 1;
                        j.JobTitle = orderedUnparsedJobs[i].title;
                        j.PayRate = orderedUnparsedJobs[i].pay_rate;
                        j.SiteURL = orderedUnparsedJobs[i].url;
                        j.Description = orderedUnparsedJobs[i].description;
                        j.Latitude = lat + "";
                        j.Longitude = longi + "";
                        j.DateExpires = DateTime.Parse(orderedUnparsedJobs[i].date_expires);
                        j.DatePosted = DateTime.Parse(orderedUnparsedJobs[i].date_posted);
                        var reqJobType = (JobTypes)Preferences.Get("jobType", (int)JobTypes.All);
                        var reqEducation = (Educations)Preferences.Get("educationReq", (int)Educations.HighSchoolOREquivalent);
                        var maxTimeEnum = (TravelTimes)Preferences.Get("travelTime", (int)TravelTimes.Sixty);
                        var maxTime = double.MaxValue;
                        if (maxTimeEnum == TravelTimes.Sixty)
                        {
                            maxTime = 60;
                        }
                        else if (maxTimeEnum == TravelTimes.FortyFive)
                        {
                            maxTime = 45;
                        }
                        else if (maxTimeEnum == TravelTimes.Thirty)
                        {
                            maxTime = 30;
                        }
                        else if (maxTimeEnum == TravelTimes.Fifteen)
                        {
                            maxTime = 15;
                        }
                        var maxDriveTime = maxTime;//Preferences.Get("maxDriveTime", double.MaxValue);
                        var maxWalkTime = maxTime;// Preferences.Get("maxWalkTime", double.MaxValue);
                        var maxBusTime = maxTime;// Preferences.Get("maxBusTime", double.MaxValue);
                        var maxBikeTime = maxTime;// Preferences.Get("maxBikeTime", double.MaxValue);
                        var driveOn = Preferences.Get("driveOn", true);
                        var walkOn = Preferences.Get("walkOn", true);
                        var bikeOn = Preferences.Get("bikeOn", true);
                        var busOn = Preferences.Get("busOn", true);
                        j.DriveSwitchOn = driveOn;
                        j.WalkSwitchOn = walkOn;
                        j.BikeSwitchOn = bikeOn;
                        j.BusSwitchOn = busOn;
                        viewModel.AddItemAsync(j);
                    }
                }
            }
            viewModel.IsDataBusy = false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            viewModel.IsDataBusy = true;
            GetJobs();

        }


        async void OnJobSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as Job;
            if (item == null)
                return;

            await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));

            // Manually deselect item.
            JobsListView.SelectedItem = null;
        }

    }
}