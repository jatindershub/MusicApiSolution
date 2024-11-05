namespace MusicApi.Contracts.Artist
{
    public record ArtistResponse(
        string Mbid,
        string Description,
        List<Album> Albums );

    public record Album(
        string Title, 
        string Id, 
        string Image);

}
