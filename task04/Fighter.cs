namespace task04;

public class Fighter : ISpaceship
{
    public int Speed => 100;
    public int FirePower => 20; 

    public void MoveForward() { }
    public void Rotate(int angle) { }
    public void Fire() { }
}