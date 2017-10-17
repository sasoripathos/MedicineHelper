using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArthitisAndMeApp
{
    interface ISpeech
    {
        String voice2text(Form1 form);
        void text2voice(string text);
    }
}
