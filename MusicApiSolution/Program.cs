using ArtistInfo.Api.Services.CoverArtArchive;
using ArtistInfo.Api.Services.MusicBrainz;
using ArtistInfo.Api.Services.Wikidata;
using ArtistInfo.Api.Services.Wikipedia;
using MusicApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Register the controllers
builder.Services.AddControllers();

// Register IArtistService and ArtistService
builder.Services.AddScoped<IArtistService, ArtistService>();

// Register the other services used by ArtistService
builder.Services.AddScoped<IMusicBrainzService, MusicBrainzService>();
builder.Services.AddScoped<IWikidataService, WikidataService>();
builder.Services.AddScoped<IWikipediaService, WikipediaService>();
builder.Services.AddScoped<ICoverArtArchiveService, CoverArtArchiveService>();

// Register HTTP Clients for each service that interacts with an external API
builder.Services.AddHttpClient<IMusicBrainzService, MusicBrainzService>();
builder.Services.AddHttpClient<IWikidataService, WikidataService>();
builder.Services.AddHttpClient<IWikipediaService, WikipediaService>();
builder.Services.AddHttpClient<ICoverArtArchiveService, CoverArtArchiveService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseExceptionHandler("/error");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();