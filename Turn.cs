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
        /// 턴 총 합계
        /// 현재 플레이어
        /// 턴 확인
        /// 컴포넌트 넘기기
        /// 준비&시작 확인
        public List<Color> Player = new List<Color>();
        public int Count;
        public int User;
        public bool Check;
        public bool Next;
        public bool Ready;
        #endregion

        #region 함수
        public void Setting()
        {
            Player.Clear();
            Player.Add(Color.White);
            Player.Add(Color.Black);

            Count = 0;
            User = 0;
            Check = false;
            Next = false;
            Ready = false;
        }

        public void System()
        {
            if (Check == true)
            {
                if (Next == true)
                {
                    Count++;
                    Next = false;
                }
                User++;
                User = (User >= Player.Count) ? 0 : User;
                Check = false;
            }
        }
        #endregion
    }
}
