using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypingTest.Service;

namespace TypingTest.Model.TextModel
{
    class TestTextModel
    {
        private static readonly string outputTestTextDefaultValue = "Output text";

        private static readonly string whitespaceCharacterTransferCharactersPattern = "( |\\r|\\n)";

        private static readonly string whitespaceCharacterTransferCharactersOppositePattern = "[^" + whitespaceCharacterTransferCharactersPattern + "]";

        private static readonly string multiWhitespaceCharacterTransferCharactersOppositePattern = "[^(" + whitespaceCharacterTransferCharactersPattern + "+)]";

        private static readonly string outputTestTextReplacementString = "⤶";

        private static readonly string outputTestTextTextStartString = "↷";

        private static readonly string outputTestTextTextEndString = "↶";

        private static readonly string outputTestTextReplacementStringTextEndStringPattern = "(" + outputTestTextReplacementString + "|" + outputTestTextTextEndString + ")";

        private static readonly string outputTestTextReplacementStringTextEndStringTextStartStringPattern = outputTestTextReplacementStringTextEndStringPattern + "|" + outputTestTextTextStartString;

        private static readonly string whitespaceCharacterTransferCharacterOutputTestTextReplacementStringTextEndStringTextStartStringPattern =
                                       whitespaceCharacterTransferCharactersPattern + "|" + outputTestTextReplacementStringTextEndStringTextStartStringPattern;

        private static readonly string oneOrNothingWhitespaceCharacterTransferCharacterOutputTestTextReplacementStringTextEndStringTextStartStringPattern = "((" + 
                                       whitespaceCharacterTransferCharacterOutputTestTextReplacementStringTextEndStringTextStartStringPattern + ")?)";

        private static readonly string whitespaceOutputTestTextReplacementStringTextEndStringPattern = " |" + outputTestTextReplacementStringTextEndStringPattern;

        private bool isTextInputPossible = true;

        private bool isWhitespaceInputEnabled;

        private string outputTestText;

        private Stack<InputOutputWordsStatistics> inputOutputWordsStatisticsList;

        public TestTextModel(string outputTestTextModel)
        {
            inputOutputWordsStatisticsList = new Stack<InputOutputWordsStatistics>();
            OutputTestText = outputTestTextModel;
        }

        public bool IsTextInputFinished { get; private set; }

        public int InputCharactersCount { get; private set; }

        public int MistypedWordsCount { get; private set; }

        public int TextInputPosition { get; private set; }

        public string OutputTestText
        {
            get { return outputTestText; }

            set
            {
                string EmptyString = String.Empty;
                if ((value != null) && (value != EmptyString))
                {
                    int firstCorrectOutputTestTextCharacter = RegularExpressionsService.FindInputStringFirstPatternStringStartIndex(multiWhitespaceCharacterTransferCharactersOppositePattern, value, 0);
                    if (firstCorrectOutputTestTextCharacter == -1)
                        LockOutputTestText();
                    else
                    {
                        if (RegularExpressionsService.FindInputStringFirstPatternStringStartIndex(outputTestTextReplacementStringTextEndStringTextStartStringPattern, value, 0) != -1)
                            LockOutputTestText();
                        else
                        {
                            outputTestText = outputTestTextTextStartString + value.Substring(firstCorrectOutputTestTextCharacter,
                                                                                             RegularExpressionsService.FindInputStringFirstPatternStringStartIndex(multiWhitespaceCharacterTransferCharactersOppositePattern,
                                                                                                                                                                   value,
                                                                                                                                                                   value.Length,
                                                                                                                                                                   true) - firstCorrectOutputTestTextCharacter + 1) +
                                             outputTestTextTextEndString;
                            RegularExpressionsService.InsertInInputStringBeforePatternStringFirstCharacterReplacementString(whitespaceCharacterTransferCharactersPattern + "+", " +",
                                                                                                                            outputTestTextReplacementString,
                                                                                                                            ref outputTestText);
                        }
                    }
                }
                else outputTestText = outputTestTextDefaultValue;
                ResetTestTextModelInputStatistics();
            }
        }

        private void AddMistypedWord()
        {
            MistypedWordsCount++;
            InputOutputWordsStatistics lastInputOutputWordsStatistics = GetTopInputOutputWordsStatisticsListElement();
            lastInputOutputWordsStatistics.OutputWordFirstInvalidCharacterLocalPosition = TextInputPosition - lastInputOutputWordsStatistics.OutputWordFirstCharacterPosition;
        }

        private void LockOutputTestText()
        {
            outputTestText = String.Empty;
            isTextInputPossible = false;
        }
        
        private void RemoveMistypedWord()
        {
            MistypedWordsCount--;
            GetTopInputOutputWordsStatisticsListElement().OutputWordFirstInvalidCharacterLocalPosition = -1;
        }

        private void ResetTestTextModelWordCharactersStatistics()
        {
            inputOutputWordsStatisticsList.Push(new InputOutputWordsStatistics(RegularExpressionsService.FindInputStringFirstPatternStringStartIndex(whitespaceCharacterTransferCharactersOppositePattern +
                                                                                                                                                     "(" + 
                                                                                                                                                     whitespaceOutputTestTextReplacementStringTextEndStringPattern 
                                                                                                                                                     + ")", 
                                                                                                                                                     OutputTestText, TextInputPosition) - TextInputPosition + 1, 
                                                                               TextInputPosition));
        }

        private bool CheckPossibilityOfMystypedWordRemoving()
        {
            InputOutputWordsStatistics lastInputOutputWordsStatistics = GetTopInputOutputWordsStatisticsListElement();
            int lastInputOutputWordsStatisticsOutputWordFirstInvalidCharacterLocalPosition = lastInputOutputWordsStatistics.OutputWordFirstInvalidCharacterLocalPosition;
            if (lastInputOutputWordsStatisticsOutputWordFirstInvalidCharacterLocalPosition > -1)
            {
                if (lastInputOutputWordsStatistics.InputWordCharactersCount == lastInputOutputWordsStatisticsOutputWordFirstInvalidCharacterLocalPosition)
                    RemoveMistypedWord();
                return true;
            }
            return false;
        }

        private InputOutputWordsStatistics GetTopInputOutputWordsStatisticsListElement() { return inputOutputWordsStatisticsList.Peek(); }

        public void AddNonWhitespaceCharacter(char inputCharacter)
        {
            if (!IsTextInputFinished && 
                (!RegularExpressionsService.HasInputStringPatternStringFormat(whitespaceCharacterTransferCharacterOutputTestTextReplacementStringTextEndStringTextStartStringPattern, 
                                                                                                      inputCharacter.ToString())))
            {
                InputOutputWordsStatistics lastInputOutputWordsStatistics = GetTopInputOutputWordsStatisticsListElement();
                lastInputOutputWordsStatistics.InputWordCharactersCount++;
                if (!isWhitespaceInputEnabled)
                    isWhitespaceInputEnabled = true;
                if (lastInputOutputWordsStatistics.OutputWordFirstInvalidCharacterLocalPosition == -1)
                {
                    int outputTestTextLength = OutputTestText.Length;
                    if ((TextInputPosition < outputTestTextLength) && (inputCharacter == OutputTestText[TextInputPosition]))
                    {
                        if ((++TextInputPosition == (outputTestTextLength - 1)) && (MistypedWordsCount == 0))
                            IsTextInputFinished = true;
                    }
                    else
                        AddMistypedWord();
                }
                InputCharactersCount++;
            }
        }

        public void ResetTestTextModelInputStatistics()
        {
            isWhitespaceInputEnabled = false;
            IsTextInputFinished = !isTextInputPossible;
            TextInputPosition = 1;
            InputCharactersCount = MistypedWordsCount = 0;
            inputOutputWordsStatisticsList.Clear();
            ResetTestTextModelWordCharactersStatistics();
        }

        public bool TryAddWhitespaceCharacter()
        {
            bool tryAddWhitespaceCharacter = false;
            if (!IsTextInputFinished && isWhitespaceInputEnabled)
            {
                int potentiallyNextTextInputPosition = RegularExpressionsService.FindInputStringFirstPatternStringStartIndex(whitespaceCharacterTransferCharactersPattern +
                                                                                                                             whitespaceCharacterTransferCharactersOppositePattern, OutputTestText,
                                                                                                                             TextInputPosition);
                if (potentiallyNextTextInputPosition != -1)
                {
                    isWhitespaceInputEnabled = false;
                    tryAddWhitespaceCharacter = true;
                    if ((GetTopInputOutputWordsStatisticsListElement().OutputWordFirstInvalidCharacterLocalPosition == -1) &&
                        !RegularExpressionsService.HasInputStringPatternStringFormat(whitespaceOutputTestTextReplacementStringTextEndStringPattern, OutputTestText[TextInputPosition].ToString()))
                        AddMistypedWord();
                    TextInputPosition = potentiallyNextTextInputPosition + 1;
                    ResetTestTextModelWordCharactersStatistics();
                    InputCharactersCount++;
                }
            }
            return tryAddWhitespaceCharacter;
        }

        public bool TryRemoveCharacter()
        {
            bool tryRemoveCharacter = true;
            if (isWhitespaceInputEnabled)
            {
                InputOutputWordsStatistics lastInputOutputWordsStatistics = GetTopInputOutputWordsStatisticsListElement();
                if ((--lastInputOutputWordsStatistics.InputWordCharactersCount == 0) | (!CheckPossibilityOfMystypedWordRemoving() &&
                    (--TextInputPosition == lastInputOutputWordsStatistics.OutputWordFirstCharacterPosition)))
                    isWhitespaceInputEnabled = false;
                InputCharactersCount--;
            }
            else
            {
                if (TextInputPosition > 1)
                {
                    inputOutputWordsStatisticsList.Pop();
                    InputOutputWordsStatistics lastInputOutputWordsStatistics = GetTopInputOutputWordsStatisticsListElement();
                    int lastInputOutputWordsStatisticsOutputWordFirstCharacterPosition = lastInputOutputWordsStatistics.OutputWordFirstCharacterPosition,
                        lastInputOutputWordsStatisticsOutputWordFirstInvalidCharacterLocalPosition = lastInputOutputWordsStatistics.OutputWordFirstInvalidCharacterLocalPosition;
                    TextInputPosition = (lastInputOutputWordsStatisticsOutputWordFirstInvalidCharacterLocalPosition == -1) ? lastInputOutputWordsStatisticsOutputWordFirstCharacterPosition +
                                                                                                                             lastInputOutputWordsStatistics.OutputWordCharactersCount :
                                                                                                                             lastInputOutputWordsStatisticsOutputWordFirstCharacterPosition +
                                                                                                                             lastInputOutputWordsStatisticsOutputWordFirstInvalidCharacterLocalPosition;
                    CheckPossibilityOfMystypedWordRemoving();
                    isWhitespaceInputEnabled = true;
                    InputCharactersCount--;
                }
                else tryRemoveCharacter = false;
            }
            return tryRemoveCharacter;
        }

        public int GetOutputTestTextWordsCount()
        {
            return RegularExpressionsService.GetInputStringPatternStringMathes(oneOrNothingWhitespaceCharacterTransferCharacterOutputTestTextReplacementStringTextEndStringTextStartStringPattern + 
                                                                               "([^(" + whitespaceCharacterTransferCharacterOutputTestTextReplacementStringTextEndStringTextStartStringPattern + 
                                                                               ")]+)" + 
                                                                               oneOrNothingWhitespaceCharacterTransferCharacterOutputTestTextReplacementStringTextEndStringTextStartStringPattern, 
                                                                               OutputTestText).Count;
        }

        private class InputOutputWordsStatistics
        {
            public InputOutputWordsStatistics(int OutputWordCharactersCount, int OutputWordFirstCharacterPosition)
            {
                InputWordCharactersCount = 0;
                this.OutputWordCharactersCount = OutputWordCharactersCount;
                this.OutputWordFirstCharacterPosition = OutputWordFirstCharacterPosition;
                OutputWordFirstInvalidCharacterLocalPosition = -1;
            }

            public int InputWordCharactersCount { get; set; }

            public int OutputWordCharactersCount { get; private set; }
            
            public int OutputWordFirstCharacterPosition { get; private set; }

            public int OutputWordFirstInvalidCharacterLocalPosition { get; set; }
        }
    }
}
