using System;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Locations;
using Android.Runtime;

using MySql.Data.MySqlClient;
using NationalMonuments.Model;
using Android.Graphics;
using System.Net;

namespace NationalMonuments
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/nationalmonuments")]
    public class MainActivity : Activity, ILocationListener
    {
        // Initialisation
        MySqlConnection con; // MySql Connection init
        List<Monument> monumentList = new List<Monument>();
        LocationManager locMgr; //Location Manager init
        Location currentLocation;//this is the current location info init

        TextView lblConnection; //Provider label init
        TextView txtConnection; //Provider text init
        TextView lblLocation;   //Location label init
        TextView txtLocation;   //Location text init
        TextView lblAddress;    //Address label init
        TextView txtAddress;    //Address text init
        TextView lblCity;       //City label init
        TextView txtCity;       //City text init

        ImageView imgMonument1;  //Monument Image init
        TextView txtName1;       //Monument Name text init
        TextView txtRange1;      //Monument Range text init
        Button btnInfo1;         //Monument Info button init
        Button btnGoTo1;         //Monument GoTo button init
            

        ImageView imgMonument2;  //Monument Image init
        TextView txtName2;       //Monument Name text init
        TextView txtRange2;      //Monument Range text init
        Button btnInfo2;         //Monument Info button init
        Button btnGoTo2;         //Monument GoTo button init

        ImageView imgMonument3;  //Monument Image init
        TextView txtName3;       //Monument Name text init
        TextView txtRange3;      //Monument Range text init
        Button btnInfo3;         //Monument Info button init
        Button btnGoTo3;         //Monument GoTo button init


        ImageView imgmPic;
        TextView txtmName;      //Monument View text init
        TextView txtmType;
        TextView txtmAddress;
        TextView txtmDescription;
        TextView txtmCity;
        Button btnBack;
        Button btnMoreInfo;

        /*ImageView Loadpic;
        ProgressBar progLoading;
        TextView txtLoading;*/



        bool background;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            background = false;
            SetContentView(Resource.Layout.Main);
            Declaration();
            MySqlConnect();
        }

        protected override void OnResume()
        {
            base.OnResume();
            background = false;
            LocationHelper();

        }

        protected override void OnPause()
        {
            base.OnPause();
            background = true;
        }

        protected override void OnDestroy()
        {
            locMgr.RemoveUpdates(this);
            base.OnDestroy();
        }

        protected void LocationHelper()
        {
            locMgr = GetSystemService(Context.LocationService) as LocationManager;


            var locationCriteria = new Criteria();
            locationCriteria.Accuracy = Accuracy.Coarse;
            locationCriteria.PowerRequirement = Power.Medium;
            string locationProvider = locMgr.GetBestProvider(locationCriteria, true);
            locMgr.RequestLocationUpdates(locationProvider, 3000, 1, this);
        }

        public async void OnLocationChanged(Android.Locations.Location location)
        {
            currentLocation = location;
            txtLocation.Text = $"{Math.Round(currentLocation.Latitude, 4)}, {Math.Round(currentLocation.Longitude, 4)}";
            txtConnection.Text = location.Provider.ToString();
            Address address = await ReverseGeocodeCurrentLocation();

            DisplayAddress(address);
            txtCity.Text = "Габрово";

            Location lChoosen = new Location("Choosen1");
            Location lChoosen2 = new Location("Choosen2");
            Location lChoosen3 = new Location("Choosen3");
            double distance = 9999.00;
            double distance2 = 9999.00;
            double distance3 = 9999.00;
            Monument Choosen = new Monument("Няма паметници в близост.");
            Monument Choosen2 = new Monument("Няма други паметници в близост.");
            Monument Choosen3 = new Monument("Няма паметници в близост.");

            foreach (Monument n in monumentList)
            {
                
                if (Math.Pow(currentLocation.Latitude - n.mGetLat(), 2) + Math.Pow((currentLocation.Longitude - n.mGetLong()), 2) < Math.Pow(0.062, 2))
                {
                    lChoosen.Latitude = n.mGetLat();
                    lChoosen.Longitude = n.mGetLong();
                    if(lChoosen.DistanceTo(currentLocation) < distance)
                    {
                        distance = lChoosen.DistanceTo(currentLocation);
                        Choosen = n;
                        foreach (Monument m in monumentList)
                        {
                            if (Math.Pow(currentLocation.Latitude - m.mGetLat(), 2) + Math.Pow((currentLocation.Longitude - m.mGetLong()), 2) < Math.Pow(0.062, 2))
                            {
                                lChoosen2.Latitude = m.mGetLat();
                                lChoosen2.Longitude = m.mGetLong();
                                if (lChoosen2.DistanceTo(currentLocation) > lChoosen.DistanceTo(currentLocation) && lChoosen2.DistanceTo(currentLocation) < distance2)
                                {
                                    distance2 = lChoosen2.DistanceTo(currentLocation);
                                    Choosen2 = m;
                                    foreach (Monument c in monumentList)
                                    {
                                        if (Math.Pow(currentLocation.Latitude - c.mGetLat(), 2) + Math.Pow((currentLocation.Longitude - c.mGetLong()), 2) < Math.Pow(0.062, 2))
                                        {
                                            lChoosen3.Latitude = c.mGetLat();
                                            lChoosen3.Longitude = c.mGetLong();
                                            if (lChoosen3.DistanceTo(currentLocation) > lChoosen.DistanceTo(currentLocation) && lChoosen3.DistanceTo(currentLocation) > lChoosen2.DistanceTo(currentLocation) && lChoosen3.DistanceTo(currentLocation) < distance3)
                                            {
                                                distance3 = lChoosen3.DistanceTo(currentLocation);
                                                Choosen3 = c;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            if (Choosen.mGetName() != "Няма паметници в близост.")
            {
                Location g = new Location("Range");
                g.Latitude = Choosen.mGetLat();
                g.Longitude = Choosen.mGetLong();
                txtName1.Text = $"{Choosen.mGetName()}";
                imgMonument1.SetImageBitmap(Choosen.mGetImg());
                txtRange1.Text = $"Range: {Range(g, currentLocation)}";
                btnInfo1.Enabled = true;
                btnGoTo1.Enabled = true;

                btnInfo1.Click += delegate
                {
                    ShowMonument(Choosen.mGetName(), Choosen.mGetType(), Choosen.mGetCity(), Choosen.mGetAddress(), Choosen.mGetUrl(), Choosen.mGetImg(), Choosen.mGetHistory());
                };

                btnGoTo1.Click += delegate
                {
                    var geoUri = Android.Net.Uri.Parse($"geo:{Choosen.mGetLat()},{Choosen.mGetLong()}");
                    var mapIntent = new Intent(Intent.ActionView, geoUri);
                    StartActivity(mapIntent);
                };
                if (background == true)
                {
                    //NOTIFICATION
                    Intent resultIntent = new Intent(this, typeof(MainActivity));
                    resultIntent.AddFlags(ActivityFlags.ClearTop);
                    TaskStackBuilder stackBuilder = TaskStackBuilder.Create(this);
                    stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(MainActivity)));
                    stackBuilder.AddNextIntent(resultIntent);
                    PendingIntent resultPendingIntent = stackBuilder.GetPendingIntent(0, PendingIntentFlags.UpdateCurrent);
                    string Chosen2;
                    string Chosen3;
                    if (Choosen2.mGetName() == "Няма други паметници в близост.")
                    {
                        Chosen2 = "";
                    }
                    else
                    {
                        Chosen2 = Choosen2.mGetName();
                    }
                    if (Choosen3.mGetName() == "Няма паметници в близост.")
                    {
                        Chosen3 = "";
                    }
                    else
                    {
                        Chosen3 = Choosen3.mGetName();
                    }
                    Notification.Builder builder = new Notification.Builder(this)
                    .SetContentTitle($"Паметник наблизо!")
                    .SetContentText($"Открити паметници: {Choosen.mGetName()}, {Chosen2}, {Chosen3}!")
                    .SetSmallIcon(Resource.Drawable.noimg)
                    .SetAutoCancel(true)
                    .SetContentIntent(resultPendingIntent)
                    .SetDefaults(NotificationDefaults.Sound | NotificationDefaults.Vibrate)
                    .SetWhen(Java.Lang.JavaSystem.CurrentTimeMillis());

                    Notification notification = builder.Build();

                    NotificationManager notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;
                    const int notificationId = 0;
                    notificationManager.Notify(notificationId, notification);
                }
            }
            else
            {
                txtName1.Text = $"Няма паметници в близост.";
                txtRange1.Text = $"";
                btnInfo1.Enabled = false;
                btnGoTo1.Enabled = false;
                imgMonument1.SetImageResource(Resource.Drawable.nationalmonuments);
            }
            if (Choosen2.mGetName() != "Няма други паметници в близост.")
            {
                Location g = new Location("Range");
                g.Latitude = Choosen2.mGetLat();
                g.Longitude = Choosen2.mGetLong();
                txtName2.Text = $"{Choosen2.mGetName()}";
                imgMonument2.SetImageBitmap(Choosen2.mGetImg());
                txtRange2.Text = $"Range: {Range(g, currentLocation)}";
                btnInfo2.Enabled = true;
                btnGoTo2.Enabled = true;

                btnInfo2.Click += delegate
                {
                    ShowMonument(Choosen2.mGetName(), Choosen2.mGetType(), Choosen2.mGetCity(), Choosen2.mGetAddress(), Choosen2.mGetUrl(), Choosen2.mGetImg(), Choosen2.mGetHistory());
                };

                btnGoTo2.Click += delegate
                {
                    var geoUri = Android.Net.Uri.Parse($"geo:{Choosen2.mGetLat()},{Choosen2.mGetLong()}");
                    var mapIntent = new Intent(Intent.ActionView, geoUri);
                    StartActivity(mapIntent);
                };
            }
            else
            {
                txtName2.Text = $"";
                txtRange2.Text = $"";
                btnInfo2.Enabled = false;
                btnGoTo2.Enabled = false;
                imgMonument2.SetImageResource(Resource.Drawable.noimg);
            }
            if (Choosen3.mGetName() != "Няма паметници в близост.")
            {
                Location g = new Location("Range");
                g.Latitude = Choosen3.mGetLat();
                g.Longitude = Choosen3.mGetLong();
                txtName3.Text = $"{Choosen3.mGetName()}";
                imgMonument3.SetImageBitmap(Choosen3.mGetImg());
                txtRange3.Text = $"Range: {Range(g, currentLocation)}";
                btnInfo3.Enabled = true;
                btnGoTo3.Enabled = true;

                btnInfo3.Click += delegate
                {
                    ShowMonument(Choosen3.mGetName(), Choosen3.mGetType(), Choosen3.mGetCity(), Choosen3.mGetAddress(), Choosen3.mGetUrl(), Choosen3.mGetImg(), Choosen3.mGetHistory());
                };
                btnGoTo3.Click += delegate
                {
                    var geoUri = Android.Net.Uri.Parse($"geo:{Choosen3.mGetLat()},{Choosen3.mGetLong()}?q={0}");
                    var mapIntent = new Intent(Intent.ActionView, geoUri);
                    StartActivity(mapIntent);
                };
            }
            else
            {
                txtName3.Text = $"";
                txtRange3.Text = $"";
                btnInfo3.Enabled = false;
                btnGoTo3.Enabled = false;
                imgMonument3.SetImageResource(Resource.Drawable.noimg);
            }
        }
        
        public void OnProviderDisabled(string provider)
        {

        }

        public void OnProviderEnabled(string provider)
        {

        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {

        }

        async Task<Address> ReverseGeocodeCurrentLocation()
        {
            Geocoder geocoder = new Geocoder(this);
            IList<Address> addressList =
                await geocoder.GetFromLocationAsync(currentLocation.Latitude, currentLocation.Longitude, 1);
            Address address = addressList.FirstOrDefault();
            return address;
        }

        void DisplayAddress(Address address)
        {
            if (address != null)
            {
                StringBuilder deviceAddress = new StringBuilder();
                for (int i = 0; i < address.MaxAddressLineIndex; i++)
                {
                    deviceAddress.AppendLine(address.GetAddressLine(i));
                }
                txtAddress.Text = deviceAddress.ToString();
            }
            else
            {
                txtAddress.Text = "ул. Хаджи Димитър 4";
            }
        }

        void MySqlConnect()
        {
            try
            {
                //con = new MySqlConnection("Server=192.168.1.104;Port=3306;database=nationalmonument;User Id=nationalmonument;Password=123456;charset=utf8");
                con = new MySqlConnection("Server=169.254.80.80;Port=3306;database=nationalmonument;User Id=nationalmonument;Password=123456;charset=utf8");
                //con = new MySqlConnection("Server=212.233.232.235;Port=3306;database=nationalmonument;User Id=nationalmonument;Password=123456;charset=utf8");
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                    QueryMonument(con);
                }
            }
            catch(MySqlException ex)
            {
                txtCity.Text = ex.ToString();
            }
            finally
            {
                con.Close();
                con.Dispose();
            }
            
        }

        public void QueryMonument(MySqlConnection conn)
        {
            string sql = "SELECT id, name, type, city, address, history, url, pointLong, pointLat, img FROM nationalmonument";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader dataReader = cmd.ExecuteReader();
            while (dataReader.Read())
            {
                int ID = int.Parse(dataReader["id"] + "");
                string name = dataReader["name"] + "";
                string type = dataReader["type"] + "";
                string city = dataReader["city"] + "";
                string address = dataReader["address"] + "";
                string history = dataReader["history"] + "";
                string url = dataReader["url"] + "";
                double longitude = Double.Parse(dataReader["pointLong"] + "");
                double latitude = Double.Parse(dataReader["pointLat"] + "");
                Bitmap img = GetImageBitmapFromUrl(dataReader["img"] + "");

                monumentList.Add(new Monument(ID, name, type, city, address, history, url, img, latitude, longitude));
            }
            dataReader.Close();
            dataReader.Dispose();
        }
        public string Range(Location a, Location b)
        {
            double meters = a.DistanceTo(b);
            double kilometeres = 0;
            if (meters > 1000)
            {
                kilometeres = meters / 1000;
                meters %= 1000;
                return $"{kilometeres:F0} км {meters:F0} м";
            }
            else
            {
                return $"{meters:F0} м";
            }

        }
        public void Declaration()
        {
            //Declaration
            lblConnection = FindViewById<TextView>(Resource.Id.lblConnection);
            txtConnection = FindViewById<TextView>(Resource.Id.txtConnection);
            lblLocation = FindViewById<TextView>(Resource.Id.lblLocation);
            txtLocation = FindViewById<TextView>(Resource.Id.txtLocation);
            lblAddress = FindViewById<TextView>(Resource.Id.lblAddress);
            txtAddress = FindViewById<TextView>(Resource.Id.txtAddress);
            lblCity = FindViewById<TextView>(Resource.Id.lblCity);
            txtCity = FindViewById<TextView>(Resource.Id.txtCity);

            txtName1 = FindViewById<TextView>(Resource.Id.txtName);
            txtRange1 = FindViewById<TextView>(Resource.Id.txtRange);
            imgMonument1 = FindViewById<ImageView>(Resource.Id.imgMonument);
            btnInfo1 = FindViewById<Button>(Resource.Id.btnInfo);
            btnGoTo1 = FindViewById<Button>(Resource.Id.btnGoTo);
            btnInfo1.Enabled = false;
            btnGoTo1.Enabled = false;

            txtName2 = FindViewById<TextView>(Resource.Id.txtName2);
            txtRange2 = FindViewById<TextView>(Resource.Id.txtRange2);
            imgMonument2 = FindViewById<ImageView>(Resource.Id.imgMonument2);
            btnInfo2 = FindViewById<Button>(Resource.Id.btnInfo2);
            btnGoTo2 = FindViewById<Button>(Resource.Id.btnGoTo2);
            btnInfo2.Enabled = false;
            btnGoTo2.Enabled = false;

            txtName3 = FindViewById<TextView>(Resource.Id.txtName3);
            txtRange3 = FindViewById<TextView>(Resource.Id.txtRange3);
            imgMonument3 = FindViewById<ImageView>(Resource.Id.imgMonument3);
            btnInfo3 = FindViewById<Button>(Resource.Id.btnInfo3);
            btnGoTo3 = FindViewById<Button>(Resource.Id.btnGoTo3);
            btnInfo3.Enabled = false;
            btnGoTo3.Enabled = false;
        }
        void ShowMonument(string Name, string Type, string City, string Address, string Url, Bitmap Img, string Description)
        {
            SetContentView(Resource.Layout.Monument);

            imgmPic = FindViewById<ImageView>(Resource.Id.imgPicture);
            imgmPic.SetImageBitmap(Img);

            txtmName = FindViewById<TextView>(Resource.Id.txtmName);
            txtmName.Text = $"Име: {Name}";
            txtmType = FindViewById<TextView>(Resource.Id.txtmType);
            txtmType.Text = $"Вид: {Type}";
            txtmCity = FindViewById<TextView>(Resource.Id.txtmCity);
            txtmCity.Text = $"Град: {City}";
            txtmAddress = FindViewById<TextView>(Resource.Id.txtmAddress);
            txtmAddress.Text = $"Адрес: {Address}";
            txtmDescription = FindViewById<TextView>(Resource.Id.txtmDescription);
            txtmDescription.Text = $"Описание: {Description}";

            btnBack = FindViewById<Button>(Resource.Id.btnBack);
            locMgr.RemoveUpdates(this);

            btnBack.Click += delegate
            {
                SetContentView(Resource.Layout.Main);
                Declaration();
                LocationHelper();
            };
            btnMoreInfo = FindViewById<Button>(Resource.Id.btnMoreInfo);
            btnMoreInfo.Click += delegate
            {
                var uri = Android.Net.Uri.Parse(Url);
                var intent = new Intent(Intent.ActionView, uri);
                StartActivity(intent);
            };
        }
        private Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (WebClient webClient = new WebClient())
            {
                try
                {
                    var imageBytes = webClient.DownloadData(url);
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    }
                }
                catch(ArgumentException ex)
                {
                    Toast.MakeText(ApplicationContext, $"National Monument was not able to load an picture: {ex}", ToastLength.Long).Show();
                }
                
            }

            return imageBitmap;
        }
    }
    

}

