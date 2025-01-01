/*using Microsoft.Extensions.Logging;

namespace Sensei.Common.Infrastructure.Persistence;

public static class MigrationManager
{
    public static IHost MigrateDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var environment = services.GetRequiredService<IHostEnvironment>();
        var logger = services.GetRequiredService<ILogger<BetsyDbContext>>();
        if (!environment.IsDevelopment()) return host;

        try
        {
            var context = services.GetService<BetsyDbContext>();
            context!.Database.EnsureCreated();

            SeedData(context, services.GetRequiredService<IPasswordHasher>());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
        }

        return host;
    }

    private static void SeedData(BetsyDbContext context, IPasswordHasher passwordHasher)
    {
        SeedUser(context, passwordHasher);
        var footballMatches = SeedFootballMatches(context);
        
        if(!footballMatches.Any())
            return;

        var tennisMatches = SeedTennisMatches(context);
        SeedOffers(context, footballMatches);
        SeedOffers(context, tennisMatches);
    }

    private static void SeedUser(BetsyDbContext context, IPasswordHasher passwordHasher)
    {
        var user = context.Set<User>().FirstOrDefault(x => x.Email == "jerry@betsy.hr");

        if (user is not null) return;

        // For the simplicity of the example, we are seeding one user with a password hash
        // this is not done in configurator because of issue in ef with navigation props
        // https://github.com/dotnet/efcore/issues/31373
        var hashedPassword = passwordHasher.HashPassword("P@ssword12").Value;
        var seedUser = new User("Jerry", "Cantrell", "jerry@betsy.hr", hashedPassword, "EUR");
        seedUser.CreatedBy = Guid.Parse("00000000-0000-0000-0000-000000000001");
        seedUser.ModifiedBy = Guid.Parse("00000000-0000-0000-0000-000000000001");
        seedUser.CreatedAt = DateTime.UtcNow;
        seedUser.ModifiedAt = DateTime.UtcNow;

        context.Add(seedUser);
        context.SaveChanges();
    }

    private static List<Match> SeedFootballMatches(BetsyDbContext context)
    {
        var matches = new List<Match>();
        if (context.Set<Match>().Any())
            return matches;

        matches.Add(GetFootballMatch("Dinamo", "Hajduk"));
        matches.Add(GetFootballMatch("Barcelona", "Real Madrid"));
        matches.Add(GetFootballMatch("Bayern", "Lille"));
        matches.Add(GetFootballMatch("Juventus", "Milan"));
        matches.Add(GetFootballMatch("PSG", "Marseille"));
        matches.Add(GetFootballMatch("Inter M.", "Stuttgart"));
        matches.Add(GetFootballMatch("Celtic", "Slovan B."));
        matches.Add(GetFootballMatch("Rijeka", "Osijek"));
        matches.Add(GetFootballMatch("Monaco", "Brentford"));
        matches.Add(GetFootballMatch("Chelsea", "Everton"));
        matches.Add(GetFootballMatch("Fulham", "Leicester City"));
        matches.Add(GetFootballMatch("Newcastle United", "Nottingham Forest"));
        matches.Add(GetFootballMatch("Southampton", "Tottenham Hotspur"));
        matches.Add(GetFootballMatch("West Ham United", "Arsenal"));
        matches.Add(GetFootballMatch("AS Roma", "FC Porto"));
        matches.Add(GetFootballMatch("SL Benfica", "Athletic Bilbao"));
        matches.Add(GetFootballMatch("VfB Stuttgart", "Galatasaray"));
        matches.Add(GetFootballMatch("Olympique Marseille", "Olympique Lyon"));
        matches.Add(GetFootballMatch("Valencia CF", "Villarreal CF"));
        matches.Add(GetFootballMatch("Sevilla FC", "SC Freiburg"));
        matches.Add(GetFootballMatch("Leeds United", "Ipswich Town"));
        matches.Add(GetFootballMatch("Stade Reims", "RSC Anderlecht"));

        foreach (var match in matches)
        {
            context.Add(match);
        }

        context.SaveChanges();

        return matches;
    }

    private static List<Match> SeedTennisMatches(BetsyDbContext context)
    {
        var matches = new List<Match>();
        if (context.Set<Match>().Any(x => x.Sport == "Tennis"))
            return matches;

        matches.Add(GetTennisMatch("Jannik Sinner", "Alexander Zverev"));
        matches.Add(GetTennisMatch("Carlos Alcaraz", "Novak Djokovic"));
        matches.Add(GetTennisMatch("Daniil Medvedev", "Andrey Rublev"));
        matches.Add(GetTennisMatch("Taylor Fritz", "Hubert Hurkacz"));
        matches.Add(GetTennisMatch("Casper Ruud", "Grigor Dimitrov"));

        foreach (var match in matches)
        {
            context.Add(match);
        }

        context.SaveChanges();

        return matches;
    }

    private static void SeedOffers(BetsyDbContext context, List<Match> matches)
    {
        var i = 0;
        foreach (var match in matches)
        {
            var offer = new Offer(match.Id, false, match.Sport == Sport.Football.ToString() ? GetFootbalBetTypes() : GetTennisBetTypes(match));
            context.Add(offer);

            if (i++ % 3 != 0) continue;

            var specialOffer = new Offer(match.Id, true, match.Sport == Sport.Football.ToString() ? GetFootbalBetTypes() : GetTennisBetTypes(match));
            context.Add(specialOffer);
        }

        context.SaveChanges();
    }

    private static DateTime GetMatchStartDate()
    {
        var random = new Random();
        var randomHour = random.Next(12, 20);

        var now = DateTime.UtcNow.AddMonths(1);
        return new DateTime(now.Year, now.Month, now.Day, randomHour, 0, 0);
    }

    private static Match GetFootballMatch(string team1, string team2)
    {
        return new Match(team1, $"{team1} vs {team2}", GetMatchStartDate(), Sport.Football, team2);
    }

    private static Match GetTennisMatch(string player1, string player2)
    {
        return new Match(player1, $"{player1} vs {player2}", GetMatchStartDate(), Sport.Tennis, player2);
    }

    private static Dictionary<string, decimal> GetFootbalBetTypes()
    {
        var rnd = new Random();
        var betTypes = new Dictionary<string, decimal>
        {
            { "1", GetRandomQuota(1,3)},
            { "X", GetRandomQuota(2,6)},
            { "2", GetRandomQuota(1,5)}
        };

        return betTypes;
    }

    private static Dictionary<string, decimal> GetTennisBetTypes(Match match)
    {
        var rnd = new Random();
        var betTypes = new Dictionary<string, decimal>
        {
            { match.NameOne, GetRandomQuota(1,4)},
            { match.NameTwo!, GetRandomQuota(4,7)},
        };

        return betTypes;
    }

    private static decimal GetRandomQuota(int from, int to)
    {
        var randomQuota = 0m;
        var rnd = new Random();
        var rndInt = rnd.Next(from, to);

        randomQuota += rndInt;
        randomQuota += (decimal)rnd.NextDouble();

        return decimal.Round(randomQuota, 2);
    }
}*/