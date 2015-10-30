using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VeniceHalloween
{
    class GiphyDataSource
    {
        private List<string> gifs;
        private int currentGifIndex;
        private bool requesting;
        private string searchTerm;
        private int offset;

        public string GetNextGif()
        {
            this.currentGifIndex += 1;
            if ( this.currentGifIndex > this.gifs.Count-1 )
            {
                this.requestGifs(this.searchTerm);
                this.currentGifIndex = 0;
            }

            return this.gifs[this.currentGifIndex];
        }
        
        public GiphyDataSource(string term)
        {
            this.searchTerm = term;
            this.requesting = false;
            this.currentGifIndex = 0;

            this.gifs = new List<string>();
            this.gifs.Add("http://i.giphy.com/edl5t7nDVPtC.gif"); // startup :)
        }

        private void requestGifs(string term)
        {
            if (this.requesting) return;
            requesting = true;

            try
            {
                string url = string.Format("http://api.giphy.com/v1/gifs/search?q={0}&offset={1}&api_key=dc6zaTOxFJmzC", term, offset);
                HttpWebRequest request = HttpWebRequest.CreateHttp(url);
                doWithResponse(request, (response) => {
                    var body = new StreamReader(response.GetResponseStream()).ReadToEnd();
                    JObject content = JObject.Parse(body);
                    System.Diagnostics.Debug.WriteLine(content);

                    try
                    {
                        List<string> newGifs = new List<string>();
                        JArray entries = (JArray)content["data"];
                        for (int i = 0; i < entries.Count; i++)
                            newGifs.Add((string)entries[i]["images"]["original"]["url"]);

                        this.currentGifIndex = 0;
                        this.gifs = newGifs;
                        this.requesting = false;
                        this.offset += this.gifs.Count;
                        if (this.offset >= 300) // put this in here to keep things fresh!
                            this.offset = 0;
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e.Message);
                    }

                });
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
        }

        private void doWithResponse(HttpWebRequest request, Action<HttpWebResponse> responseAction)
        {
            Action wrapperAction = () =>
            {
                request.BeginGetResponse(new AsyncCallback((iar) =>
                {
                    var response = (HttpWebResponse)((HttpWebRequest)iar.AsyncState).EndGetResponse(iar);
                    responseAction(response);
                }), request);
            };
            wrapperAction.BeginInvoke(new AsyncCallback((iar) =>
            {
                var action = (Action)iar.AsyncState;
                action.EndInvoke(iar);
            }), wrapperAction);
        }

    }
}
