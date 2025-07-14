namespace DiaLuna;

using Godot;

public interface IInputComponent
{
	public bool MainAttackPressed { get; }
	public event InputComponent.ButtonPressed? MainAttack;

	public bool SecondaryUsePressed { get; }
	public event InputComponent.ButtonPressed? SecondaryUse;

	public bool SprintPressed { get; }
	public event InputComponent.ButtonPressed? Sprint;

	public bool OpenInventoryPressed { get; }
	public event InputComponent.ButtonPressed? OpenInventory;

	public Vector2 InputDirection { get; }
	public bool IsMoving { get; }

	public Vector2 MouseGlobalPosition { get; }
}

[GlobalClass]
public partial class InputComponent : Node, IInputComponent
{
	public delegate void ButtonPressed(bool pressed);

	public bool MainAttackPressed => _mainAttackPressed;
	private bool _mainAttackPressed;
	public event ButtonPressed? MainAttack;

	public bool SecondaryUsePressed => _secondaryUsePressed;
	private bool _secondaryUsePressed;
	public event ButtonPressed? SecondaryUse;

	public bool SprintPressed => _sprintPressed;
	private bool _sprintPressed;
	public event ButtonPressed? Sprint;

	public bool OpenInventoryPressed => _openInventoryPressed;
	private bool _openInventoryPressed;
	public event ButtonPressed? OpenInventory;

	public Vector2 InputDirection => _inputDirection;
	private Vector2 _inputDirection;

	public bool IsMoving => !_inputDirection.IsZeroApprox();

	public Vector2 MouseGlobalPosition => _mousePosition;

	private Vector2 _mouseDirection;
	private Vector2 _mousePosition;

	public override void _PhysicsProcess(double delta)
	{
		_inputDirection = Input.GetVector(
			InputMap.MoveLeft,
			InputMap.MoveRight,
			InputMap.MoveUp,
			InputMap.MoveDown
		);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion evt)
		{
			_mousePosition = evt.GlobalPosition;
		}

		HandleButtonAction(
			InputMap.MainAttack,
			ref _mainAttackPressed,
			MainAttack,
			@event
		);
		HandleButtonAction(
			InputMap.SecondaryUse,
			ref _secondaryUsePressed,
			SecondaryUse,
			@event
		);
	}

	public override void _UnhandledKeyInput(InputEvent @event)
	{
		HandleButtonAction(InputMap.Sprint, ref _sprintPressed, Sprint, @event);
		HandleButtonAction(
			InputMap.OpenInventory,
			ref _openInventoryPressed,
			OpenInventory,
			@event
		);
	}

	private static void HandleButtonAction(
		StringName name,
		ref bool isPressed,
		ButtonPressed? eventHandler,
		InputEvent @event
	)
	{
		// this should be the fewest native calls
		// if is pressed event, cant be released event
		// by default ignores echo key presses
		if (@event.IsActionPressed(name))
		{
			isPressed = true;
			eventHandler?.Invoke(true);
		}
		else if (@event.IsActionReleased(name))
		{
			isPressed = false;
			eventHandler?.Invoke(false);
		}
	}
}
