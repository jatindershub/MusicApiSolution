namespace ArtistInfo.Api.Services.CoverArtArchive
{
    public interface ICoverArtArchiveService
    {
        Task<IEnumerable<string>> FetchCoverArtsAsync(List<string> releaseGroupIds);
        Task<string> FetchCoverArtAsync(string releaseGroupId);
    }
}
