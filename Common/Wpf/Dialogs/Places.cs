using System;
using System.Collections.ObjectModel;
using System.IO;

namespace Wpf.Dialogs
{
    internal class Places : ObservableCollection<Place>
    {
        public Places()
        {
            this.AddDrives();
        }

        private void AddDrives()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            foreach (DriveInfo driveInfo in drives)
            {
                if (driveInfo.IsReady)
                {
                    this.Add(new Place(driveInfo.RootDirectory, driveInfo.AvailableFreeSpace, this.ToPlaceType(driveInfo.DriveType), driveInfo.VolumeLabel, (int)((driveInfo.TotalSize - driveInfo.AvailableFreeSpace) * 100 / driveInfo.TotalSize)));
                }
            }

            this.OnCollectionChanged(new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
        }

        private PlaceType ToPlaceType(DriveType driveType)
        {
            switch (driveType)
            {
                case DriveType.Unknown:
                    return PlaceType.Unknown;
                case DriveType.NoRootDirectory:
                    return PlaceType.NoRootDirectory;
                case DriveType.Removable:
                    return PlaceType.Removable;
                case DriveType.Fixed:
                    return PlaceType.Fixed;
                case DriveType.Network:
                    return PlaceType.Network;
                case DriveType.CDRom:
                    return PlaceType.CDRom;
                case DriveType.Ram:
                    return PlaceType.Ram;
                default:
                    throw new ArgumentOutOfRangeException("driveType");
            }
        }
    }
}