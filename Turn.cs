using System.Collections.Generic;
using System.Drawing;

namespace YINSH
{
    class Turn
    {
        private static Turn Instance = null;

        public static Turn GetInstance
        {
            get
            {
                if (Instance == null)
                {
                    Instance = new Turn();
                }
                return Instance;
            }
        }

        #region 변수
        /// <summary>
        /// YINSH Player White & Black
        /// </summary>
        public List<Color> Player = new List<Color>();
        public int[] Each = new int[0];
        int Count;
        public int User;
        public bool Next;
        public bool Check;
        #endregion

        #region 함수
        public void System()
        {
            if (Check == true)
            {
                if (Next == true)
                {
                    Each[User]++;
                    Count = 0;
                    for (var count = 0; count < Each.Length; count++)
                    {
                        Count += Each[count];
                    }
                    Next = false;
                }
                User++;
                User = (User >= Player.Count) ? 0 : User;
                //label1.Text = "Turn " + Count + " " + label1.Text;
                Check = false;
            }
        }

        public void Setting()
        {
            Player.Clear();
            Next = false;
            Check = false;
            Player.Add(Color.White);
            Player.Add(Color.Black);
            Each = new int[Player.Count];
            User = 0;
        }
        #endregion
    }
}
