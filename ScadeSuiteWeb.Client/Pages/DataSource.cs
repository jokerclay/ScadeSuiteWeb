
namespace ScadeSuiteWeb.Client.Pages;

public class DataSource
{

    public List<string> Hits { get; } =
    [
        "Please Please Me",
        "With The Beatles",
        "A Hard Day's Night",
        "Beatles for Sale",
        "Help!",
        "Rubber Soul",
        "Revolver",
        "Sgt. Pepper's Lonely Hearts Club Band",
        "Magical Mystery Tour",
        "The Beatles",
        "Yellow Submarine",
        "Abbey Road",
        "Let It Be",
    ];

    public List<string> Sizes { get; } =
    [
        "Extra small",
        "Small",
        "Medium",
        "Large",
        "Extra Large"
    ];

    public static async Task WaitAsync(int milliseconds, Action action)
    {
        var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(milliseconds));
        while (await timer.WaitForNextTickAsync())
        {
            timer.Dispose();
            action.Invoke();
        };
    }

    public class MonthItem
    {
        public string Index { get; set; } = "00";
        public string Name { get; set; } = "";
        
        public override string ToString() => $"{Index:00} {Name}";
    }

    public static MonthItem[] AllMonths = Enumerable.Range(0, 12)
                                            .Select(i => new MonthItem
                                            {
                                                Index = $"{i + 1:00}",
                                                Name = GetMonthName(i)
                                            })
                                            .ToArray();

    private static string GetMonthName(int index)
    {
        return System.Globalization
                     .DateTimeFormatInfo
                     .CurrentInfo
                     .MonthNames
                     .ElementAt(index % 12);
    }

}