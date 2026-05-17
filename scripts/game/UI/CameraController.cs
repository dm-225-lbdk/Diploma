using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CameraController : Camera2D
{
    // Сигнал для передачі інформації дебагеру
    [Signal]
    public delegate void ZoomDebugEventHandler(string[] keys, string[] values);

    private float dragSpeed = 1.25f;
    private bool dragging = false;
    private Vector2 lastMousePos;
    private Dictionary<string, Func<string>> debugVarList = new();

    public override void _Ready()
    {
        // Інформація для дебагера
        debugVarList = new()
        {
            {"CurentMousePos", () => GetGlobalMousePosition().Floor().ToString()},
            {"LastMousePos", () => lastMousePos.Floor().ToString()},
            {"ZoomLevel", () => (Zoom / 7.0f * 100.0f).Floor() + "%"},
            {"Drag", () => dragging.ToString()}
        };
    }

    public override void _Process(double delta)
    {
        Position = Position.Round();

        // Початок перетягування
        if (Input.IsActionJustPressed("drag"))
        {
            dragging = true;
            lastMousePos = GetViewport().GetMousePosition();
        }

        // Кінець перетягування
        if (Input.IsActionJustReleased("drag")) dragging = false;

        // Перетягування
        if (dragging)
        {
            Vector2 mousePos = GetViewport().GetMousePosition();
            Vector2 deltaPos = mousePos - lastMousePos;
            Position -= deltaPos * dragSpeed / Zoom;
            lastMousePos = mousePos;
        }

        // Зум
        if (Input.IsActionJustPressed("zoom_in") && Zoom.Y < 7.0f) Zoom *= 1.1f;
        if (Input.IsActionJustPressed("zoom_out") && Zoom.Y > 0.75f) Zoom *= 0.9f;

        // Еміт (відправка) сигналу дебагеру
        EmitSignal(SignalName.ZoomDebug,
            debugVarList.Keys.ToArray(),
            debugVarList.Values.Select(f => f()).ToArray());
    }
}
