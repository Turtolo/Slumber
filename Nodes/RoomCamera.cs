
#nullable disable

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Opal.Params;
using Opal.Tools;
using Opal.Util;

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
    private int dir;

    [Export]
    public Node2D TargetNode { get; set; }

    [Export]
    public Action TransitionStarted { get; set; }
    [Export]
    public Action TransitionEnded { get; set; }

    public RoomCamera() { }

    public override void EnterTree()
    {
      base.EnterTree();

      if (TargetNode is KinematicBody2D)
      {
        TransitionStarted += LockBody;
        TransitionEnded += UnlockBody;
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
            ShiftRoom(-1);
            break;
          case CameraSide.Right:
            ShiftRoom(1);
            break;
        }
      }

      entered = side != CameraSide.None;
    }

    public override void Submit(Canvas2D canvas)
    {
      base.Submit(canvas);
    }

    private void ShiftRoom(int dir)
    {
      dir = dir;

      var camera = GetWorldViewRectangle();

      Vector2 targetPos = new Vector2(Transform.Global.Position.X + camera.Width * dir, Transform.Global.Position.Y);

      TransitionStarted?.Invoke();

      var cameraXTween = Core.Token.CreateTween(t => Position = t, Transform.Global.Position, targetPos, 0.5f, Vector2.Lerp, EasingFunctions.Linear);

      cameraXTween.SetCallbackAction
      (
          () => TransitionEnded?.Invoke()
      );

    }

    private void LockBody()
    {
      if (TargetNode is Player player)
      {
        player.Position += new Vector2(10 * dir, 0);
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
