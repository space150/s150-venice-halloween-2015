using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace VeniceHalloween
{
    class Costume
    {
        public string Type;
        public ImageSource Head;
        public ImageSource BicepRight;
        public ImageSource BicepLeft;
        public ImageSource ForearmRight;
        public ImageSource ForearmLeft;
        public ImageSource HandRight;
        public ImageSource HandLeft;
        public ImageSource FemurRight;
        public ImageSource FemurLeft;
        public ImageSource ShinRight;
        public ImageSource ShinLeft;
        public ImageSource Neck;
        public ImageSource Ribcage;
        public ImageSource Spine;
        public ImageSource Pelvis;
 
        public Costume(string type)
        {
            this.Type = type;
            this.Head = this.getImageSource(type, "head.png");
            this.BicepRight = this.getImageSource(type, "right-bicep.png");
            this.BicepLeft = this.getImageSource(type, "left-bicep.png");
            this.ForearmRight = this.getImageSource(type, "right-forearm.png");
            this.ForearmLeft = this.getImageSource(type, "left-forearm.png");
            this.HandRight = this.getImageSource(type, "right-hand.png");
            this.HandLeft = this.getImageSource(type, "left-hand.png");
            this.FemurRight = this.getImageSource(type, "right-femur.png");
            this.FemurLeft = this.getImageSource(type, "left-femur.png");
            this.ShinRight = this.getImageSource(type, "right-shin.png");
            this.ShinLeft = this.getImageSource(type, "left-shin.png");
            this.Neck = this.getImageSource(type, "neck.png");
            this.Ribcage = this.getImageSource(type, "ribcage.png");
            this.Spine = this.getImageSource(type, "spine.png");
            this.Pelvis = this.getImageSource(type, "pelvis.png");
        }

        private ImageSource getImageSource(string type, string entry)
        {
            string packUri = string.Format("Images/costumes/{0}/{1}", type, entry);
            return new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
        }

    }
}
