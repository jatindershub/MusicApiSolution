namespace ArtistInfo.Api.Services.CoverArtArchive
{
    public interface ICoverArtArchiveService
    {
        Task<List<string>> GetCoverArtsAsync(List<string> releaseGroupIds);
        Task<string> FetchCoverArtAsync(string releaseGroupId);
    }
}
