using System.ComponentModel.DataAnnotations;

namespace I_HAVE_GAME.ViewModels
{
    /// <summary>
    /// Search wizard state and results
    /// NOTE: PlayMode and Budget are stored in the application state but may not be directly filterable by RAWG.
    /// RAWG provides genre, platform, and release date filtering. Other criteria are informational.
    /// </summary>
    public class GameSearchViewModel
    {
        // Current step (1-5)
        public int CurrentStep { get; set; } = 1;

        // Step 1: Genre - Supported by RAWG
        [Display(Name = "Genre")]
        public string? SelectedGenre { get; set; }

        // Step 2: Device/Platform - Supported by RAWG
        [Display(Name = "Device")]
        public string? SelectedDevice { get; set; }

        // Step 3: Play Mode - Not directly supported by RAWG API
        // Stored for user selection and future enhancement
        [Display(Name = "Play Mode")]
        public string? SelectedPlayMode { get; set; }

        // Step 4: Budget - Not directly supported by RAWG API
        // Stored for user selection and future enhancement
        [Display(Name = "Budget")]
        public string? SelectedBudget { get; set; }

        // Step 5: Era/Release Date - Supported by RAWG via dates parameter
        [Display(Name = "Era")]
        public string? SelectedEra { get; set; }

        // Pagination (RAWG uses page-based pagination, 1-indexed)
        public int CurrentPage { get; set; } = 0;
        public int PageSize { get; set; } = 20;

        // Results
        public List<GameResultViewModel> Results { get; set; } = new List<GameResultViewModel>();
        public int TotalResults { get; set; } = 0;

        // Messages
        public string? ErrorMessage { get; set; }
        public string? WarningMessage { get; set; }
        public bool IsLoading { get; set; }

        // Genre options (mapped to RAWG genre slugs)
        public static List<DropdownOption> GenreOptions => new List<DropdownOption>
        {
            new DropdownOption { Value = "Action", Label = "Action" }, new DropdownOption { Value = "Adventure", Label = "Adventure" }, new DropdownOption { Value = "RPG", Label = "RPG" }, new DropdownOption { Value = "Strategy", Label = "Strategy" }, new DropdownOption { Value = "Simulation", Label = "Simulation" }, new DropdownOption { Value = "Sports", Label = "Sports" }, new DropdownOption { Value = "Racing", Label = "Racing" }, new DropdownOption { Value = "Shooter", Label = "Shooter" }, new DropdownOption { Value = "Puzzle", Label = "Puzzle" }, new DropdownOption { Value = "Platformer", Label = "Platformer" }, new DropdownOption { Value = "Horror", Label = "Horror" }, new DropdownOption { Value = "Indie", Label = "Indie" }, new DropdownOption { Value = "any", Label = "เลือกได้ทุกแนว" }
        };

        // Device/Platform options (mapped to RAWG platform IDs)
        // PC: 4 (Windows), 187 (Web), 18 (macOS) | PlayStation: 18 | Xbox: 1 | Nintendo: 7 | Mobile: 8 (iOS), 14 (Android)
        public static List<DropdownOption> DeviceOptions => new List<DropdownOption>
        {
            new DropdownOption { Value = "PC", Label = "PC" }, new DropdownOption { Value = "PlayStation", Label = "PlayStation" }, new DropdownOption { Value = "Xbox", Label = "Xbox" }, new DropdownOption { Value = "Nintendo", Label = "Nintendo Switch" }, new DropdownOption { Value = "Mobile", Label = "Mobile" }, new DropdownOption { Value = "any", Label = "เลือกได้ทุกเครื่อง" }
        };

        // Play Mode options - INFORMATIONAL ONLY
        // RAWG API does not provide direct single-player vs multiplayer filtering
        // Development note: These selections are stored but not used for filtering
        public static List<DropdownOption> PlayModeOptions => new List<DropdownOption>
        {
            new DropdownOption { Value = "solo", Label = "เล่นคนเดียว" }, new DropdownOption { Value = "multiplayer", Label = "เล่นกับเพื่อน" }, new DropdownOption { Value = "any", Label = "ได้ทั้งสองแบบ" }
        };

        // Budget options - INFORMATIONAL ONLY
        // RAWG API does not provide direct free vs paid filtering
        // Development note: These selections are stored but not used for filtering
        public static List<DropdownOption> BudgetOptions => new List<DropdownOption>
        {
            new DropdownOption { Value = "free", Label = "ฟรี" }, new DropdownOption { Value = "under-20", Label = "ประหยัด (ไม่เกิน $20)" }, new DropdownOption { Value = "premium", Label = "ราคาไม่จำกัด" }, new DropdownOption { Value = "any", Label = "ยังไม่กำหนดงบ" }
        };

        // Era/Release Date options (formatted for RAWG dates parameter: YYYY-MM-DD,YYYY-MM-DD)
        public static List<DropdownOption> EraOptions => new List<DropdownOption>
        {
            new DropdownOption { Value = "2020+", Label = "เกมใหม่ (2020 เป็นต้นไป)" }, new DropdownOption { Value = "before-2020", Label = "เกมคลาสสิก (ก่อน 2020)" }, new DropdownOption { Value = "any", Label = "ได้ทุกช่วงเวลา" }
        };
    }

    /// <summary>
    /// Dropdown option for UI
    /// </summary>
    public class DropdownOption
    {
        public string Value { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
    }

    /// <summary>
    /// Game result to display in search results
    /// </summary>
    public class GameResultViewModel
    {
        public int Id { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? BackgroundImage { get; set; }
        public string? ReleasedDate { get; set; }
        public decimal Rating { get; set; }
        public int RatingsCount { get; set; }
        public List<string> Genres { get; set; } = new List<string>();
        public List<string> Platforms { get; set; } = new List<string>();
        public List<string> Stores { get; set; } = new List<string>();
        public string? Description { get; set; }
    }

    /// <summary>
    /// Request model for search wizard POST
    /// </summary>
    public class GameSearchRequest
    {
        public string? Action { get; set; }
        public int CurrentStep { get; set; } = 1;
        public string? SelectedGenre { get; set; }
        public string? SelectedDevice { get; set; }
        public string? SelectedPlayMode { get; set; }
        public string? SelectedBudget { get; set; }
        public string? SelectedEra { get; set; }
        public int CurrentPage { get; set; } = 0;
        public int PageSize { get; set; } = 20;
    }
}
