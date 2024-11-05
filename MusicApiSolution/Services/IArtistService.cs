using MusicApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicApi.Contracts.Artist;

namespace MusicApi.Services
{
    public interface IArtistService
    {
        Artist GetArtist(string mbid);
    }
}
