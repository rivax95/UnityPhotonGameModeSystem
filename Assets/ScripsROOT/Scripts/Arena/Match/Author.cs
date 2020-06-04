using System;
using System.Reflection;

[AttributeUsage( AttributeTargets.Method)]
public class ThaidEvent : Attribute
{
    private string name;
    public double version;
    public Action Destino;
    public ThaidEvent(string name)
    {
        this.name = name;
        version = 1.0;

    }
    public void setDestino(ref Action a)
    {
        a = Destino;
    }
}