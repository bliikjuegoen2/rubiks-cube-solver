using Godot;
using System;
using System.Linq;
using System.Text.RegularExpressions;

public class Cube : Spatial
{
    Piece[][][] pieces;

    // Colours

    Material black;
    Material white;
    Material yellow;
    Material red;
    Material orange;
    Material blue;
    Material green;

    public enum Move
    {
        U
        , D 
        , R 
        , L 
        , F 
        , B
    }

    public enum Sticker {
        NoSticker
        , White
        , Yellow 
        , Red 
        , Orange 
        , Blue 
        , Green
    }

    Sticker[][][][] stickers;

    int[][] faceZToPieceFormat;
    int[][] faceXToPieceFormat;

    (int,int)[] ring;

    (int,int,int) ringSelectX;
    (int,int) cornerFixX;
    (int,int,int) ringSelectY;
    (int,int) cornerFixY;
    (int,int,int) ringSelectZ;
    (int,int) cornerFixZ;
    (int,int) cornerFixHalf;

    public override void _Ready()
    {
        faceZToPieceFormat = new int[][]{
            new int[] {1, 1, 1}
            , new int[] {0, 0, 0}
            , new int[] {1, 1, 1}
        };

        faceXToPieceFormat = new int[][]{
            new int[] {2, 1, 2}
            , new int[] {1, 0, 1}
            , new int[] {2, 1, 2}
        };

        ring = new (int,int)[]{
            (0,0)
            , (1,0)
            , (2,0)
            , (2,1)
            , (2,2)
            , (1,2)
            , (0,2)
            , (0,1)
        };

        ringSelectX = (0,1,2);
        cornerFixX = (0,1);
        ringSelectY = (2,0,1);
        cornerFixY = (1,2);
        ringSelectZ = (1,2,0);
        cornerFixZ = (0,2);
        cornerFixHalf = (0,0);

        pieces = new Piece[][][]{
            // left
            new Piece[][]{
                // up
                new Piece[]{
                    // front
                    GetNode<Piece>("CornerUFL")
                    , GetNode<Piece>("EdgeUL")
                    , GetNode<Piece>("CornerUBL")
                    // back
                }
                , new Piece[]{
                    // front
                    GetNode<Piece>("EdgeFL")
                    , GetNode<Piece>("CentreL")
                    , GetNode<Piece>("EdgeBL")
                    // back
                }
                , new Piece[]{
                    // front
                    GetNode<Piece>("CornerDFL")
                    , GetNode<Piece>("EdgeDL")
                    , GetNode<Piece>("CornerDBL")
                    // back
                }
                // down
            }
            , new Piece[][]{
                // up
                new Piece[]{
                    // front
                    GetNode<Piece>("EdgeUF")
                    , GetNode<Piece>("CentreU")
                    , GetNode<Piece>("EdgeUB")
                    // back
                }
                , new Piece[]{
                    // front
                    GetNode<Piece>("CentreF")
                    , GetNode<Piece>("Core")
                    , GetNode<Piece>("CentreB")
                    // back
                }
                , new Piece[]{
                    // front
                    GetNode<Piece>("EdgeDF")
                    , GetNode<Piece>("CentreD")
                    , GetNode<Piece>("EdgeDB")
                    // back
                }
                // down
            }
            , new Piece[][]{
                // up
                new Piece[]{
                    // front
                    GetNode<Piece>("CornerUFR")
                    , GetNode<Piece>("EdgeUR")
                    , GetNode<Piece>("CornerUBR")
                    // back
                }
                , new Piece[]{
                    // front
                    GetNode<Piece>("EdgeFR")
                    , GetNode<Piece>("CentreR")
                    , GetNode<Piece>("EdgeBR")
                    // back
                }
                , new Piece[]{
                    // front
                    GetNode<Piece>("CornerDFR")
                    , GetNode<Piece>("EdgeDR")
                    , GetNode<Piece>("CornerDBR")
                    // back
                }
                // down
            }
            // right
        };

        black = ResourceLoader.Load<Material>("res://Cube/Colour/black.tres");
        white = ResourceLoader.Load<Material>("res://Cube/Colour/white.tres");
        yellow = ResourceLoader.Load<Material>("res://Cube/Colour/yellow.tres");
        red = ResourceLoader.Load<Material>("res://Cube/Colour/red.tres");
        orange = ResourceLoader.Load<Material>("res://Cube/Colour/orange.tres");
        blue = ResourceLoader.Load<Material>("res://Cube/Colour/blue.tres");
        green = ResourceLoader.Load<Material>("res://Cube/Colour/green.tres");

        stickers = new Sticker[3][][][];

        for(int i = 0; i < 3; i ++)
        {
            stickers[i] = new Sticker[3][][];

            for(int j = 0; j < 3; j ++)
            {
                stickers[i][j] = new Sticker[3][];
                
                for(int k = 0; k < 3; k ++)
                {
                    int[] indices = {i,j,k};
                    stickers[i][j][k] = new Sticker[indices.Count(s => s != 1)];
                }
            }
        }

        // set white face
        for(int i = 0; i < 3; i ++)
        for(int k = 0; k < 3; k ++)
        {
            stickers[i][0][k][0] = Sticker.White;
        }

        // set yellow face
        for(int i = 0; i < 3; i ++)
        for(int k = 0; k < 3; k ++)
        {
            stickers[i][2][k][0] = Sticker.Yellow;
        }

        // set red face
        for(int i = 0; i < 3; i ++)
        for(int j = 0; j < 3; j ++)
        {
            stickers[i][j][0][faceZToPieceFormat[j][i]] = Sticker.Red;
        }

        // set orange face
        for(int i = 0; i < 3; i ++)
        for(int j = 0; j < 3; j ++)
        {
            stickers[i][j][2][faceZToPieceFormat[j][i]] = Sticker.Orange;
        }

        // set green face
        for(int j = 0; j < 3; j ++)
        for(int k = 0; k < 3; k ++)
        {
            stickers[0][j][k][faceXToPieceFormat[j][k]] = Sticker.Green;
        }

        // set blue face
        for(int j = 0; j < 3; j ++)
        for(int k = 0; k < 3; k ++)
        {
            stickers[2][j][k][faceXToPieceFormat[j][k]] = Sticker.Blue;
        }

        RunAlgorithm("R U R' U R U2 R'");

        UpdateCube();
    }

    public void UpdateCube()
    {
        // top
        for(int i = 0; i < 3; i ++)
        for(int k = 0; k < 3; k ++)
        {
            pieces[i][0][k].SetColourY(ToMaterial(stickers[i][0][k][0]));
        }

        // bottom
        for(int i = 0; i < 3; i ++)
        for(int k = 0; k < 3; k ++)
        {
            pieces[i][2][k].SetColourY(ToMaterial(stickers[i][2][k][0]));
        }

        // front
        for(int i = 0; i < 3; i ++)
        for(int j = 0; j < 3; j ++)
        {
            pieces[i][j][0].SetColourZ(ToMaterial(stickers[i][j][0][faceZToPieceFormat[j][i]]));
        }

        // back
        for(int i = 0; i < 3; i ++)
        for(int j = 0; j < 3; j ++)
        {
            pieces[i][j][2].SetColourZ(ToMaterial(stickers[i][j][2][faceZToPieceFormat[j][i]]));
        }

        // left
        for(int j = 0; j < 3; j ++)
        for(int k = 0; k < 3; k ++)
        {
            pieces[0][j][k].SetColourX(ToMaterial(stickers[0][j][k][faceXToPieceFormat[j][k]]));
        }

        // right
        for(int j = 0; j < 3; j ++)
        for(int k = 0; k < 3; k ++)
        {
            pieces[2][j][k].SetColourX(ToMaterial(stickers[2][j][k][faceXToPieceFormat[j][k]]));
        }
    }

    private void TurnInternal((int,int,int) ringSelect, (int,int) cornerFix, int layer, int shift, bool invertEdge) {

        var ringTemp = new Sticker[8][];

        for(int n = 0; n < 8; n ++) 
        {
            var (i,j) = ring[n];

            int[] indices = {layer, i, j};

            ringTemp[n] = stickers
                [indices[ringSelect.Item1]]
                [indices[ringSelect.Item2]]
                [indices[ringSelect.Item3]];

            // fix corner faces
            if(n % 2 == 0) {
                (ringTemp[n][cornerFix.Item1], ringTemp[n][cornerFix.Item2]) 
                    = (ringTemp[n][cornerFix.Item2], ringTemp[n][cornerFix.Item1]);
            }

            // invert edge
            if(invertEdge && n % 2 == 1) {
                (ringTemp[n][0],ringTemp[n][1]) = (ringTemp[n][1],ringTemp[n][0]);
            }
        }
        for(int n = 0; n < 8; n ++) 
        {
            int m = (n + shift) % 8;
            var (i,j) = ring[m];

            int[] indices = {layer, i, j};

            stickers
                [indices[ringSelect.Item1]]
                [indices[ringSelect.Item2]]
                [indices[ringSelect.Item3]] 
                = ringTemp[n];
        }
    }

    public void TurnLeft(bool half = false, bool inverse = false)
    {
        TurnInternal(ringSelectX, half ? cornerFixHalf : cornerFixX, 0, half ? 4 : inverse ? 6 : 2, false);
    }

    public void TurnRight(bool half = false, bool inverse = false)
    {
        TurnInternal(ringSelectX, half ? cornerFixHalf : cornerFixX, 2, half ? 4 : inverse ? 2 : 6, false);
    }

    public void TurnUp(bool half = false, bool inverse = false)
    {
        TurnInternal(ringSelectY, half ? cornerFixHalf : cornerFixY, 0, half ? 4 : inverse ? 6 : 2, false);
    }

    public void TurnDown(bool half = false, bool inverse = false)
    {
        TurnInternal(ringSelectY, half ? cornerFixHalf : cornerFixY, 2, half ? 4 : inverse ? 2 : 6, false);
    }

    public void TurnFront(bool half = false, bool inverse = false)
    {
        TurnInternal(ringSelectZ, half ? cornerFixHalf : cornerFixZ, 0, half ? 4 : inverse ? 6 : 2, true);
    }

    public void TurnBack(bool half = false, bool inverse = false)
    {
        TurnInternal(ringSelectZ, half ? cornerFixHalf : cornerFixZ, 2, half ? 4 : inverse ? 2 : 6, true);
    }

    public void RunAlgorithm(string algorithm) {

        string algorithmTrimmed = Regex.Replace(algorithm, "[^UDFBLR'2]", "");

        for(int i = 0; i < algorithmTrimmed.Length(); i++) 
        {
            int nextI = Math.Min(i + 1, algorithmTrimmed.Length() - 1);
            bool half = false;
            bool inverse = false;

            GD.Print(nextI);
            GD.Print(algorithmTrimmed[nextI]);

            switch(algorithmTrimmed[nextI])
            {
                case '\'':
                    inverse = true;
                    break;
                case '2':
                    half = true;
                    break;
            }

            switch(algorithmTrimmed[i])
            {
                case 'U':
                    TurnUp(half, inverse);
                    break;
                case 'D':
                    TurnDown(half, inverse);
                    break;
                case 'F':
                    TurnFront(half, inverse);
                    break;
                case 'B':
                    TurnBack(half, inverse);
                    break;
                case 'L':
                    TurnLeft(half, inverse);
                    break;
                case 'R':
                    TurnRight(half, inverse);
                    break;
            }
        }
    }

    public Material ToMaterial(Sticker sticker) {
        switch(sticker){
            case Sticker.White:
                return white;
            case Sticker.Yellow:
                return yellow;
            case Sticker.Red:
                return red;
            case Sticker.Orange:
                return orange;
            case Sticker.Green:
                return green;
            case Sticker.Blue:
                return blue;
        }
        return black;
    }


}
