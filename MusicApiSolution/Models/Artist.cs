namespace MusicApi.Models
{
    public class Artist
    {
        // todo fjern set - spørg hvorfor
        public string Mbid { get; set; }
        public string Description { get; set; }
        public List<Album> Albums { get; set; }

        public Artist(string mbid, string description, List<Album> albums)
        {
            // enforce invariants
            Mbid = mbid;
            Description = description;
            Albums = albums;
        }

    }
}
