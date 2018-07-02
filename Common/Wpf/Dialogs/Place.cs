using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Wpf.Dialogs
{
    internal class Place
    {
        public long AvailableFreeSpace { get; private set; }
        public PlaceType PlaceType { get; private set; }
        public string VolumeLabel { get; private set; }
        public string Path { get; }
        public int PercentFilled { get; private set; }


        public BitmapImage Icon
        {
            get
            {
                string uri = string.Empty;

                switch (this.PlaceType)
                {
                    case PlaceType.Fixed:
                        uri = "pack://siteoforigin:,,,/Resources/FixedDrive.png";
                        break;
                    case PlaceType.Removable:
                        uri = "pack://siteoforigin:,,,/Resources/RemovableDrive.png";
                        break;
                    default:
                        return null;
                }

                BitmapImage src = new BitmapImage();
                src.BeginInit();
                src.UriSource = new Uri(uri, UriKind.RelativeOrAbsolute);
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();

                return src;
            }
        }

        public Place(DirectoryInfo rootDirectory, long availableFreeSpace, PlaceType placeType, string volumeLabel, int percentFilled)
        {
            this.AvailableFreeSpace = availableFreeSpace;
            this.PlaceType = placeType;
            this.VolumeLabel = volumeLabel;
            this.Path = rootDirectory.FullName;
            this.PercentFilled = percentFilled;
        }
    }
}