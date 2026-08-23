
#nullable disable

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Opal.Params;
using Opal.Tools;
using Opal.Tools;

namespace Slumber
{
  public enum CameraSide
  {
    None,
    Left,
    Right,
    Top,
    Bottom
  }

  public class RoomCamera : Camera2D
  {
    private bool entered;
    private Vector2 dir;

    [Export]
    public Node2D TargetNode { get; set; }

    [Export]
    public Action ScreenEffectsStarted { get; set; }
    [Export]
    public Action ScreenEffectsEnded { get; set; }

    public RoomCamera() { }

    public override void EnterTree()
    {
      base.EnterTree();

      if (TargetNode is KinematicBody2D)
      {
        ScreenEffectsStarted += LockBody;
        ScreenEffectsEnded += UnlockBody;
      }
    }

    public override void ExitTree()
    {
      base.ExitTree();
    }

    public override void Process(float delta)
    {
      base.Process(delta);

      if (TargetNode == null || TargetNode.Get<CollisionShape2D>() == null)
        return;

      var targetShape = TargetNode.Get<CollisionShape2D>();
      var shape = targetShape.Shape;

      var pos = TargetNode.Transform.Global.Position;

      var camera = GetWorldViewRectangle();

      CameraSide side = CameraSide.None;

      if (pos.X + shape.Size.Width > camera.Right)
        side = CameraSide.Right;
      else if (pos.X < camera.Left)
        side = CameraSide.Left;
      else if (pos.Y < camera.Top)
        side = CameraSide.Top;
      else if (pos.Y + shape.Size.Height > camera.Bottom)
        side = CameraSide.Bottom;

      if (!entered)
      {
        switch (side)
        {
          case CameraSide.Left:
            Shift(new Vector2(-1, 0));
            break;
          case CameraSide.Right:
            Shift(new Vector2(1, 0));
            break;
          case CameraSide.Top:
            Shift(new Vector2(0, -1));
            break;
          case CameraSide.Bottom:
            Shift(new Vector2(0, 1));
            break;
        }
      }

      entered = side != CameraSide.None;
    }

    public override void Submit(Canvas2D canvas)
    {
      base.Submit(canvas);
    }

    private void Shift(Vector2 dir)
    {
      this.dir = dir;

      var camera = GetWorldViewRectangle();

      Vector2 targetPos = new Vector2(Transform.Global.Position.X + camera.Width * dir.X, Transform.Global.Position.Y + camera.Height * dir.Y);

      ScreenEffectsStarted?.Invoke();

      var cameraTween = Core.Token.CreateTween(t => Position = t, Transform.Global.Position, targetPos, 0.5f, Vector2.Lerp, EasingFunctions.Linear);

      cameraTween.SetCallbackAction
      (
        () => ScreenEffectsEnded?.Invoke()
      );
    }

    private void ShiftRoom(int dir)
    {
      //this.dir = dir;
      var camera = GetWorldViewRectangle();

      Vector2 targetPos = new Vector2(Transform.Global.Position.X + camera.Width * dir, Transform.Global.Position.Y);

      ScreenEffectsStarted?.Invoke();

      var cameraXTween = Core.Token.CreateTween(t => Position = t, Transform.Global.Position, targetPos, 0.5f, Vector2.Lerp, EasingFunctions.Linear);

      cameraXTween.SetCallbackAction
      (
        () => ScreenEffectsEnded?.Invoke()
      );

    }

    private void LockBody()
    {
      if (TargetNode is Player player)
      {
        player.Position += new Vector2(10, -20) * dir;
        player.Velocity.X = 0;
        player.Properties.AllowControl = false;
      }
    }

    private void UnlockBody()
    {
      if (TargetNode is Player player)
      {
        player.Properties.AllowControl = true;
      }
    }

  }
}
