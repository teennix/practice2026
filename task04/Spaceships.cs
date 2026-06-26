using System;

namespace task04
{
    public interface ISpaceship
    {
        void MoveForward();      // Движение вперед
        void Rotate(int angle);  // Поворот на угол (градусы)
        void Fire();             // Выстрел ракетой
        int Speed { get; }       // Скорость корабля
        int FirePower { get; }   // Мощность выстрела
    }

    public class Cruiser : ISpaceship
    {
        public int Speed => 50;
        public int FirePower => 100;

        public void MoveForward() => Console.WriteLine("Крейсер медленно движется вперед.");
        public void Rotate(int angle) => Console.WriteLine($"Крейсер поворачивается на {angle} градусов.");
        public void Fire() => Console.WriteLine("Крейсер запускает мощную ракету.");
    }

    public class Fighter : ISpaceship
    {
        public int Speed => 100;
        public int FirePower => 20;

        public void MoveForward() => Console.WriteLine("Истребитель быстро летит вперед.");
        public void Rotate(int angle) => Console.WriteLine($"Истребитель резко поворачивает на {angle} градусов.");
        public void Fire() => Console.WriteLine("Истребитель запускает слабую ракету.");
    }
}