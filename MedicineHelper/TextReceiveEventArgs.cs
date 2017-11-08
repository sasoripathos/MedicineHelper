using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicineHelper
{
    public class TextReceiveEventArgs : EventArgs
    {
        public List<String> textList { get; set; }
    }
}
