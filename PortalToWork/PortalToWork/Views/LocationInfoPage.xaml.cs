using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using PortalToWork.Models;
using PortalToWork.Views;
using PortalToWork.ViewModels;
using Xamarin.Essentials;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using PortalToWork.Services;
using System.Net;
using System.IO;
using System.Data;
using System.Xml.Linq;

namespace PortalToWork.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class LocationInfoPage : ContentPage
    {
        JobsViewModel viewModel;
        int jobOffset = 0;


        public const string JobsAPIKEY = "h2iv8WsrWurhG0SlRBXDZWUgOG7noUYbn7wlRuJHiFsDsmtelN7PDbu1tuHNO7wzY9XEheAlc1wyjLkW";
        public LocationInfoPage()
        {

            var jobType=(JobTypes)Preferences.Get("jobType", (int)JobTypes.All);
            var eduReq=(Educations)Preferences.Get("educationReq", (int)Educations.HighSchoolOREquivalent);
            var travelTime=(TravelTimes)Preferences.Get("travelTime", (int)TravelTimes.Sixty);
            bool isWalkOn = Preferences.Get("walkOn", true);
            bool isBikeOn= Preferences.Get("bikeOn", true);
            bool isBusOn = Preferences.Get("busOn", true);
            bool isDriveOn = Preferences.Get("driveOn", true);

            InitializeComponent();

            typePicker.SelectedItem = jobType;
            educationPicker.SelectedItem = eduReq;
            travelTimePicker.SelectedItem = travelTime;
            walkSwitch.IsToggled = isWalkOn;
            bikeSwitch.IsToggled = isBikeOn;
            busSwitch.IsToggled = isBusOn;
            driveSwitch.IsToggled = isDriveOn;

            BindingContext = viewModel = new JobsViewModel();
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (JobsNearMe.IsVisible)
            {
                JobsNearMe.IsVisible = false;
                SetLocation.IsVisible = true;
                JobSearchSettings.IsVisible = false;
            }
            else if (JobSearchSettings.IsVisible)
            {
                viewModel.IsDataBusy = true;
                jobOffset = 0;
                GetJobs(jobOffset);
                jobOffset += 5;
            }
            else
            {
                SetLocation.IsVisible = true;
                JobSearchSettings.IsVisible = false;
                JobsNearMe.IsVisible = false;
            }
        }

        private async void WrenchTapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            SetLocation.IsVisible = false;
            JobSearchSettings.IsVisible = true;
            JobsNearMe.IsVisible = false;
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

        private const string ApiBaseAddress = "https://maps.googleapis.com/maps/";
        private HttpClient CreateClient()
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(ApiBaseAddress)
            };

            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
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

    

        public async Task<GooglePlaceAutoCompleteResult> GetPlaces(string text)
        {
            GooglePlaceAutoCompleteResult results = null;

            using (var httpClient = CreateClient())
            {
                var response = await httpClient.GetAsync($"api/place/autocomplete/json?input={Uri.EscapeUriString(text)}&key={Constants.GoogleMapsApiKey}").ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    if (!string.IsNullOrWhiteSpace(json) && json != "ERROR")
                    {
                        results = await Task.Run(() =>
                           JsonConvert.DeserializeObject<GooglePlaceAutoCompleteResult>(json)
                        ).ConfigureAwait(false);

                    }
                }
            }

            return results;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

             if (viewModel.Jobs.Count == 0)
                 viewModel.LoadJobsCommand.Execute(null);
        }

        private async void UseCurrentButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                viewModel.IsDataBusy = true;
                var request = new GeolocationRequest(GeolocationAccuracy.High);
                var location = await Geolocation.GetLocationAsync(request);
                var mine = location.Latitude + "," + location.Longitude;
                Preferences.Set("myLocation", mine);
                GetJobs(jobOffset);
                jobOffset += 5;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Unable to get actual location", "Ok");
            }
        }

        private Tuple<double, double> GetBikeTimeAndDistance(string address)
        {
            string origin = Preferences.Get("myLocation", "");
            string destination = address;
            string url = "https://maps.googleapis.com/maps/api/distancematrix/json?origins="+origin+"&destinations="+destination+"&mode=bicycling&language=en-US&key="+ Constants.GoogleMapsApiKey;
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
                            if (durationSplit.Length == 2 && durationSplit[1]!="")
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
            catch(Exception ex)
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



        private void ShowJobs()
        {
            viewModel.LoadJobsCommand.Execute(null);
            JobsNearMe.IsVisible = true;
            SetLocation.IsVisible = false;
            JobSearchSettings.IsVisible = false;
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


        private async void GetJobs(int startIndex)
        {
            var unparsedJobs = await GetAllJobs();
            viewModel.ClearJobsAsync();
            List<Datum> orderedUnparsedJobs=unparsedJobs.data.OrderBy(x => DateTime.Parse(x.date_posted)).Reverse().ToList();
            for (int i = startIndex; i < startIndex+5; i++)
            {
                if (orderedUnparsedJobs[i].locations != null && orderedUnparsedJobs[i].locations.data != null && orderedUnparsedJobs[i].locations.data.Count > 0)
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
                    if (orderedUnparsedJobs[i].job_type !=null && orderedUnparsedJobs[i].job_type.Contains("full"))
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
                    j.Index = i+1;
                    j.JobTitle = orderedUnparsedJobs[i].title;
                    j.JobID = orderedUnparsedJobs[i].job_id+"";
                    j.PayRate = orderedUnparsedJobs[i].pay_rate;
                    j.SiteURL = orderedUnparsedJobs[i].url;
                    j.Description = orderedUnparsedJobs[i].description;
                    j.Latitude = lat+"";
                    j.Longitude = longi+"";
                    j.DateExpires = DateTime.Parse(orderedUnparsedJobs[i].date_expires);
                    j.DatePosted = DateTime.Parse(orderedUnparsedJobs[i].date_posted);
                    var reqJobType = (JobTypes)Preferences.Get("jobType", (int)JobTypes.All);
                    var reqEducation = (Educations)Preferences.Get("educationReq", (int)Educations.HighSchoolOREquivalent);
                    var maxTimeEnum = (TravelTimes)Preferences.Get("travelTime", (int)TravelTimes.Sixty);
                    var maxTime = double.MaxValue;
                    if(maxTimeEnum==TravelTimes.Sixty)
                    {
                        maxTime = 60;
                    }
                    else if(maxTimeEnum==TravelTimes.FortyFive)
                    {
                        maxTime = 45;
                    }
                    else if(maxTimeEnum==TravelTimes.Thirty)
                    {
                        maxTime = 30;
                    }
                    else if(maxTimeEnum==TravelTimes.Fifteen)
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
                    if(busTime>400 || driveTime>400 || bikeTime>400 || walkTime>400)
                    {
                        continue;
                    }
                    if (j.JobType == reqJobType || reqJobType == JobTypes.All)
                    {
                        if (j.RequiredEducation == reqEducation || j.RequiredEducation == Educations.HighSchoolOREquivalent)
                        {
                            if (busOn)
                            {
                                if (busTime < maxBusTime)
                                {
                                    viewModel.AddItemAsync(j);
                                    continue;
                                }
                            }
                            if (driveOn)
                            {
                                if (driveTime < maxDriveTime)
                                {
                                    viewModel.AddItemAsync(j);
                                    continue;
                                }
                            }
                            if (walkOn)
                            {
                                if (walkTime < maxWalkTime)
                                {
                                    viewModel.AddItemAsync(j);
                                    continue;
                                }
                            }
                            if (bikeOn)
                            {
                                if (bikeTime < maxBikeTime)
                                {
                                    viewModel.AddItemAsync(j);
                                    continue;
                                }
                            }
                        }
                    }
                }
                else
                {

                }
            }
            ShowJobs();
            viewModel.IsDataBusy = false;
        }

        private async void UseThisAddressButton_Clicked(object sender, EventArgs e)
        {
            viewModel.IsDataBusy = true;
            var addressResult = await GetPlaces(streetAddressLineOneEntry.Text + "," + streetAddressLineTwoEntry.Text + "," + cityEntry.Text + "," + postalCodeEntry.Text);
            if (addressResult != null && addressResult.AutoCompletePlaces != null && addressResult.AutoCompletePlaces.Count > 0 && addressResult.AutoCompletePlaces[0].Description != "")
            {
                var mine = addressResult.AutoCompletePlaces[0].Description;
                Preferences.Set("myLocation", mine);
                GetJobs(jobOffset);
                jobOffset += 5;
               
            }
            else
            {

            }
        }

        private async void LoadMore_Clicked(object sender, EventArgs e)
        {
            viewModel.IsDataBusy = true;
            GetJobs(jobOffset);
            jobOffset += 5;
        }

        public void UpdateSettings()
        {
            Preferences.Set("jobType", (int)typePicker.SelectedItem);
            Preferences.Set("educationReq", (int)educationPicker.SelectedItem);
            Preferences.Set("travelTime", (int)travelTimePicker.SelectedItem);
            Preferences.Set("walkOn", walkSwitch.IsToggled);
            Preferences.Set("bikeOn", bikeSwitch.IsToggled);
            Preferences.Set("busOn", busSwitch.IsToggled);
            Preferences.Set("driveOn", driveSwitch.IsToggled);
        }

        private void EducationPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSettings();
        }

        private void WalkSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            UpdateSettings();
        }
    }
}