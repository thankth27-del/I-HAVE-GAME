using I_HAVE_GAME.Models;

namespace I_HAVE_GAME.Data
{
    public static class DbSeeder
    {
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
    }
}
