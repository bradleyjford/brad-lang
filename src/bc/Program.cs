using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using BradLang.CodeAnalysis;
using BradLang.CodeAnalysis.Syntax;
using BradLang.CodeAnalysis.Text;

namespace BradLang.CommandLine
{
    static class Program
    {
        static void Main()
        {
            var repl = new BradLangRepl();

            repl.Run();
        }
    }
}
