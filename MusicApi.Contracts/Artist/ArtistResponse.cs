namespace MusicApi.Contracts.Artist
{
    public record ArtistResponse(
        string Mbid,
        string Description,
        List<AlbumDto> Albums );

    public record AlbumDto(
        string Title, 
        string Id, 
        string Image);

}
