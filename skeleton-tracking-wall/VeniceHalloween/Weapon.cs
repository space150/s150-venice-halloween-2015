using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace VeniceHalloween
{
    class Weapon
    {
        public string Type;
        public ImageSource ImageLeft;
        public ImageSource ImageRight;
        public Point OffsetLeft;
        public Point OffsetRight;
        public double ScaleLeft;
        public double ScaleRight;

        public Weapon(string type)
        {
            this.Type = type;
            this.ImageLeft = this.getImageSource(type, "left");
            this.ImageRight = this.getImageSource(type, "right");
            this.OffsetLeft.Y = 0.0;
            this.OffsetRight.Y = 0.0;
            this.ScaleLeft = 1.5;
            this.ScaleRight = 1.5;
        }
  
        private ImageSource getImageSource(string type, string entry)
        {
            string packUri = string.Format("Images/weapons/{0}-{1}.png", entry, type);
            return new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
        }
    }
}
