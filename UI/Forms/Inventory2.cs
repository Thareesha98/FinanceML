// ShapeLibrary.cs (Conceptual Library File)
using System;

namespace ShapeCalculator
{
    // 1. Abstract Base Class
    public abstract class Shape
    {
        public string Name { get; protected set; }

        public abstract double CalculateArea();
        public abstract double CalculatePerimeter();

        public virtual void DisplayInfo()
        {
            Console.WriteLine($"\n--- {Name} ---");
            Console.WriteLine($"Area: {CalculateArea():F2}");
            Console.WriteLine($"Perimeter/Circumference: {CalculatePerimeter():F2}");
        }
    }

    // 2. Derived Class: Circle
    public class Circle : Shape
    {
        public double Radius { get; set; }

        public Circle(double radius)
        {
            if (radius <= 0) throw new ArgumentOutOfRangeException(nameof(radius), "Radius must be positive.");
            Name = "Circle";
            Radius = radius;
        }

        // Implementation of Abstract methods
        public override double CalculateArea()
        {
            return Math.PI * Radius * Radius;
        }

        public override double CalculatePerimeter() // Circumference
        {
            return 2 * Math.PI * Radius;
        }
    }

    // 3. Derived Class: Rectangle
    public class Rectangle : Shape
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public Rectangle(double width, double height)
        {
            if (width <= 0 || height <= 0) throw new ArgumentException("Dimensions must be positive.");
            Name = "Rectangle";
            Width = width;
            Height = height;
        }

        // Implementation of Abstract methods
        public override double CalculateArea()
        {
            return Width * Height;
        }

        public override double CalculatePerimeter()
        {
            return 2 * (Width + Height);
        }
    }

    // 4. Derived Class: Triangle (Focus on Heron's Formula for Area)
    public class Triangle : Shape
    {
        public double SideA { get; set; }
        public double SideB { get; set; }
        public double SideC { get; set; }

        public Triangle(double a, double b, double c)
        {
            // Basic triangle inequality check (A + B > C)
            if (a + b <= c || a + c <= b || b + c <= a)
                throw new ArgumentException("Invalid side lengths for a triangle.");

            Name = "Triangle";
            SideA = a;
            SideB = b;
            SideC = c;
        }

        public override double CalculateArea()
        {
            // Heron's Formula: Area = sqrt(s * (s-a) * (s-b) * (s-c)) where s = semi-perimeter
            double s = CalculatePerimeter() / 2;
            return Math.Sqrt(s * (s - SideA) * (s - SideB) * (s - SideC));
        }

        public override double CalculatePerimeter()
        {
            return SideA + SideB + SideC;
        }
    }
}

// Program.cs (Conceptual Console App to use the library)
using System;
using ShapeCalculator;

class Program
{
    static void Main(string[] args)
    {
        // Demonstration of Polymorphism: Treating derived classes as their base type (Shape)
        Shape[] shapes = new Shape[]
        {
            new Circle(5.0),
            new Rectangle(10.0, 4.5),
            new Triangle(3.0, 4.0, 5.0) // A right triangle
        };

        foreach (var shape in shapes)
        {
            // The appropriate CalculateArea and CalculatePerimeter method is called dynamically
            // based on the actual object type (Circle, Rectangle, or Triangle).
            shape.DisplayInfo();
        }

        try
        {
            var invalidCircle = new Circle(-10);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"\nError creating shape: {ex.Message}");
        }
    }
}
