using System;
using System.Collections.ObjectModel;

namespace BradLang.CommandLine;

internal partial class Repl
{
    private class DocumentView
    {
        private readonly ObservableCollection<string> _document;
        private readonly Action<string> _lineRenderer;
        private readonly int _cursorTop;
        private int _currentLine;
        private int _currentCharacter;
        private int _renderedLineCount;

        public DocumentView(ObservableCollection<string> document, Action<string> lineRenderer)
        {
            _cursorTop = Console.CursorTop;

            _document = document;
            _document.CollectionChanged += (s, args) => Render();

            _lineRenderer = lineRenderer;

            Render();
        }

        private void UpdateCursorPosition()
        {
            Console.CursorTop = _cursorTop + _currentLine;

            // TODO: Handle scenario where line should wrap - currently throws
            Console.CursorLeft = _currentCharacter + 2;
        }

        public int CurrentLine
        {
            get => _currentLine;

            set
            {
                if (_currentLine != value)
                {
                    _currentLine = value;
                    _currentCharacter = Math.Min(_document[_currentLine].Length, _currentCharacter);

                    UpdateCursorPosition();
                }
            }
        }

        public int CurrentCharacter
        {
            get => _currentCharacter;

            set
            {
                if (_currentCharacter != value)
                {
                    _currentCharacter = value;
                        
                    UpdateCursorPosition();
                }
            }
        }

        public void Render()
        {
            Console.CursorVisible = false;

            var lineCount = 0;

            foreach (var line in _document)
            {
                Console.SetCursorPosition(0, _cursorTop + lineCount);
    
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(lineCount == 0 ? "» " : "· ");
                Console.ResetColor();

                _lineRenderer(line);

                Console.WriteLine(new string(' ', Console.WindowWidth - line.Length - 2));

                lineCount++;
            }

            var requiredBlankLines = _renderedLineCount - lineCount;

            if (requiredBlankLines > 0)
            {
                var blankLine = new string(' ', Console.WindowWidth);

                for (var i = 0; i < requiredBlankLines; i++)
                {
                    Console.SetCursorPosition(0, _cursorTop + lineCount + i);
                    Console.WriteLine(blankLine);
                }
            }

            _renderedLineCount = lineCount;

            Console.CursorVisible = true;

            UpdateCursorPosition();
        }
    }
}
