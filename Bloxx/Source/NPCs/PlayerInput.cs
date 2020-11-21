using Microsoft.Xna.Framework.Input;
using MonoExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloxx.Source.NPCs
{
    public abstract class PlayerInput
    {
        public abstract int Horizontal();
        public abstract bool Jump();
        public abstract bool JumpDown();
    }

    public class PlayerKeys : PlayerInput
    {
        public PlayerKeys(Keys _right, Keys _left, Keys _jump, Keys _altJump)
        {
            right = _right;
            left = _left;
            jump = _jump;
            altJump = _altJump;
        }

        readonly Keys right, left, jump, altJump;

        public override int Horizontal()
        {
            return (Input.KeyDown(right) ? 1 : 0) -
                   (Input.KeyDown(left) ? 1 : 0);
        }

        public override bool Jump()
        {
            return Input.KeyDown(jump) || Input.KeyDown(altJump);
        }

        public override bool JumpDown()
        {
            return Input.KeyPressed(jump) || Input.KeyPressed(altJump);
        }
    }

    public class OtherInput : PlayerInput
    {
        static bool lastJumping;
        static bool jumping;
        public static int hor;

        public static void PushJump(bool newJump)
        {
            //lastJumping = jumping;
            jumping = newJump;
        }

        public override int Horizontal()
        {
            return hor;
        }

        public override bool Jump()
        {
            lastJumping = jumping;
            return jumping;
        }

        public override bool JumpDown()
        {
            return !lastJumping && jumping;
        }
    }
}
