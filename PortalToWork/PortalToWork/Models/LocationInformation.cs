using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PortalToWork.Models
{
    public enum Educations
    {
        [Description("High School Or Equivalent")]
        HighSchoolOREquivalent,
        [Description("Some College")]
        SomeCollege,
        [Description("Bachelor's Degree")]
        BachelorsDegree,
        [Description("Master's Degree or Higher")]
        MastersDegree,
        [Description("Other")]
        Other
    }

    public class MyEducationStateConverter : IMarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Educations)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class MyJobTypeStateConverter : IMarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((JobTypes)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public class MyTravelTimeStateConverter : IMarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((TravelTimes)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }


    public class MyOnConverter : IMarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }

    public enum JobTypes
    {
        [Description("All")]
        All,
        [Description("Full Time")]
        FullTime,
        [Description("Part Time")]
        PartTime,
        [Description("Seasonal")]
        Seasonal
    }

    public enum TravelTimes
    {
        [Description("15 min")]
        Fifteen,
        [Description("30 min")]
        Thirty,
        [Description("45 min")]
        FortyFive,
        [Description("60 min")]
        Sixty
    }

    public enum States
    {
        [Description("Alabama")]
        AL,
        [Description("Alaska")]
        AK,
        [Description("Arkansas")]
        AR,
        [Description("Arizona")]
        AZ,
        [Description("California")]
        CA,
        [Description("Colorado")]
        CO,
        [Description("Connecticut")]
        CT,
        [Description("D.C.")]
        DC,
        [Description("Delaware")]
        DE,
        [Description("Florida")]
        FL,
        [Description("Georgia")]
        GA,
        [Description("Hawaii")]
        HI,
        [Description("Iowa")]
        IA,
        [Description("Idaho")]
        ID,
        [Description("Illinois")]
        IL,
        [Description("Indiana")]
        IN,
        [Description("Kansas")]
        KS,
        [Description("Kentucky")]
        KY,
        [Description("Louisiana")]
        LA,
        [Description("Massachusetts")]
        MA,
        [Description("Maryland")]
        MD,
        [Description("Maine")]
        ME,
        [Description("Michigan")]
        MI,
        [Description("Minnesota")]
        MN,
        [Description("Missouri")]
        MO,
        [Description("Mississippi")]
        MS,
        [Description("Montana")]
        MT,
        [Description("North Carolina")]
        NC,
        [Description("North Dakota")]
        ND,
        [Description("Nebraska")]
        NE,
        [Description("New Hampshire")]
        NH,
        [Description("New Jersey")]
        NJ,
        [Description("New Mexico")]
        NM,
        [Description("Nevada")]
        NV,
        [Description("New York")]
        NY,
        [Description("Oklahoma")]
        OK,
        [Description("Ohio")]
        OH,
        [Description("Oregon")]
        OR,
        [Description("Pennsylvania")]
        PA,
        [Description("Rhode Island")]
        RI,
        [Description("South Carolina")]
        SC,
        [Description("South Dakota")]
        SD,
        [Description("Tennessee")]
        TN,
        [Description("Texas")]
        TX,
        [Description("Utah")]
        UT,
        [Description("Virginia")]
        VA,
        [Description("Vermont")]
        VT,
        [Description("Washington")]
        WA,
        [Description("Wisconsin")]
        WI,
        [Description("West Virginia")]
        WV,
        [Description("Wyoming")]
        WY
    }

    public class Employer
    {
        public int id { get; set; }
        public string name { get; set; }
        public string category { get; set; }
        public int naics { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

    public class Links
    {
        public string self { get; set; }
    }

    public class Locations
    {
        public List<object> data { get; set; }
        public Links links { get; set; }
    }

    public class Datum
    {
        public int id { get; set; }
        public string date_posted { get; set; }
        public string date_updated { get; set; }
        public string date_expires { get; set; }
        public int employer_id { get; set; }
        public Employer employer { get; set; }
        public Locations locations { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string job_type { get; set; }
        public int job_id { get; set; }
        public string pay_rate { get; set; }
        public string req_education { get; set; }
        public string data_source { get; set; }
        public string data_site { get; set; }
        public string url { get; set; }
        public int fake { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
    }

    public class Links2
    {
        public string self { get; set; }
    }

    public class RootObject
    {
        public List<Datum> data { get; set; }
        public Links2 links { get; set; }
    }

    public class Distance
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class Duration
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class Element
    {
        public Distance distance { get; set; }
        public Duration duration { get; set; }
        public string status { get; set; }
    }

    public class Row
    {
        public List<Element> elements { get; set; }
    }

    public class RootObject2
    {
        public List<string> destination_addresses { get; set; }
        public List<string> origin_addresses { get; set; }
        public List<Row> rows { get; set; }
        public string status { get; set; }
    }

    public class Job
    {
        public int Index { get; set; }
        public DateTime DatePosted { get; set; }
        public DateTime DateExpires { get; set; }
        public string EmployerName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string JobTitle { get; set; }
        public JobTypes JobType { get; set; }
        public string JobID { get; set; }
        public string PayRate { get; set; }
        public Educations RequiredEducation { get; set; }
        public string SiteURL { get; set; }
        public string WalkTime { get; set; }
        public string BikeTime { get; set; }
        public string DriveTime { get; set; }
        public string BusTime { get; set; }
        public bool BikeSwitchOn { get; set; }
        public bool WalkSwitchOn { get; set; }
        public bool DriveSwitchOn { get; set; }
        public bool BusSwitchOn { get; set; }
        public string Description { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}