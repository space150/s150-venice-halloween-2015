using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace VeniceHalloween
{
    class Costume
    {
        public string Type;
        public ImageSource Head;
        public double HeadScale;
        public Point HeadOffset;

        public ImageSource BicepRight;
        public ImageSource BicepLeft;
        public double BicepScale;
        public Point BicepOffset;

        public ImageSource ForearmRight;
        public ImageSource ForearmLeft;
        public double ForearmScale;
        public Point ForearmOffset;

        public ImageSource HandRight;
        public ImageSource HandLeft;
        public double HandScale;
        public Point HandOffset;

        public ImageSource FemurRight;
        public ImageSource FemurLeft;
        public double FemurScale;
        public Point FemurOffset;

        public ImageSource ShinRight;
        public ImageSource ShinLeft;
        public double ShinScale;
        public Point ShinOffset;

        public ImageSource FootRight;
        public ImageSource FootLeft;
        public double FootScale;
        public Point FootOffset;

        public ImageSource Neck;
        public double NeckScale;
        public Point NeckOffset;

        public ImageSource Ribcage;
        public double RibcageScale;
        public Point RibcageOffset;

        public ImageSource Spine;
        public double SpineScale;
        public Point SpineOffset;

        public ImageSource Pelvis;
        public double PelvisScale;
        public Point PelvisOffset;
 
        public Costume(string type)
        {
            this.Type = type;

            this.Head = this.getImageSource(type, "head.png");
            this.HeadScale = 1.1;
            this.HeadOffset.Y = -6.0;

            this.BicepRight = this.getImageSource(type, "right-bicep.png");
            this.BicepLeft = this.getImageSource(type, "left-bicep.png");
            this.BicepScale = 1.0;
            this.BicepOffset.X = 0;

            this.ForearmRight = this.getImageSource(type, "right-forearm.png");
            this.ForearmLeft = this.getImageSource(type, "left-forearm.png");
            this.ForearmScale = 1.0;
            this.ForearmOffset.X = 0;

            this.HandRight = this.getImageSource(type, "right-hand.png");
            this.HandLeft = this.getImageSource(type, "left-hand.png");
            this.HandScale = 1.2;
            this.HandOffset.X = 0;

            this.FemurRight = this.getImageSource(type, "right-femur.png");
            this.FemurLeft = this.getImageSource(type, "left-femur.png");
            this.FemurScale = 1.0;
            this.FemurOffset.X = 0;

            this.ShinRight = this.getImageSource(type, "right-shin.png");
            this.ShinLeft = this.getImageSource(type, "left-shin.png");
            this.ShinScale = 1.0;
            this.ShinOffset.X = 0;

            this.FootRight = this.getImageSource(type, "right-foot.png");
            this.FootLeft = this.getImageSource(type, "left-foot.png");
            this.FootScale = 1.4;
            this.FootOffset.X = 0;

            this.Neck = this.getImageSource(type, "neck.png");
            this.NeckScale = 1.0;
            this.NeckOffset.Y = 5.0;

            this.Ribcage = this.getImageSource(type, "ribcage.png");
            this.RibcageScale = 0.8;
            this.RibcageOffset.Y = -10.0;

            this.Spine = this.getImageSource(type, "spine.png");
            this.SpineScale = 0.8;
            this.SpineOffset.X = 0;

            this.Pelvis = this.getImageSource(type, "pelvis.png");
            this.PelvisScale = 1.0;
            this.PelvisOffset.Y = 8.0;
        }

        private ImageSource getImageSource(string type, string entry)
        {
            string packUri = string.Format("Images/costumes/{0}/{1}", type, entry);
            return new ImageSourceConverter().ConvertFromString(packUri) as ImageSource;
        }

    }
}
