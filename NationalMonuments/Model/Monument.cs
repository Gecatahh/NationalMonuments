using Android.Graphics;

namespace NationalMonuments.Model
{
    class Monument
    {
        private int ID { get; set; }
        private string Name { get; set; }
        private string Type { get; set; }
        private string City { get; set; }
        private string Address { get; set; }
        private string History { get; set; }
        private string Url { get; set;  }
        private double Latitude { get; set; }
        private double Longitude { get; set; }
        private Bitmap Img { get; set; }


        public Monument(string Name)
        {
            this.Name = Name;
        }
        public Monument(int ID, string Name, string Type, string City, string Address, string History, string Url, Bitmap Img, double Latitude, double Longitude)
        {
            this.ID = ID;
            this.Name = Name;
            this.Type = Type;
            this.City = City;
            this.Address = Address;
            this.History = History;
            this.Url = Url;
            this.Latitude = Latitude;
            this.Longitude = Longitude;
            this.Img = Img;
        }
        public double mGetLat()
        {
            return this.Latitude;
        }
        public double mGetLong()
        {
            return this.Longitude;
        }
        public string mGetName()
        {
            return this.Name;
        }
        public string mGetAddress()
        {
            return this.Address;
        }
        public string mGetType()
        {
            return this.Type;
        }
        public string mGetCity()
        {
            return this.City;
        }
        public string mGetHistory()
        {
            return this.History;
        }
        public string mGetUrl()
        {
            return this.Url;
        }
        public Bitmap mGetImg()
        {
            return this.Img;
        }
    }
}