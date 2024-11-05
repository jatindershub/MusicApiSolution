using Newtonsoft.Json.Linq;

namespace MusicApi.Models
{
    public class Album
    {
        public string Title { get; set; }
        public string Id { get; set; }
        public string Image { get; set; }

        // todo: er der behov for en ctor her?  
        // todo: find ud af hvorfor den skal være required

        // Constructor that takes three arguments
        public Album(string title, string id, string image)
        {
            Title = title;
            Id = id;
            Image = image;
        }
    }


}
