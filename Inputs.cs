using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivuco
{
    interface Inputs
    {
        void GetInputs();

        float MoveX();
        float MoveY();

        bool JumpHolding();
        bool JumpClicked();
        bool JumpReleased();

        bool FirstSkillHolding();
        bool FirstSkillClicked();
        bool FirstSkillReleased();

        bool SecondSkillHolding();
        bool SecondSkillClicked();
        bool SecondSkillReleased();

        bool ThirdSkillHolding();
        bool ThirdSkillClicked();
        bool ThirdSkillReleased();

        bool Aim();
        void RotateAim(Aim aim, Player.Side side);
        void SetDefaultAim(Aim aim); 

        bool PauseGame();
        int SelectX();
        int SelectY();
        bool Back();
    }
}
