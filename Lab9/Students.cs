sealed record Student(string Name, string Group, int Score)
{
    public override string ToString() => $"{Name} ({Group}, {Score})";
}

static class Students
{
    public static List<Student> FromLab6() =>
    [
        new Student("Коваленко", "КН-3/1", 88),
        new Student("Мельник",   "КН-3/2", 64),
        new Student("Іщенко",    "КН-3/1", 93),
        new Student("Бондар",    "КН-3/1", 71),
        new Student("Савченко",  "КН-3/2", 55),
        new Student("Ткачук",    "КН-3/2", 90),
        new Student("Кравець",   "КН-3/1", 76),
        new Student("Іванів",    "КН-3/1", 48),
        new Student("Мороз",     "КН-3/2", 82),
        new Student("Шевчук",    "КН-3/1", 67),
        new Student("Бойко",     "КН-3/2", 95),
        new Student("Кушнір",    "КН-3/2", 73),
    ];
}
