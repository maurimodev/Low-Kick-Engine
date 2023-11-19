using System;
using Microsoft.Xna.Framework;

namespace KungFuPlatform.Scripts.Math;

public enum Edge
{
    TOP,
    BOTTOM,
    LEFT,
    RIGHT
}
public struct RectangleF : IEquatable<RectangleF>
{
    #region Statics
    private static RectangleF emptyRectangle = new RectangleF();
    public static RectangleF Empty => emptyRectangle;
    public static RectangleF MaxRect => new RectangleF();
    #endregion
    #region Properties
    public float X;
    public float Y;
    public float Width;
    public float Height;
    public float Left => X;
    public float Right => (X + Width);
    public float Top => Y;
    public float Bottom => (Y + Height);

    public Vector2 Max => new Vector2(Right, Bottom);
    public bool IsEmpty => ((((Width == 0) && (Height == 0)) && (X == 0)) && (Y == 0));
    public Vector2 Location
    {
        get => new Vector2(X, Y);
        set
        {
            X = value.X;
            Y = value.Y;
        }
    }
    public Vector2 Size
    {
        get => new Vector2(Width, Height);
        set
        {
            Width = value.X;
            Height = value.Y;
        }
    }
    public Vector2 Center => new Vector2(X + (Width / 2), Y + (Height / 2));

    #endregion
    #region Constructors
    public RectangleF(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public RectangleF(Vector2 location, Vector2 size)
    {
        X = location.X;
        Y = location.Y;
        Width = size.X;
        Height = size.Y;
    }

    public static RectangleF FromMinMax(Vector2 min, Vector2 max)
    {
        return new RectangleF(min.X, min.Y, max.X - min.X, max.Y - min.Y);
    }

    public static RectangleF FromMinMax(float minX, float minY, float maxX, float maxY)
    {
        return new RectangleF(minX, minY, maxX - minX, maxY - minY);
    }

    public static RectangleF RectEncompassingPoints(Vector2[] points)
    {
        var minX = float.PositiveInfinity;
        var minY = float.PositiveInfinity;
        var maxX = float.NegativeInfinity;
        var maxY = float.NegativeInfinity;
        
        foreach (var point in points)
        {
            if (point.X < minX)
                minX = point.X;
            if (point.X > maxX)
                maxX = point.X;
            if (point.Y < minY)
                minY = point.Y;
            if (point.Y > maxY)
                maxY = point.Y;
        }
        
        return FromMinMax(minX, minY, maxX, maxY);
    }
    #endregion
    
    #region Public Methods

    public float GetSide(Edge edge) => edge switch
    {
        Edge.TOP => Top,
        Edge.BOTTOM => Bottom,
        Edge.LEFT => Left,
        Edge.RIGHT => Right,
        _ => throw new ArgumentOutOfRangeException(nameof(edge), edge, null)
    };
    
    public bool Contains(float x, float y)
    {
        return ((((X <= x) && (x < (X + Width))) && (Y <= y)) && (y < (Y + Height)));
    }

    public bool Contains(int x, int y)
    {
        return ((((X <= x) && (x < (X + Width))) && (Y <= y)) && (y < (Y + Height)));
    }
    
    public bool Contains(Vector2 value)
    {
        return ((((X <= value.X) && (value.X < (X + Width))) && (Y <= value.Y)) && (value.Y < (Y + Height)));
    }
    
    public bool Contains(RectangleF value)
    {
        return ((((X <= value.X) && ((value.X + value.Width) <= (X + Width))) && (Y <= value.Y)) && ((value.Y + value.Height) <= (Y + Height)));
    }
    
    public void Contains(ref Vector2 value, out bool result)
    {
        result = (((X <= value.X) && (value.X < (X + Width))) && (Y <= value.Y)) && (value.Y < (Y + Height));
    }
    
    public void Contains(ref RectangleF value, out bool result)
    {
        result = (((X <= value.X) && ((value.X + value.Width) <= (X + Width))) && (Y <= value.Y)) && ((value.Y + value.Height) <= (Y + Height));
    }
    
    public void Contains(Point value, out bool result)
    {
        result = (((X <= value.X) && (value.X < (X + Width))) && (Y <= value.Y)) && (value.Y < (Y + Height));
    }

    public bool Contains(Point value)
    {
        return ((((X <= value.X) && (value.X < (X + Width))) && (Y <= value.Y)) && (value.Y < (Y + Height)));
    }
    
    public void Inflate(float horizontalAmount, float verticalAmount)
    {
        X -= horizontalAmount;
        Y -= verticalAmount;
        Width += horizontalAmount * 2;
        Height += verticalAmount * 2;
    }
    
    public void Inflate(Vector2 amount)
    {
        X -= amount.X;
        Y -= amount.Y;
        Width += amount.X * 2;
        Height += amount.Y * 2;
    }
    
    public bool Intersects(RectangleF value)
    {
        return value.Left < Right && Left < value.Right && value.Top < Bottom && Top < value.Bottom;
    }
    
    public void Intersects(ref RectangleF value, out bool result)
    {
        result = value.Left < Right && Left < value.Right && value.Top < Bottom && Top < value.Bottom;
    }
    
    public static void Intersects(ref RectangleF value1, ref RectangleF value2, out RectangleF result)
    {
        if (value1.Intersects(value2))
        {
            var right_side = System.Math.Min(value1.X + value1.Width, value2.X + value2.Width);
            var left_side = System.Math.Max(value1.X, value2.X);
            var top_side = System.Math.Max(value1.Y, value2.Y);
            var bottom_side = System.Math.Min(value1.Y + value1.Height, value2.Y + value2.Height);

            result = new RectangleF(left_side, top_side, right_side - left_side, bottom_side - top_side);
        }
        else
        {
            result = new RectangleF(0, 0, 0, 0);
        }
    }
    
    public static RectangleF Intersect(RectangleF value1, RectangleF value2)
    {
        Intersects(ref value1, ref value2, out var rectangle);
        return rectangle;
    }

    public void Offset(int offsetX, int offsetY)
    {
        X += offsetX;
        Y += offsetY;
    }
    
    public void Offset(Vector2 amount)
    {
        X += amount.X;
        Y += amount.Y;
    }
    
    public void Offset(float offsetX, float offsetY)
    {
        X += offsetX;
        Y += offsetY;
    }
    
    public void Offset(Point amount)
    {
        X += amount.X;
        Y += amount.Y;
    }
    
    public static RectangleF Union(RectangleF value1, RectangleF value2)
    {
        var x = System.Math.Min(value1.X, value2.X);
        var y = System.Math.Min(value1.Y, value2.Y);
        return new RectangleF(x, y, System.Math.Max(value1.Right, value2.Right) - x, System.Math.Max(value1.Bottom, value2.Bottom) - y);
    }
    
    public static void Union(ref RectangleF value1, ref RectangleF value2, out RectangleF result)
    {
        result = new RectangleF();
        result.X = System.Math.Min(value1.X, value2.X);
        result.Y = System.Math.Min(value1.Y, value2.Y);
        result.Width = System.Math.Max(value1.Right, value2.Right) - result.X;
        result.Height = System.Math.Max(value1.Bottom, value2.Bottom) - result.Y;
    }

    public static RectangleF Overlap(RectangleF value1, RectangleF value2)
    {
        var x = System.Math.Max(System.Math.Max(value1.X, value2.X), 0);
        var y = System.Math.Max(System.Math.Max(value1.Y, value2.Y), 0);

        return new RectangleF(x, y, System.Math.Max(System.Math.Min(value1.Right, value2.Right) - x, 0),
            System.Math.Max(System.Math.Min(value1.Bottom, value2.Bottom) - y, 0));
    }
    
    public static void Overlap(ref RectangleF value1, ref RectangleF value2, out RectangleF result)
    {
        result = new RectangleF();
        result.X = System.Math.Max(System.Math.Max(value1.X, value2.X), 0);
        result.Y = System.Math.Max(System.Math.Max(value1.Y, value2.Y), 0);
        result.Width = System.Math.Max(System.Math.Min(value1.Right, value2.Right) - result.X, 0);
        result.Height = System.Math.Max(System.Math.Min(value1.Bottom, value2.Bottom) - result.Y, 0);
    }

    public void CalculateBounds(Vector2 parentPosition, Vector2 position, Vector2 origin, Vector2 scale, float rotation,
        float width, float height)
    {
        if (rotation == 0f)
        {
            X = parentPosition.X + position.X - origin.X * scale.X;
            Y = parentPosition.Y + position.Y - origin.Y * scale.Y;
            Width = width * scale.X;
            Height = height * scale.Y;
        }
        else
        {
            var worldPosX = parentPosition.X + position.X;
            var worldPosY = parentPosition.Y + position.Y;
            
            // Set the reference point to world taking origin into account
            Matrix.CreateTranslation(-worldPosX - origin.X, -worldPosY - origin.Y, 0f, out var transformMat);
            Matrix.CreateScale(scale.X, scale.Y, 0, out var scaleMat);
            Matrix.Multiply(ref transformMat, ref scaleMat, out transformMat);

            Matrix.CreateRotationZ(rotation, out scaleMat);
            
            Matrix.Multiply(ref transformMat, ref scaleMat, out transformMat);
            
            Matrix.CreateTranslation(worldPosX, worldPosY, 0f, out scaleMat);
            
            Matrix.Multiply(ref transformMat, ref scaleMat, out transformMat);
            
            // Calculate the four corners by transforming the offset vectors
            var topLeft = new Vector2(worldPosX, worldPosY);
            var topRight = new Vector2(worldPosX + width, worldPosY);
            var bottomLeft = new Vector2(worldPosX, worldPosY + height);
            var bottomRight = new Vector2(worldPosX + width, worldPosY + height);
            
            Vector2.Transform(ref topLeft, ref transformMat, out topLeft);
            Vector2.Transform(ref topRight, ref transformMat, out topRight);
            Vector2.Transform(ref bottomLeft, ref transformMat, out bottomLeft);
            Vector2.Transform(ref bottomRight, ref transformMat, out bottomRight);
            
            // Calculate the min and max positions
            // The min position is the smallest X and smallest Y
            // The max position is the largest X and largest Y
            
            var minX = System.Math.Min(System.Math.Min(topLeft.X, topRight.X), System.Math.Min(bottomLeft.X, bottomRight.X));
            var maxX = System.Math.Max(System.Math.Max(topLeft.X, topRight.X), System.Math.Max(bottomLeft.X, bottomRight.X));
            var minY = System.Math.Min(System.Math.Min(topLeft.Y, topRight.Y), System.Math.Min(bottomLeft.Y, bottomRight.Y));
            var maxY = System.Math.Max(System.Math.Max(topLeft.Y, topRight.Y), System.Math.Max(bottomLeft.Y, bottomRight.Y));

            Location = new Vector2(minX, minY);
            Width = maxX - minX;
            Height = maxY - minY;
        }
    }

    public RectangleF GetSweptBroadphaseBounds(float deltaX, float deltaY)
    {
        var broadPhaseBox = Empty;
        
        broadPhaseBox.X = deltaX > 0 ? X : X + deltaX;
        broadPhaseBox.Y = deltaY > 0 ? Y : Y + deltaY;
        broadPhaseBox.Width = deltaX > 0 ? deltaX + Width : Width - deltaX;
        broadPhaseBox.Height = deltaY > 0 ? deltaY + Height : Height - deltaY;
        
        return broadPhaseBox;
    }

    // This method will return true if the boxes are colliding.
    // moveX and moveY return the movement that the box has to be moved in order to not be colliding anymore.
    public bool CollisionCheck(ref RectangleF other, out float moveX, out float moveY)
    {
        moveX = moveY = 0.0f;
        
        var l = other.X - (X + Width);
        var r = (other.X + other.Width) - X;
        var t = other.Y - (Y + Height);
        var b = (other.Y + other.Height) - Y;
        
        // check that there was a collision
        if (l > 0 || r < 0 || t > 0 || b < 0)
            return false;
        
        // find the offset of both sides
        moveX = System.Math.Abs(l) < r ? l : r;
        moveY = System.Math.Abs(t) < b ? t : b;
        
        // only use whichever offset is the smallest
        if (System.Math.Abs(moveX) < System.Math.Abs(moveY))
            moveY = 0.0f;
        else
            moveX = 0.0f;
        
        return true;
    }

    public static Vector2 GetIntersectionDepth(ref RectangleF rectA, ref RectangleF rectB)
    {
        // Calculate half sizes.
        var halfWidthA = rectA.Width / 2.0f;
        var halfHeightA = rectA.Height / 2.0f;
        var halfWidthB = rectB.Width / 2.0f;
        var halfHeightB = rectB.Height / 2.0f;
        
        // Calculate centers.
        var centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
        var centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);
        
        // Calculate current and minimum-non-intersecting distances between centers.
        var distanceX = centerA.X - centerB.X;
        var distanceY = centerA.Y - centerB.Y;
        var minDistanceX = halfWidthA + halfWidthB;
        var minDistanceY = halfHeightA + halfHeightB;
        
        // If we are not intersecting at all, return (0, 0).
        if (System.Math.Abs(distanceX) >= minDistanceX || System.Math.Abs(distanceY) >= minDistanceY)
            return Vector2.Zero;
        
        // Calculate and return intersection depths.
        var depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
        var depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
        
        return new Vector2(depthX, depthY);
    }
    
    public static bool operator ==(RectangleF a, RectangleF b)
    {
        return a.X == b.X && a.Y == b.Y && a.Width == b.Width && a.Height == b.Height;
    }
    
    public static bool operator !=(RectangleF a, RectangleF b)
    {
        return !(a == b);
    }
    
    public override int GetHashCode()
    {
        return (int)(X + Y + Width + Height);
    }
    
    public bool Equals(object obj)
    {
        return (obj is RectangleF) && this == ((RectangleF)obj);
    }

    public bool Equals(RectangleF other)
    {
        return this == other;
    }

    public override string ToString()
    {
        return $"[X: {X}, Y: {Y}, Width: {Width}, Height: {Height}]";
    }

    public static implicit operator Rectangle(RectangleF self)
    {
        return new Rectangle((int)self.X, (int)self.Y, (int)self.Width, (int)self.Height);
    }
    #endregion
}