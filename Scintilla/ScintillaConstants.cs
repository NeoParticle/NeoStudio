using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoStudio
{
    internal static class ScintillaConstants
    {
        internal const string NEW_DOCUMENT_TEXT = "Untitled";
        internal const int LINE_NUMBERS_MARGIN_WIDTH = 30;

        internal const int BACK_COLOR = 0x2A211C;
        internal const int FORE_COLOR = 0xB7B7B7;

        //Margin and Marker Order
        internal const int NUMBER_MARGIN = 1;

        internal const int BREAKPOINT_MARGIN = 2;
        internal const int BREAKPOINT_MARKER = 2;

        internal const int FOLDING_MARGIN = 3;

        internal const int BREAKPOINT_BG = 4;
        internal const int STEP_BG = 5;

        //Indicators
        internal const int BREAKPOINT_INDICATOR = 8;
        internal const int STEP_INDICATOR = 9;
        internal const int ERROR_INDICATOR = 10;

        //set this true to show circular buttons for code folding (the [+] and [-] buttons on the margin)
        internal const bool CODEFOLDING_CIRCULAR = false;
    }
}
