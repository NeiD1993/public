using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypingTest.Model.TimerModel;

namespace TypingTest.Model
{
    class ResultsModel
    {
        private static readonly int inputCharactersCountMistypedWordsCountDefaultValue = 0;

        private static readonly int wordsCountDefaultValue = 1;

        private static readonly TimeSpan elapsedTimeDefaultValue = BaseTimerModel.minimalTimeLowerInitializationBorder;

        private int inputCharactersCount;

        private int mistypedWordsCount;

        private int wordsCount;

        private TimeSpan elapsedTime;

        public ResultsModel(int inputCharactersCount, int mistypedWordsCount, int wordsCount, TimeSpan elapsedTime)
        {
            InputCharactersCount = inputCharactersCount;
            WordsCount = wordsCount;
            MistypedWordsCount = mistypedWordsCount;
            ElapsedTime = elapsedTime;
        }

        public int InputCharactersCount 
        { 
            get { return inputCharactersCount; }

            private set { inputCharactersCount = (value > 0) ? value : inputCharactersCountMistypedWordsCountDefaultValue; }
        }
        
        public int MistypedWordsCount 
        {
            get { return mistypedWordsCount; }

            private set { mistypedWordsCount = ((value > 0) && (value <= wordsCount)) ? value : inputCharactersCountMistypedWordsCountDefaultValue; }
        }

        public int WordsCount
        {
            get { return wordsCount; }

            private set { wordsCount = (value > 1) ? value : wordsCountDefaultValue; }
        }
        
        public TimeSpan ElapsedTime 
        {
            get { return elapsedTime; }

            private set { elapsedTime = (value >= elapsedTimeDefaultValue) ? value : elapsedTimeDefaultValue; }
        }
    }
}
