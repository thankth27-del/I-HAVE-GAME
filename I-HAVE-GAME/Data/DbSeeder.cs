using I_HAVE_GAME.Models;
using Microsoft.AspNetCore.Identity;

namespace I_HAVE_GAME.Data
{
    public static class DbSeeder
    {
        /// <summary>
        /// Creates the administrator account for a fresh database. The password is saved as
        /// an ASP.NET Identity hash, never as plain text in SQLite.
        /// </summary>
        public static void SeedDefaultAdmin(AppDbContext context)
        {
            const string username = "Thank";
            const string email = "thank.admin@i-have-game.local";
            const string password = "123456";

            var admin = context.Users.SingleOrDefault(user => user.Username == username);
            if (admin is null)
            {
                admin = new User
                {
                    Username = username,
                    Email = email,
                    PasswordHash = string.Empty,
                    Nickname = username,
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
                };

                admin.PasswordHash = new PasswordHasher<User>().HashPassword(admin, password);
                context.Users.Add(admin);
                context.SaveChanges();
                return;
            }

            // An existing default account is always kept as an administrator, while its
            // password and profile data remain untouched.
            if (admin.Role != "Admin")
            {
                admin.Role = "Admin";
                context.SaveChanges();
            }
        }

        public static void SeedQuizQuestions(AppDbContext context)
        {
            if (context.QuizQuestions.Any())
            {
                return; // Database already seeded
            }

            var quizQuestions = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    QuestionText = "Which game is known for the phrase 'It's dangerous to go alone'?",
                    CorrectAnswer = "The Legend of Zelda",
                    Choice1 = "The Legend of Zelda",
                    Choice2 = "Super Mario Bros",
                    Choice3 = "Metroid",
                    Choice4 = "Castlevania"
                },
                new QuizQuestion
                {
                    QuestionText = "What is the name of the plumber protagonist in the most famous platform game series?",
                    CorrectAnswer = "Mario",
                    Choice1 = "Mario",
                    Choice2 = "Luigi",
                    Choice3 = "Toad",
                    Choice4 = "Yoshi"
                },
                new QuizQuestion
                {
                    QuestionText = "Which game features the character Link?",
                    CorrectAnswer = "The Legend of Zelda",
                    Choice1 = "The Legend of Zelda",
                    Choice2 = "Final Fantasy",
                    Choice3 = "Dragon Quest",
                    Choice4 = "Fire Emblem"
                },
                new QuizQuestion
                {
                    QuestionText = "What is the main objective in Pac-Man?",
                    CorrectAnswer = "Eat all the dots while avoiding ghosts",
                    Choice1 = "Eat all the dots while avoiding ghosts",
                    Choice2 = "Reach the top of the screen",
                    Choice3 = "Defeat the final boss",
                    Choice4 = "Collect all treasures"
                },
                new QuizQuestion
                {
                    QuestionText = "In which year was the first Pokémon game released?",
                    CorrectAnswer = "1996",
                    Choice1 = "1996",
                    Choice2 = "1998",
                    Choice3 = "2000",
                    Choice4 = "1994"
                },
                new QuizQuestion
                {
                    QuestionText = "Which of these is NOT a core Pokémon type?",
                    CorrectAnswer = "Light",
                    Choice1 = "Light",
                    Choice2 = "Psychic",
                    Choice3 = "Electric",
                    Choice4 = "Normal"
                },
                new QuizQuestion
                {
                    QuestionText = "What is the name of the main character in the Halo series?",
                    CorrectAnswer = "Master Chief",
                    Choice1 = "Master Chief",
                    Choice2 = "Arbiter",
                    Choice3 = "Cortana",
                    Choice4 = "Commander Keyes"
                },
                new QuizQuestion
                {
                    QuestionText = "Which game series features battles between 'Terrans', 'Protoss', and 'Zerg'?",
                    CorrectAnswer = "StarCraft",
                    Choice1 = "StarCraft",
                    Choice2 = "Warcraft",
                    Choice3 = "Diablo",
                    Choice4 = "Command & Conquer"
                },
                new QuizQuestion
                {
                    QuestionText = "What is the primary setting of the Elder Scrolls series?",
                    CorrectAnswer = "Tamriel",
                    Choice1 = "Tamriel",
                    Choice2 = "Azeroth",
                    Choice3 = "Nirn",
                    Choice4 = "Cyrodiil"
                },
                new QuizQuestion
                {
                    QuestionText = "In Minecraft, what material is required to activate a Nether Portal?",
                    CorrectAnswer = "Obsidian",
                    Choice1 = "Obsidian",
                    Choice2 = "Diamond",
                    Choice3 = "Gold",
                    Choice4 = "Iron"
                },
                new QuizQuestion
                {
                    QuestionText = "Which game introduces the 'VATS' combat system?",
                    CorrectAnswer = "Fallout 3",
                    Choice1 = "Fallout 3",
                    Choice2 = "The Elder Scrolls IV: Oblivion",
                    Choice3 = "Skyrim",
                    Choice4 = "New Vegas"
                },
                new QuizQuestion
                {
                    QuestionText = "What is the name of the antagonist in the original Metal Gear Solid?",
                    CorrectAnswer = "Liquid Snake",
                    Choice1 = "Liquid Snake",
                    Choice2 = "Solid Snake",
                    Choice3 = "Revolver Ocelot",
                    Choice4 = "Psycho Mantis"
                },
                new QuizQuestion
                {
                    QuestionText = "Which game features the mechanic of 'Portal' creation?",
                    CorrectAnswer = "Portal",
                    Choice1 = "Portal",
                    Choice2 = "Half-Life 2",
                    Choice3 = "Team Fortress 2",
                    Choice4 = "Left 4 Dead"
                },
                new QuizQuestion
                {
                    QuestionText = "What is the primary mechanic in the game 'Tetris'?",
                    CorrectAnswer = "Arranging falling blocks",
                    Choice1 = "Arranging falling blocks",
                    Choice2 = "Avoiding obstacles",
                    Choice3 = "Collecting items",
                    Choice4 = "Defeating enemies"
                },
                new QuizQuestion
                {
                    QuestionText = "Which game is famous for its 'BioShock' series tagline 'Would You Kindly'?",
                    CorrectAnswer = "BioShock",
                    Choice1 = "BioShock",
                    Choice2 = "System Shock",
                    Choice3 = "Deus Ex",
                    Choice4 = "Rapture"
                }
            };

            context.QuizQuestions.AddRange(quizQuestions);
            context.SaveChanges();
        }

        public static void SeedGames(AppDbContext context)
        {
            if (context.Games.Any())
            {
                return; // already seeded
            }

            var games = new List<Models.Game>
            {
                new Models.Game
                {
                    Title = "The Legend of Zelda: Breath of the Wild",
                    Slug = "zelda-botw",
                    Description = "An open-world action-adventure game set in Hyrule.",
                    Genres = "Action,Adventure,Open World",
                    Platforms = "Nintendo Switch",
                    Tags = "open-world,exploration,puzzle",
                    ImageUrl = "https://example.com/images/zelda.jpg",
                    Rating = 4.9,
                    Price = 59.99m,
                    ReleaseDate = new DateTime(2017,3,3),
                    AddedAt = DateTime.UtcNow
                },
                new Models.Game
                {
                    Title = "Stardew Valley",
                    Slug = "stardew-valley",
                    Description = "A farming RPG where players build a farm and relationships.",
                    Genres = "Simulation,RPG,Indie",
                    Platforms = "PC, Nintendo Switch, PS4, Xbox One",
                    Tags = "farming,crafting,relaxing",
                    ImageUrl = "https://example.com/images/stardew.jpg",
                    Rating = 4.7,
                    Price = 14.99m,
                    ReleaseDate = new DateTime(2016,2,26),
                    AddedAt = DateTime.UtcNow
                },
                new Models.Game
                {
                    Title = "Hades",
                    Slug = "hades",
                    Description = "A roguelike dungeon crawler from Supergiant Games.",
                    Genres = "Action,Roguelike,Indie",
                    Platforms = "PC, Nintendo Switch, PS4, PS5, Xbox",
                    Tags = "roguelike,action,story-driven",
                    ImageUrl = "https://example.com/images/hades.jpg",
                    Rating = 4.8,
                    Price = 24.99m,
                    ReleaseDate = new DateTime(2020,9,17),
                    AddedAt = DateTime.UtcNow
                },
                new Models.Game
                {
                    Title = "Celeste",
                    Slug = "celeste",
                    Description = "A precision platformer about climbing a mountain.",
                    Genres = "Platformer,Indie",
                    Platforms = "PC, Nintendo Switch, PS4, Xbox",
                    Tags = "platformer,difficult,indie",
                    ImageUrl = "https://example.com/images/celeste.jpg",
                    Rating = 4.6,
                    Price = 19.99m,
                    ReleaseDate = new DateTime(2018,1,25),
                    AddedAt = DateTime.UtcNow
                },
                new Models.Game
                {
                    Title = "Portal 2",
                    Slug = "portal-2",
                    Description = "A first-person puzzle-platform game with portals.",
                    Genres = "Puzzle,Platformer",
                    Platforms = "PC, PS3, Xbox 360",
                    Tags = "puzzle,co-op,physics",
                    ImageUrl = "https://example.com/images/portal2.jpg",
                    Rating = 4.9,
                    Price = 9.99m,
                    ReleaseDate = new DateTime(2011,4,19),
                    AddedAt = DateTime.UtcNow
                },
                new Models.Game
                {
                    Title = "Hollow Knight",
                    Slug = "hollow-knight",
                    Description = "A challenging Metroidvania with a beautiful hand-drawn world.",
                    Genres = "Action,Metroidvania,Indie",
                    Platforms = "PC, Nintendo Switch, PS4, Xbox One",
                    Tags = "metroidvania,exploration,challenging",
                    ImageUrl = "https://example.com/images/hollowknight.jpg",
                    Rating = 4.8,
                    Price = 14.99m,
                    ReleaseDate = new DateTime(2017,2,24),
                    AddedAt = DateTime.UtcNow
                },
                new Models.Game
                {
                    Title = "Minecraft",
                    Slug = "minecraft",
                    Description = "A sandbox game about placing blocks and going on adventures.",
                    Genres = "Sandbox,Survival,Indie",
                    Platforms = "PC, Console, Mobile",
                    Tags = "sandbox,building,creative,survival",
                    ImageUrl = "https://example.com/images/minecraft.jpg",
                    Rating = 4.5,
                    Price = 26.95m,
                    ReleaseDate = new DateTime(2011,11,18),
                    AddedAt = DateTime.UtcNow
                },
                new Models.Game
                {
                    Title = "The Witcher 3: Wild Hunt",
                    Slug = "witcher-3",
                    Description = "An open-world RPG following Geralt of Rivia.",
                    Genres = "RPG,Open World,Action",
                    Platforms = "PC, PS4, PS5, Xbox One, Xbox Series X",
                    Tags = "rpg,story-driven,open-world",
                    ImageUrl = "https://example.com/images/witcher3.jpg",
                    Rating = 4.9,
                    Price = 39.99m,
                    ReleaseDate = new DateTime(2015,5,19),
                    AddedAt = DateTime.UtcNow
                },
                new Models.Game
                {
                    Title = "DOOM Eternal",
                    Slug = "doom-eternal",
                    Description = "A fast-paced first-person shooter with intense combat.",
                    Genres = "Shooter,Action",
                    Platforms = "PC, PS4, PS5, Xbox One, Xbox Series X, Nintendo Switch",
                    Tags = "fps,action,fast-paced",
                    ImageUrl = "https://example.com/images/doometernal.jpg",
                    Rating = 4.4,
                    Price = 59.99m,
                    ReleaseDate = new DateTime(2020,3,20),
                    AddedAt = DateTime.UtcNow
                },
                new Models.Game
                {
                    Title = "God of War (2018)",
                    Slug = "god-of-war-2018",
                    Description = "A narrative-driven action game following Kratos and Atreus.",
                    Genres = "Action,Adventure",
                    Platforms = "PS4, PS5, PC",
                    Tags = "action,narrative,adventure",
                    ImageUrl = "https://example.com/images/godofwar.jpg",
                    Rating = 4.9,
                    Price = 49.99m,
                    ReleaseDate = new DateTime(2018,4,20),
                    AddedAt = DateTime.UtcNow
                },
                new Models.Game
                {
                    Title = "Among Us",
                    Slug = "among-us",
                    Description = "A social deduction multiplayer game.",
                    Genres = "Multiplayer,Party,Indie",
                    Platforms = "PC, Mobile, Nintendo Switch, PS4, PS5, Xbox",
                    Tags = "multiplayer,social-deduction,party",
                    ImageUrl = "https://example.com/images/amongus.jpg",
                    Rating = 4.0,
                    Price = 4.99m,
                    ReleaseDate = new DateTime(2018,6,15),
                    AddedAt = DateTime.UtcNow
                },
                new Models.Game
                {
                    Title = "Terraria",
                    Slug = "terraria",
                    Description = "A 2D sandbox adventure game with crafting and exploration.",
                    Genres = "Sandbox,Adventure,Indie",
                    Platforms = "PC, Console, Mobile",
                    Tags = "sandbox,crafting,exploration",
                    ImageUrl = "https://example.com/images/terraria.jpg",
                    Rating = 4.3,
                    Price = 9.99m,
                    ReleaseDate = new DateTime(2011,5,16),
                    AddedAt = DateTime.UtcNow
                }
            };

            context.Games.AddRange(games);
            context.SaveChanges();
        }

        public static void SeedGameReleaseTimelines(AppDbContext context)
        {
            var gamesWithoutTimeline = context.Games
                .Where(game => game.ReleaseDate.HasValue && !context.GameUpdates.Any(update => update.GameId == game.Id))
                .ToList();

            if (gamesWithoutTimeline.Count == 0)
            {
                return;
            }

            foreach (var game in gamesWithoutTimeline)
            {
                context.GameUpdates.Add(new GameUpdate
                {
                    GameId = game.Id,
                    Title = "วางจำหน่ายแล้ว",
                    Description = $"{game.Title} เปิดตัวและเริ่มให้ผู้เล่นได้สัมผัสเกมเป็นครั้งแรก",
                    PublishedAt = game.ReleaseDate!.Value
                });
                game.LastUpdatedAt = game.ReleaseDate;
            }

            context.SaveChanges();
        }
    }
}
