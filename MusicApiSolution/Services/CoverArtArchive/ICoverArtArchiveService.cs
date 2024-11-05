namespace ArtistInfo.Api.Services.CoverArtArchive
{
    public interface ICoverArtArchiveService
    {
        Task<string> GetCoverArtAsync(string releaseGroupId);
    }
}
