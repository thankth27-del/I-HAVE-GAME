using System.Text.Json.Serialization;

namespace I_HAVE_GAME.Models.Rawg
{
    /// <summary>
    /// Options for RAWG API configuration
    /// </summary>
    public class RawgOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://api.rawg.io/api/";
    }

    /// <summary>
    /// RAWG API Game Response
    /// </summary>
    public class RawgGame
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("background_image")]
        public string? BackgroundImage { get; set; }

        [JsonPropertyName("released")]
        public string? Released { get; set; }

        [JsonPropertyName("rating")]
        public decimal Rating { get; set; }

        [JsonPropertyName("ratings_count")]
        public int RatingsCount { get; set; }

        [JsonPropertyName("genres")]
        public List<RawgGenre>? Genres { get; set; }

        [JsonPropertyName("platforms")]
        public List<RawgPlatformWrapper>? Platforms { get; set; }

        [JsonPropertyName("stores")]
        public List<RawgStoreWrapper>? Stores { get; set; }

        [JsonPropertyName("description_raw")]
        public string? DescriptionRaw { get; set; }

        [JsonPropertyName("short_screenshots")]
        public List<RawgScreenshot>? ShortScreenshots { get; set; }
    }

    /// <summary>
    /// RAWG Genre
    /// </summary>
    public class RawgGenre
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("slug")]
        public string Slug { get; set; } = string.Empty;
    }

    /// <summary>
    /// RAWG Platform wrapper (contains platform details)
    /// </summary>
    public class RawgPlatformWrapper
    {
        [JsonPropertyName("platform")]
        public RawgPlatform? Platform { get; set; }
    }

    /// <summary>
    /// RAWG Platform
    /// </summary>
    public class RawgPlatform
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("slug")]
        public string Slug { get; set; } = string.Empty;
    }

    /// <summary>
    /// RAWG Store wrapper (contains store details)
    /// </summary>
    public class RawgStoreWrapper
    {
        [JsonPropertyName("store")]
        public RawgStore? Store { get; set; }
    }

    /// <summary>
    /// RAWG Store
    /// </summary>
    public class RawgStore
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("slug")]
        public string Slug { get; set; } = string.Empty;
    }

    /// <summary>
    /// RAWG Screenshot
    /// </summary>
    public class RawgScreenshot
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("image")]
        public string? Image { get; set; }
    }

    /// <summary>
    /// RAWG API Paginated Response
    /// </summary>
    public class RawgGameListResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("next")]
        public string? Next { get; set; }

        [JsonPropertyName("previous")]
        public string? Previous { get; set; }

        [JsonPropertyName("results")]
        public List<RawgGame> Results { get; set; } = new List<RawgGame>();
    }

    /// <summary>
    /// RAWG Genres List Response
    /// </summary>
    public class RawgGenreListResponse
    {
        [JsonPropertyName("results")]
        public List<RawgGenre> Results { get; set; } = new List<RawgGenre>();
    }

    /// <summary>
    /// RAWG Platforms List Response
    /// </summary>
    public class RawgPlatformListResponse
    {
        [JsonPropertyName("results")]
        public List<RawgPlatform> Results { get; set; } = new List<RawgPlatform>();
    }
}
