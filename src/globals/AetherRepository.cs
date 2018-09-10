using System;
using System.Collections.Generic;

namespace ProjectAether.src
{
    public class AetherRepository
    {
        private Dictionary<int, string> m_faces = new Dictionary<int, string>();

        public AetherRepository()
        {
            addFaces();
        }

        private void addFaces()
        {
            m_faces.Add(0, "(✿◠‿◠)   ");
            m_faces.Add(1, "(◡‿◡✿)   ");
            m_faces.Add(2, "(◕‿◕✿)   ");
            m_faces.Add(3, "(╥﹏╥)    ");
            m_faces.Add(4, "(⊙_◎)    ");
            m_faces.Add(5, "(ಠ_ಠ)    ");
            m_faces.Add(6, "(° ͜ ʖ°)   ");
            m_faces.Add(7, "ʕ•ᴥ•ʔ     ");
            m_faces.Add(8, "༼ つ◕_◕ ༽つ");
            m_faces.Add(9, "(⌐■_■)    ");
            m_faces.Add(10, "(◉_◉)    ");
            m_faces.Add(11, "(*◔_◔)   ");
            m_faces.Add(12, "(︶︹︶) ");
            m_faces.Add(13, "(҂◡_◡)ᕤ ");
            m_faces.Add(14, "(❍ᴥ❍ʋ) ");
            m_faces.Add(15, "┌(◉ ͜ʖ◉)つ");
            m_faces.Add(16, "(⌐⊙_⊙)   ");
            m_faces.Add(17, "(っˆڡˆς) ");
            m_faces.Add(18, "(ง'̀-'́)ง  ");
            m_faces.Add(19, "(ᵔᴥᵔ)    ");
            m_faces.Add(20, "(~˘▾˘)~  ");
            m_faces.Add(21, "ᕙ(⇀‸↼‶)ᕗ ");
            m_faces.Add(22, "(◔̯◔)    ");
            m_faces.Add(23, "(°ω°)    ");
            m_faces.Add(24, "ლ(´ڡ`ლ)");
            m_faces.Add(25, "ᶘ ᵒᴥᵒᶅ   ");
            m_faces.Add(26, "(•‿•)   ");
            m_faces.Add(27, "(ㆆ _ ㆆ)");
            m_faces.Add(28, "(⌐⊙_⊙)   ");
        }

        public string getFace(int key)
        {
            if (m_faces.ContainsKey(key))
            {
                return m_faces[key];
            }
            else
            {
                return m_faces[0];
            }
        }
        
        public string[] getRandomFaces(int length)
        {
            if (length > m_faces.Count) { return new string[0]; }
            Random rand = new Random();
            string[] faces = new string[length];
            bool[] set = new bool[m_faces.Count];
            for (int i = 0; i < faces.Length; i++)
            {
                int index = rand.Next(m_faces.Count);
                // get a unused face
                while (set[index] == true) { index = rand.Next(m_faces.Count); }
                set[index] = true;
                faces[i] = m_faces[index];
            }
            return faces;
        }
    }
}
