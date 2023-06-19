namespace TextEditor
{
    struct InputKey
    {
        public ConsoleKeyInfo key { get; init; }

        public InputKey(ConsoleKeyInfo key)
        {
            this.key = key;
        }
    }
    class TextEditor
    {
        int totalRows;
        int columnsIntheCurrentRow;
        int cursorRow;
        List<InputKey> inputKeys = new List<InputKey>(); 
        
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
                var (cursorLeft, cursorTop) = Console.GetCursorPosition();
                consoleInput = Console.ReadKey();
                switch (consoleInput.Key)
                {
                    case ConsoleKey.Enter:
                        editor.inputKeys.Add(new InputKey(consoleInput));
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
                    case ConsoleKey.Backspace:
                        if (cursorLeft == 0 && cursorTop != 0) //deletion of ENTER across paragraphs
                            editor.HandleParagraphBackspace();
                        else if (cursorLeft != 0) //we delete midline
                            editor.HandleLiniarBackspace();
                        
                        break;
                    default:
                        if (editor.IsAddedMidParagraph(cursorLeft, cursorTop))
                        {
                            editor.HandleMidParagraphInsertion(consoleInput);
                        }
                        else
                        {
                            editor.inputKeys.Add(new InputKey(consoleInput));
                            editor.CheckForNewRowThroughChar();
                        }
                        break;
                }
            } while (consoleInput.Key != ConsoleKey.Escape); //close app on ESC
        }

        private void HandleMidParagraphInsertion(ConsoleKeyInfo key)
        {
            var (cursorLeft, cursorTop) = Console.GetCursorPosition();
            int newKeyIndex = GetCurrentKeyIndex(cursorLeft, cursorTop)-1;

            //save in the indexKeys
            inputKeys.Insert(newKeyIndex, new InputKey(key));

            //update numberOfColumnsPerRow depending on if new row is formed or not
            int indexOfEndChar = GetIndexOfLastCharInParagraph(newKeyIndex);
            DisplayInputKeys(newKeyIndex+1, indexOfEndChar);

            if (IsOnNewRow(indexOfEndChar))
            {
                columnsIntheCurrentRow = 1;
                if (numberOfColumnsPerRow.ContainsKey(cursorRow))
                {
                    for (int i = numberOfColumnsPerRow.Count - 1; i >= cursorRow; i--)
                    {
                        ChangeKey(numberOfColumnsPerRow, i, i + 1);
                    }
                }
                numberOfColumnsPerRow.Add(cursorRow, columnsIntheCurrentRow);
                totalRows++;
            }
            else
            {
                numberOfColumnsPerRow[cursorTop]++;
            }

            DisplayInputKeys(indexOfEndChar + 1);
            Console.SetCursorPosition(cursorLeft, cursorTop);
        }

        private bool IsOnNewRow(int indexOfEndChar)
        {
            var (_, cursorTop) = Console.GetCursorPosition();
            cursorRow = cursorTop;
            int oldRowOfEndChar = GetRowOfChar(indexOfEndChar);
            return cursorRow != oldRowOfEndChar;
        }

        private int GetRowOfChar(int indexOfEndChar)
        {
            int row = 0;
            for (int i = 0; i < numberOfColumnsPerRow.Count && indexOfEndChar > numberOfColumnsPerRow[i]; i++)
            {
                indexOfEndChar -= numberOfColumnsPerRow[i];
                row++;
            }
            return row;
        }

        private int GetIndexOfLastCharInParagraph(int newKeyIndex)
        {
            int indexOfNextEnter = inputKeys.FindIndex(newKeyIndex, inputKeys.Count - newKeyIndex, (key) => key.key.Key.ToString() == "Enter");
            if(indexOfNextEnter == -1)
            {
                return inputKeys.Count - 1; //the new element has already been added into inputKeys
            }   
            return indexOfNextEnter;
        }

        private bool IsAddedMidParagraph(int cursorLeft, int cursorTop)
        {
            int currentKeyIndex = inputKeys.Count == 0 ? 0 : GetCurrentKeyIndex(cursorLeft, cursorTop);
            return currentKeyIndex < inputKeys.Count;
        }

        private void HandleParagraphBackspace()
        {
            //these should still be on the lower row
            var (_, cursorTop) = Console.GetCursorPosition();

            //enter index is on the upper row
            int enterIndex = GetCurrentKeyIndex(0, cursorTop)-1;

            //delete '/r' from inputKeys
            inputKeys.RemoveAt(enterIndex);

            //initial Number Of Columns On Upper Row Without Enter
            int initialNoColumns = numberOfColumnsPerRow[cursorTop - 1] - 1;

            //update numberOfColumnsPerRow
            numberOfColumnsPerRow[cursorTop - 1] = initialNoColumns + numberOfColumnsPerRow[cursorTop];
            cursorRow = cursorTop - 1;
            Console.Clear();
            DisplayInputKeys(0);
            Console.Write(' ');
            Console.SetCursorPosition(initialNoColumns, cursorRow);
        }
        private int GetCurrentKeyIndex(int startingPosition, int cursorTop)
        {
            int keyIndex = startingPosition;
            for (int i = 0; i < cursorTop; i++)
            {
                keyIndex += numberOfColumnsPerRow[i];
            }
            return keyIndex;
        }
        private void DisplayInputKeys(int startingPosition)
        {
            for (int i = startingPosition; i < inputKeys.Count; i++)
                if (inputKeys[i].key.Key.ToString() == "Enter")
                    Console.Write(" \n");
                else Console.Write(inputKeys[i].key.KeyChar);
        }
        private void DisplayInputKeys(int startingPosition, int endingPosition)
        {
            for (int i = startingPosition; i <= endingPosition; i++)
                if (inputKeys[i].key.Key.ToString() == "Enter")
                    Console.Write(" \n");
                else Console.Write(inputKeys[i].key.KeyChar);
        }

        private void HandleLiniarBackspace()
        {
            //Backspace already knows to move the cursor once to the left
            var (cursorLeft, cursorTop) = Console.GetCursorPosition();
            int keyIndex = GetCurrentKeyIndex(cursorLeft, cursorTop);
           if (keyIndex >= 0)
            {
                inputKeys.RemoveAt(keyIndex);
                if(cursorLeft == numberOfColumnsPerRow[cursorRow]-1)
                {
                    Console.Write(' ');
                    ArrowMoveCursor(-1, 0);
                }
                else
                {
                    DisplayInputKeys(keyIndex);
                    Console.Write(' ');
                    Console.SetCursorPosition(cursorLeft, cursorTop);
                }
                numberOfColumnsPerRow[cursorRow]--;
            }
            
        }
        private static void ChangeKey<TKey, TValue>(IDictionary<TKey, TValue> dic,
                                      TKey fromKey, TKey toKey)
        {
            TValue value = dic[fromKey];
            dic.Remove(fromKey);
            dic[toKey] = value;
        }
        private void CheckForNewRowThroughChar()
        {
            var (_, Top) = Console.GetCursorPosition();

            if (Top != cursorRow)
            {
                cursorRow = Top;
                columnsIntheCurrentRow = 1;
                if (numberOfColumnsPerRow.ContainsKey(cursorRow)){
                    for(int i = numberOfColumnsPerRow.Count-1; i>=cursorRow; i--)
                    {
                        ChangeKey(numberOfColumnsPerRow, i, i + 1);
                    }
                }
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

            columnsIntheCurrentRow++;
            numberOfColumnsPerRow[cursorRow] = columnsIntheCurrentRow;
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