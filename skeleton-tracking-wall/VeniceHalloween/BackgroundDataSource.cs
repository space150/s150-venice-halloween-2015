using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeniceHalloween
{
    class BackgroundDataSource
    {
        private List<string> videoPaths;
        private int currentPathIndex;

        public string GetNextVideoPath()
        {
            this.currentPathIndex += 1;
            if (this.currentPathIndex > this.videoPaths.Count - 1)
            {
                Shuffle(this.videoPaths);
                this.currentPathIndex = 0;
            }

            return this.videoPaths[this.currentPathIndex];
        }

        public BackgroundDataSource()
        {
            this.currentPathIndex = 0;

            this.videoPaths = new List<string>();
            this.videoPaths.Add("Videos/backgrounds/09w-0hioqwdl.mov");
            this.videoPaths.Add("Videos/backgrounds/137219080.mp4");
            this.videoPaths.Add("Videos/backgrounds/196029237.mp4");
            this.videoPaths.Add("Videos/backgrounds/268089752.mp4");
            this.videoPaths.Add("Videos/backgrounds/279643250.mp4");
            this.videoPaths.Add("Videos/backgrounds/421633579.mp4");
            this.videoPaths.Add("Videos/backgrounds/517075039.mp4");
            this.videoPaths.Add("Videos/backgrounds/543064114.mp4");
            this.videoPaths.Add("Videos/backgrounds/ewf-w-efwoef-we.mov");
            this.videoPaths.Add("Videos/backgrounds/ewfwef9fjif.mp4");
            this.videoPaths.Add("Videos/backgrounds/jwe9-iwefjwpeor.mp4");
            this.videoPaths.Add("Videos/backgrounds/jwpwepfojwfe.mov");
            this.videoPaths.Add("Videos/backgrounds/lcodiji0enken.mp4");
            this.videoPaths.Add("Videos/backgrounds/nnsksododop.mp4");
            this.videoPaths.Add("Videos/backgrounds/nwpo-qj03dd.mov");
            this.videoPaths.Add("Videos/backgrounds/odwpojejpofwjoef.mov");
            this.videoPaths.Add("Videos/backgrounds/oijw0990fijfp.mov");
            this.videoPaths.Add("Videos/backgrounds/ouwij0wfhioo-je.mov");
            this.videoPaths.Add("Videos/backgrounds/uhdiwihoew0diwhoef.mp4");
            this.videoPaths.Add("Videos/backgrounds/uhfu8fu9egueijo.mov");
            this.videoPaths.Add("Videos/backgrounds/weopjweoj.mov");
            this.videoPaths.Add("Videos/backgrounds/wjonepfiwejpiofowef.mp4");
            this.videoPaths.Add("Videos/backgrounds/wowef-0we-ojwe-ojf.mp4");

            Shuffle(this.videoPaths);
        }

        private Random rng = new Random();
        private void Shuffle(IList<string> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                string value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
