using Godot;
using System;

public class Piece : CSGCombiner
{
    CSGBox boxX;
    CSGBox boxY;
    CSGBox boxZ;

    public override void _Ready()
    {
        boxX = GetNode<CSGBox>("X");
        boxY = GetNode<CSGBox>("Y");
        boxZ = GetNode<CSGBox>("Z");
    }

    public void SetColourX(Material colourX)
    {
        boxX.Material = colourX;
    }

    public void SetColourY(Material colourY)
    {
        boxY.Material = colourY;
    }

    public void SetColourZ(Material colourZ)
    {
        boxZ.Material = colourZ;
    }
}
