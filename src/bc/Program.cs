namespace BradLang.CommandLine;

public static class Program
{
    public static void Main()
    {
        var repl = new BradLangRepl();

        repl.Run();
    }
}
