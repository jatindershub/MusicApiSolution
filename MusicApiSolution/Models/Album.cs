namespace MusicApi.Models
{
    public class Album
    {
        public required string Title { get; set; }
        public required string Id { get; set; }
        public required string Image { get; set; }

        // todo: er der behov for en ctor her?  
        // todo: find ud af hvorfor den skal være required
    }


}
