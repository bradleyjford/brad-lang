using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace BradLang.CommandLine;

internal abstract partial class Repl
{
    private readonly List<string> _history = new List<string>();
    private int _historyIndex;

    private bool _done;

    protected abstract void EvaluateMetaCommand(string command);
    protected abstract void EvaluateCommand(string command);
    protected abstract bool IsCompleteDocument(string text);

    public void Run()
    {
        while (true)
        {
            var command = ReadCommand();

            if (String.IsNullOrEmpty(command))
            {
                return;
            }

            if (!command.Contains(Environment.NewLine) && command.StartsWith("#"))
            {
                EvaluateMetaCommand(command);
            }
            else
            {
                EvaluateCommand(command);
            }

            _history.Add(command);
            _historyIndex = 0;
        }
    }

    private string ReadCommand()
    {
        var document = new ObservableCollection<string>() { "" };
        var view = new DocumentView(document, RenderLine);

        _done = false;

        while (!_done)
        {
            var key = Console.ReadKey();

            HandleKey(key, document, view);
        }

        view.CurrentLine = document.Count - 1;
        view.CurrentCharacter = document[view.CurrentLine].Length;

        Console.WriteLine();

        return String.Join(Environment.NewLine, document);
    }

    private void HandleKey(ConsoleKeyInfo key, ObservableCollection<string> document, DocumentView view)
    {
        if (key.Modifiers == default)
        {
            switch (key.Key)
            {
                case ConsoleKey.Enter:
                    HandleEnter(document, view);
                    break;

                case ConsoleKey.Escape:
                    HandleEscape(document, view);
                    break;

                case ConsoleKey.Backspace:
                    HandleBackspace(document, view);
                    break;

                case ConsoleKey.Delete:
                    HandleDelete(document, view);
                    break;

                case ConsoleKey.Tab:
                    HandleTab(document, view);
                    break;

                case ConsoleKey.PageUp:
                    HandlePageUp(document, view);
                    break;

                case ConsoleKey.PageDown:
                    HandlePageDown(document, view);
                    break;

                case ConsoleKey.LeftArrow:
                    HandleLeftArrow(document, view);
                    break;

                case ConsoleKey.RightArrow:
                    HandleRightArrow(document, view);
                    break;

                case ConsoleKey.UpArrow:
                    HandleUpArrow(document, view);
                    break;

                case ConsoleKey.DownArrow:
                    HandleDownArrow(document, view);
                    break;

                case ConsoleKey.Home:
                    HandleHome(document, view);
                    break;

                case ConsoleKey.End:
                    HandleEnd(document, view);
                    break;
            }
        }
        else if (key.Modifiers == ConsoleModifiers.Control)
        {
            switch (key.Key)
            {
                case ConsoleKey.Enter:
                    HandleControlEnter(document, view);
                    break;
            }
        }

        if (key.KeyChar >= ' ')
        {
            HandleOtherKey(key.KeyChar, document, view);
        }
    }

    private void HandleControlEnter(ObservableCollection<string> document, DocumentView view)
    {
        InsertLine(document, view);
    }

    private void HandleEnter(ObservableCollection<string> document, DocumentView view)
    {
        var submissionText = String.Join(Environment.NewLine, document);

        if (submissionText.StartsWith("#") || IsCompleteDocument(submissionText))
        {
            _done = true;
            return;
        }

        InsertLine(document, view);
    }

    private static void InsertLine(ObservableCollection<string> document, DocumentView view)
    {
        var remainder = document[view.CurrentLine].Substring(view.CurrentCharacter);

        document[view.CurrentLine] = document[view.CurrentLine].Substring(0, view.CurrentCharacter);

        var lineIndex = view.CurrentLine + 1;

        document.Insert(lineIndex, remainder);
            
        view.CurrentCharacter = 0;
        view.CurrentLine = lineIndex;
    }

    private void HandleEscape(ObservableCollection<string> document, DocumentView view)
    {
        document.Clear();

        document.Add(String.Empty);

        view.CurrentLine = 0;
        view.CurrentCharacter = 0;
    }

    private void HandleTab(ObservableCollection<string> document, DocumentView view)
    {
        const int TabWidth = 4;

        var start = view.CurrentCharacter;
        var remainingSpaces = TabWidth - start % TabWidth;
        var line = document[view.CurrentLine];

        document[view.CurrentLine] = line.Insert(start, new string(' ', remainingSpaces));

        view.CurrentCharacter += remainingSpaces;
    }

    private void HandleBackspace(ObservableCollection<string> document, DocumentView view)
    {
        var start = view.CurrentCharacter;

        if (start == 0)
        {
            if (view.CurrentLine == 0)
            {
                return;
            }

            var currentLine = document[view.CurrentLine];
            var previousLine = document[view.CurrentLine - 1];

            document.RemoveAt(view.CurrentLine);

            view.CurrentLine--;

            document[view.CurrentLine] = previousLine + currentLine;

            view.CurrentCharacter = previousLine.Length;
        }
        else
        {
            var lineIndex = view.CurrentLine;
            var line = document[lineIndex];

            var before = line.Substring(0, start - 1);
            var after = line.Substring(start);

            document[lineIndex] = before + after;

            view.CurrentCharacter--;
        }
    }

    private void HandleDelete(ObservableCollection<string> document, DocumentView view)
    {
        var lineIndex = view.CurrentLine;
        var line = document[lineIndex];
        var start = view.CurrentCharacter;

        if (start >= line.Length)
        {
            if (view.CurrentLine == document.Count - 1)
            {
                return;
            }

            var nextLine = document[view.CurrentLine + 1];

            document[view.CurrentLine] += nextLine;
            document.RemoveAt(view.CurrentLine + 1);

            return;
        }

        var before = line.Substring(0, start);
        var after = line.Substring(start + 1);

        document[lineIndex] = before + after;
    }

    private void HandleHome(ObservableCollection<string> document, DocumentView view)
    {
        view.CurrentCharacter = 0;
    }

    private void HandleEnd(ObservableCollection<string> document, DocumentView view)
    {
        view.CurrentCharacter = document[view.CurrentLine].Length;
    }

    private void HandlePageUp(ObservableCollection<string> document, DocumentView view)
    {
        _historyIndex--;

        if (_historyIndex < 0)
        {
            _historyIndex = _history.Count - 1;
        }
            
        SetDocumentFromHistory(document, view);
    }

    private void HandlePageDown(ObservableCollection<string> document, DocumentView view)
    {
        _historyIndex++;

        if (_historyIndex > _history.Count - 1)
        {
            _historyIndex = 0;
        }
            
        SetDocumentFromHistory(document, view);
    }

    private void SetDocumentFromHistory(ObservableCollection<string> document, DocumentView view)
    {
        if (_history.Count== 0)
        {
            return;
        }

        var lines = _history[_historyIndex].Split(Environment.NewLine);

        document.Clear();

        foreach (var line in lines)
        {
            document.Add(line);
        }

        view.CurrentLine = document.Count - 1;
        view.CurrentCharacter = document[document.Count - 1].Length - 1;
    }

    private void HandleLeftArrow(ObservableCollection<string> document, DocumentView view)
    {
        if (view.CurrentCharacter > 0)
        {
            view.CurrentCharacter--;
        }
    }

    private void HandleRightArrow(ObservableCollection<string> document, DocumentView view)
    {
        var line = document[view.CurrentLine];

        if (view.CurrentCharacter <= line.Length - 1)
        {
            view.CurrentCharacter++;
        }
    }

    private void HandleUpArrow(ObservableCollection<string> document, DocumentView view)
    {
        if (view.CurrentLine > 0)
        {
            view.CurrentLine--;
        }
    }

    private void HandleDownArrow(ObservableCollection<string> document, DocumentView view)
    {
        if (view.CurrentLine < document.Count - 1)
        {
            view.CurrentLine++;
        }
    }

    private void HandleOtherKey(char key, ObservableCollection<string> document, DocumentView view)
    {
        var text = key.ToString();

        var lineIndex = view.CurrentLine;
        var start = view.CurrentCharacter;

        document[lineIndex] = document[lineIndex].Insert(start, text);
            
        view.CurrentCharacter += text.Length;        
    }

    protected virtual void RenderLine(string line)
    {
        Console.Write(line);
    }
}
