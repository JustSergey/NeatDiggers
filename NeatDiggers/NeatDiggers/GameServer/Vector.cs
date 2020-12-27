using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NeatDiggers.GameServer.Maps;

namespace NeatDiggers.GameServer
{
    public struct Vector
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Vector(int x, int y) => (X, Y) = (x, y);

        public bool CheckAvailability(Vector targetPoint, int radius)
        {
            // Определяем стороны прямоугольного треугольника с гипотенузой между координатами этой точки и targetPoint
            double a = Math.Abs(targetPoint.X - X);
            double b = Math.Abs(targetPoint.Y - Y);
            double c = Math.Sqrt(a * a + b * b);
            
            double sinAlpha = a / c;
            double cosAlpha = b / c;
            
            //Находим точку пересечения гипотенузы с окружностью
            double x = sinAlpha * radius;
            double y = cosAlpha * radius;

            return (a - x <= 0.3f && b - y <= 0.3f);
        }

        public bool IsInMap(GameMap map)
        {
            return X < map.Width && X >= 0 && Y < map.Height && Y >= 0;
        }
    }
}
