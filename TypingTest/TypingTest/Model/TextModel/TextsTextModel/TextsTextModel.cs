using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypingTest.Service;

namespace TypingTest.Model.TextModel.TextsTextModel
{
    class TextsTextModel : MainTextModel
    {
        public static readonly long textLengthDefaultValue = 0;
        
        private long textLength;

        public TextsTextModel(string textName, string textDirectoryName, long textLength, DateTime textCreationTime) : base(textName, textDirectoryName, textCreationTime) { TextLength = textLength; }
                
        public long TextLength
        {
            get { return textLength; }

            set { textLength = (value > 0) ? value : textLengthDefaultValue; }
        }
    }
}
