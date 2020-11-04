using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media.Imaging;
namespace PuzzleDev.Models
{
    public class ImageData
    {
        private BitmapImage _Images;
        public BitmapImage Images
        {
            get { return this._Images; }
            set { this._Images = value; }
        }

    }
}
