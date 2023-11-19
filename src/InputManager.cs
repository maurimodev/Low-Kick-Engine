using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public enum ControlScheme
{
    UP_L,
    DOWN_L,
    RIGHT_L,
    LEFT_L,
    UP_R,
    DOWN_R,
    LEFT_R,
    RIGHT_R,
    A,
    B,
    LEFT_TRIGGER,
    RIGHT_TRIGGER,
    START,
    SELECT,
    DEBUG_1,
    DEBUG_2,
    DEBUG_3
}

public enum Axis
{
    LEFT_STICK,
    RIGHT_STICK
}
public static class InputManager
{
    private static KeyboardState currentKeyState;
    private static KeyboardState previousKeyState;

    private static GamePadState currentGamePadState;
    private static GamePadState previousGamePadState;

    private static bool UseGamepad = false;
    private static bool IsGamepadConnected = false;
    private static PlayerIndex GamepadPort = PlayerIndex.One;

    private static Dictionary<ControlScheme, Keybind> keybinds = new Dictionary<ControlScheme, Keybind>();

    public static void Initialize()
    {
        // MOVEMENT

        Bind(ControlScheme.UP_L, new Keybind(Keys.W, Buttons.LeftThumbstickUp));
        Bind(ControlScheme.DOWN_L, new Keybind(Keys.S, Buttons.LeftThumbstickDown));
        Bind(ControlScheme.LEFT_L, new Keybind(Keys.A, Buttons.LeftThumbstickLeft));
        Bind(ControlScheme.RIGHT_L, new Keybind(Keys.D, Buttons.LeftThumbstickRight));

        // AIM

        Bind(ControlScheme.UP_L, new Keybind(Keys.Up, Buttons.RightThumbstickUp));
        Bind(ControlScheme.DOWN_L, new Keybind(Keys.Down, Buttons.RightThumbstickDown));
        Bind(ControlScheme.LEFT_L, new Keybind(Keys.Left, Buttons.RightThumbstickLeft));
        Bind(ControlScheme.RIGHT_L, new Keybind(Keys.Right, Buttons.RightThumbstickRight));

        // ACTIONS

        Bind(ControlScheme.A, new Keybind(Keys.J, Buttons.A));
        Bind(ControlScheme.B, new Keybind(Keys.K, Buttons.B));
        Bind(ControlScheme.LEFT_TRIGGER, new Keybind(Keys.Q, Buttons.LeftTrigger));
        Bind(ControlScheme.RIGHT_TRIGGER, new Keybind(Keys.R, Buttons.RightTrigger));

        // MISC

        Bind(ControlScheme.START, new Keybind(Keys.Escape, Buttons.Start));
        Bind(ControlScheme.SELECT, new Keybind(Keys.Enter, Buttons.Back));

        // DEBUG

        Bind(ControlScheme.DEBUG_1, new Keybind(Keys.F1));
        Bind(ControlScheme.DEBUG_2, new Keybind(Keys.F2));
        Bind(ControlScheme.DEBUG_3, new Keybind(Keys.F3));

    }

    private static void Bind(ControlScheme control, Keybind bind)
    {
        if (keybinds.ContainsKey(control))
        {
            Console.Error.WriteLine("There already is a key named \"" + control.ToString() + "\" in the Control Scheme Dictionary.");
            return;
        }

        keybinds.Add(control, bind);
    }

    public static void Update(GameTime time)
    {
        previousKeyState = currentKeyState;
        currentGamePadState = previousGamePadState;

        currentKeyState = Keyboard.GetState();
        currentGamePadState = GamePad.GetState(GamepadPort);

        GetCurrentInput();
    }

    public static Vector2 GetAxis(Axis axis)
    {
        var vec = Vector2.Zero;
        switch (axis)
        {
            case Axis.LEFT_STICK:
                if (UseGamepad)
                {
                    vec = GamePad.GetState(GamepadPort).ThumbSticks.Left;
                }
                else
                {
                    vec.X = GetKeyboardHorizontalAxis(axis);
                    vec.Y = GetKeyboardVerticalAxis(axis);
                }
                break;
            case Axis.RIGHT_STICK:
                if (UseGamepad)
                {
                    vec = GamePad.GetState(GamepadPort).ThumbSticks.Right;
                }
                else
                {
                    vec.X = GetKeyboardHorizontalAxis(axis);
                    vec.Y = GetKeyboardVerticalAxis(axis);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
        }
        return vec;
    }

    private static float GetKeyboardHorizontalAxis(Axis a)
    {
        float horizontal = 0;

        if (a == Axis.LEFT_STICK)
        {
            if (GetInput(ControlScheme.LEFT_L))
            {
                horizontal = -1f;
            }
            else if (GetInput(ControlScheme.RIGHT_L))
            {
                horizontal = 1f;
            }
        }
        else
        {
            if (GetInput(ControlScheme.LEFT_R))
            {
                horizontal = -1f;
            }
            else if (GetInput(ControlScheme.RIGHT_R))
            {
                horizontal = 1f;
            }
        }
        return horizontal;
    }
    private static float GetKeyboardVerticalAxis(Axis a)
    {
        float vertical = 0;

        if (a == Axis.LEFT_STICK)
        {
            if (GetInput(ControlScheme.DOWN_L))
            {
                vertical = 1f;
            }
            else if (GetInput(ControlScheme.UP_L))
            {
                vertical = -1f;
            }
        }
        else
        {
            if (GetInput(ControlScheme.DOWN_R))
            {
                vertical = 1f;
            }
            else if (GetInput(ControlScheme.UP_R))
            {
                vertical = -1f;
            }
        }
        return vertical;
    }
    public static bool GetInputDown(ControlScheme control)
    {
        if (keybinds.ContainsKey(control))
        {
            return IsKeybindJustPressed(keybinds[control]);
        }

        return false;
    }

    public static bool GetInput(ControlScheme control)
    {
        if (keybinds.ContainsKey(control))
        {
            return IsKeybindPressed(keybinds[control]);
        }

        return false;
    }

    public static bool GetInputUp(ControlScheme control)
    {
        if (keybinds.ContainsKey(control))
        {
            return IsKeybindReleased(keybinds[control]);
        }

        return false;
    }

    private static bool IsKeybindPressed(Keybind key)
    {
        var isPressed = false;

        if (IsKeybindJustPressed(key))
        {
            // This is kind of hacky- basically I'm making sure that JustPressed always gets called before IsPressed, for order purposes.
            // Since this doesn't call "GetInputDown" and that's the only method that other scripts can access, it should be okay.
            return false;
        }

        if (UseGamepad && !key.PreferKeyboard)
        {
            isPressed = currentGamePadState.IsButtonDown(key.gamepadBinding);
        }
        else
        {
            isPressed = currentKeyState.IsKeyDown(key.keyboardBinding);
        }

        return isPressed;
    }

    private static bool IsKeybindJustPressed(Keybind key)
    {
        var isJustPressed = false;

        if (UseGamepad && !key.PreferKeyboard)
        {
            isJustPressed = currentGamePadState.IsButtonDown(key.gamepadBinding) && previousGamePadState.IsButtonUp(key.gamepadBinding);
        }
        else
        {
            isJustPressed = currentKeyState.IsKeyDown(key.keyboardBinding) && previousKeyState.IsKeyUp(key.keyboardBinding);
        }

        return isJustPressed;
    }

    private static bool IsKeybindReleased(Keybind key)
    {
        var isReleased = false;

        if (UseGamepad && !key.PreferKeyboard)
        {
            isReleased = currentGamePadState.IsButtonUp(key.gamepadBinding) && previousGamePadState.IsButtonDown(key.gamepadBinding);
        }
        else
        {
            isReleased = currentKeyState.IsKeyUp(key.keyboardBinding) && previousKeyState.IsKeyDown(key.keyboardBinding);
        }

        return isReleased;
    }

    #region Debug

    private static void GetCurrentInput()
    {
        foreach (var c in keybinds)
        {
            bool v1 = GetInput(c.Key);
            bool v2 = GetInputDown(c.Key);
            bool v3 = GetInputUp(c.Key);
        }
    }
    #endregion

}

public struct Keybind
{
    public Keys keyboardBinding;
    public Buttons gamepadBinding;

    public bool PreferKeyboard;
    public bool GamepadOnly;
    public Keybind(Keys key)
    {
        keyboardBinding = key;
        gamepadBinding = new Buttons();

        GamepadOnly = false;
        PreferKeyboard = true;
    }

    public Keybind(Keys key, Buttons gamepad)
    {
        keyboardBinding = key;
        gamepadBinding = gamepad;

        GamepadOnly = false;
        PreferKeyboard = true;
    }

    public Keybind(Buttons gamepad)
    {
        gamepadBinding = gamepad;
        keyboardBinding = new Keys();

        GamepadOnly = true;
        PreferKeyboard = false;
    }
}
