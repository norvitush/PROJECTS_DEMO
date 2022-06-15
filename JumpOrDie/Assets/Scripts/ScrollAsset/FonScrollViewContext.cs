using System;

namespace VOrb
{
    public class FonScrollViewContext
    {
        int selectedIndex = -1;

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if (value == selectedIndex)
                {
                    return;
                }

                selectedIndex = value;

                OnSelectedIndexChanged?.Invoke(selectedIndex);
            }
        }

        public Action<FonCellView> OnPressedCell;
        public Action<int> OnSelectedIndexChanged;
    }
}
