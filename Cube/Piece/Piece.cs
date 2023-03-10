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

    public void SetColours(Material colourX, Material colourY, Material colourZ)
    {
        boxX.Material = colourX;
        boxY.Material = colourY;
        boxZ.Material = colourZ;
    }
}
