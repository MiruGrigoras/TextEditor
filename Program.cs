using System;
namespace TextEditor
{
    class TextEditor
    {
        int totalRows;
        int columnsIntheCurrentRow;
        int cursorRow;
        
        // contains each row key and the number of coresponding characters on that row 
        Dictionary<int, int> numberOfColumnsPerRow = new Dictionary<int, int>();

        public TextEditor()
        {
             cursorRow = totalRows = Console.CursorTop;
             columnsIntheCurrentRow = Console.CursorLeft;
        }
        static void Main(string[] args)
        {
            TextEditor editor = new();
            ConsoleKeyInfo consoleInput;

            do
            {
                consoleInput = Console.ReadKey();
                switch(consoleInput.Key)
                {
                    case ConsoleKey.Enter:
                        editor.EnterMoveCursor();
                        break;
                    case ConsoleKey.DownArrow:
                        editor.ArrowMoveCursor(0, 1);
                        break;
                    case ConsoleKey.UpArrow:
                        editor.ArrowMoveCursor(0, -1);
                        break;
                    case ConsoleKey.LeftArrow:
                        editor.ArrowMoveCursor(-1, 0);
                        break;
                    case ConsoleKey.RightArrow:
                        editor.ArrowMoveCursor(1, 0);
                        break;
                    default: 
                        editor.CheckForNewRowThroughChar();
                        break;
                }
            } while (consoleInput.Key != ConsoleKey.Escape); //close app on ESC
        }

        private void CheckForNewRowThroughChar()
        {
            var (_, Top) = Console.GetCursorPosition();
            if (Top != cursorRow)
            {
                cursorRow = Top;
                columnsIntheCurrentRow = 0;
                numberOfColumnsPerRow.Add(cursorRow, columnsIntheCurrentRow);
                totalRows++;
            }
            else
            {
                columnsIntheCurrentRow++;
                numberOfColumnsPerRow[cursorRow] = columnsIntheCurrentRow;
            }
        }

        private void EnterMoveCursor()
        {
            cursorRow += 1;
            totalRows++;
            columnsIntheCurrentRow = 0;
            numberOfColumnsPerRow.Add(cursorRow, columnsIntheCurrentRow);
            Console.SetCursorPosition(0, cursorRow);
        }

        private void ArrowMoveCursor(int columnOffset, int rowOffset)
        {
            int newRow = cursorRow + rowOffset;
            int newColumn = Console.GetCursorPosition().Left + columnOffset;
            
            if (NewPositionInsideBounds(newRow, newColumn))
            {
                if(newColumn > numberOfColumnsPerRow[newRow])
                {
                    if(newRow + 1 <= totalRows && rowOffset == 0)
                    {
                        newRow += 1;
                        newColumn = 0;
                    }
                    else
                    {
                        newColumn = numberOfColumnsPerRow[newRow];
                    }
                }
                else if(newColumn < 0){
                    if (newRow - 1 >= 0 && rowOffset == 0)
                    {
                        newRow -= 1;
                        newColumn = numberOfColumnsPerRow[newRow];
                    }
                    else
                    {
                        newColumn = 0;
                    }
                }
                cursorRow = newRow;
                Console.SetCursorPosition(newColumn, newRow);
            }
        }

        private bool NewPositionInsideBounds(int newRow, int newColumn)
        {
            return newRow <= totalRows && newRow >= 0;
        }
    }
}