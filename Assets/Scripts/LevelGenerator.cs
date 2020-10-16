using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Holders")]
    public GameObject WallHolder;
    public GameObject PelletHolder;

    [Header("Level Generator")]
    public GameObject Empty;    //  0           :   Nothing
    public GameObject InsideCorner; //  3       :   Top Left
    public GameObject InsideCornerBL;    //  31 :   Bottom Left
    public GameObject InsideCornerBR;    //  32 :   Bottom Right
    public GameObject InsideCornerTR;    //  33 :   Top Right
    public GameObject InsideWall;   //  4       :
    public GameObject InsideWallH;  //  41      :
    public GameObject OutsideCorner;    //  1   :   Top Left
    public GameObject OutsideCornerBL;  //  11  :   Bottom Left
    public GameObject OutsideCornerBR;  //  12  :   Bottom Right
    public GameObject OutsideCornerTR;  //  13  :   Top Right
    public GameObject OutsideWall;  //  2
    public GameObject OutsideWallH; //  21
    public GameObject SmallDot; //  5
    public GameObject BigDot;   //  6
    public GameObject TJunction;    //  7
    public GameObject TJunctionU;    //  77

    [Header("Corners With Dots")]
    public GameObject DR;   //  8   :   Down Right
    public GameObject UR;   //  81  :   Up Right
    public GameObject DL;   //  83  :   Down Left
    public GameObject UL;   //  84  :   Up Left
    public GameObject LRU;  //  85  :   Horizontal Up
    public GameObject LRD;  //  86  :   Horizontal Down
    public GameObject UDL;  //  87  :   Vertical Left
    public GameObject UDR;  //  88  :   Vertical Right
    public GameObject ALL;  //  89  :   Intersection

    [Header("Portals")]
    public GameObject PortL;    //  9   :   Portal on the Left
    public GameObject PortR;    //  91  :   Portal on the Right.

    public const float PIXEL_32 = .3125f;

    #region Level Schematic
    readonly int[,] levelMap =
         {              //LEFT OF THE SCREEN\\                  //RIGHT OF THE SCREEN\\
         {11,21,21,21,21,21,21,21,21,21,21,21,21,77 , 77,21,21,21,21,21,21,21,21,21,21,21,21,12},   //  Bottom of the Screen
         {02,81,05,05,05,05,85,05,05,05,05,05,84,04 , 04,81,05,05,05,05,05,85,05,05,05,05,84,02},
         {02,05,31,41,41,32,05,31,41,41,41,32,05,04 , 04,05,31,41,41,41,32,05,31,41,41,32,05,02},
         {02,06,04,00,00,04,05,04,00,00,00,04,05,04 , 04,05,04,00,00,00,04,05,04,00,00,04,06,02},
         {02,05,03,41,41,33,05,03,41,41,41,33,05,03 , 33,05,03,41,41,41,33,05,03,41,41,33,05,02},
         {02,88,05,05,05,05,89,05,05,85,05,05,86,05 , 05,86,05,05,85,05,05,89,05,05,05,05,87,02},
         {02,05,31,41,41,32,05,31,32,05,31,41,41,41 , 41,41,41,32,05,31,32,05,31,41,41,32,05,02},
         {02,05,03,41,41,33,05,04,04,05,03,41,41,32 , 31,41,41,33,05,04,04,05,03,41,41,33,05,02},
         {02,08,05,05,05,05,87,04,04,08,05,05,-4,04 , 04,-1,05,05,83,04,04,88,05,05,05,05,83,02},
         {01,21,21,21,21,12,05,04,03,41,41,32,00,04 , 04,00,31,41,41,33,04,05,11,21,21,21,21,13},
         {00,00,00,00,00,02,05,04,31,41,41,33,00,03 , 33,00,03,41,41,32,04,05,02,00,00,00,00,00},
         {00,00,00,00,00,02,05,04,04,-1,00,00,-6,00 , 00,-6,00,00,-4,04,04,05,02,00,00,00,00,00},
         {00,00,00,00,00,02,05,04,04,00,31,41,00,00 , 00,00,41,32,00,04,04,05,02,00,00,00,00,00},
         {21,21,21,21,21,13,05,03,33,00,04,00,00,00 , 00,00,00,04,00,03,33,05,01,21,21,21,21,21},
         {09,00,00,00,00,00,89,00,00,-7,04,00,00,00 , 00,00,00,04,-9,00,00,89,00,00,00,00,00,91},   //  Portals Row
                                            //  Reflect Here \\
         {21,21,21,21,21,12,05,31,32,00,04,00,00,00 , 00,00,00,04,00,31,32,05,11,21,21,21,21,21},
         {00,00,00,00,00,02,05,04,04,00,03,41,00,00 , 00,00,41,33,00,04,04,05,02,00,00,00,00,00},
         {00,00,00,00,00,02,05,04,04,-8,00,00,-5,00 , 00,-5,00,00,-3,04,04,05,02,00,00,00,00,00},
         {00,00,00,00,00,02,05,04,03,41,41,32,00,31 , 32,00,31,41,41,33,04,05,02,00,00,00,00,00},
         {11,21,21,21,21,13,05,04,31,41,41,33,00,04 , 04,00,03,41,41,32,04,05,01,21,21,21,21,12},
         {02,81,05,05,05,05,87,04,04,81,05,05,-3,04 , 04,-8,05,05,84,04,04,88,05,05,05,05,84,02},
         {02,05,31,41,41,32,05,04,04,05,31,41,41,33 , 03,41,41,32,05,04,04,05,31,41,41,32,05,02},
         {02,05,03,41,41,33,05,03,33,05,03,41,41,41 , 41,41,41,33,05,03,33,05,03,41,41,33,05,02},
         {02,88,05,05,05,05,89,05,05,86,05,05,85,05 , 05,85,05,05,86,05,05,89,05,05,05,05,87,02},
         {02,05,31,41,41,32,05,31,41,41,41,32,05,31 , 32,05,31,41,41,41,32,05,31,41,41,32,05,02},
         {02,06,04,00,00,04,05,04,00,00,00,04,05,04 , 04,05,04,00,00,00,04,05,04,00,00,04,06,02},
         {02,05,03,41,41,33,05,03,41,41,41,33,05,04 , 04,05,03,41,41,41,33,05,03,41,41,33,05,02},
         {02,08,05,05,05,05,86,05,05,05,05,05,83,04 , 04,08,05,05,05,05,05,86,05,05,05,05,83,02},
         {01,21,21,21,21,21,21,21,21,21,21,21,21,07 , 07,21,21,21,21,21,21,21,21,21,21,21,21,13},   //  Top of the Screen
         };
    #endregion

    #region Level Generator Requirement

    void Awake()
    {
        for (int i = 0; i < levelMap.Length/28; i++)
            for (int k = 0; k < 28; k++)
                switch (levelMap[i, k])
                {
                    case 0: //  Empty
                        GeneratePlacements(Empty, new Vector3(k, i, 0f));
                        break;
                    case 1: //  Outside Corner
                        GeneratePlacements(OutsideCorner, new Vector3(k, i, 0f));
                        break;
                    case 11:
                        GeneratePlacements(OutsideCornerBL, new Vector3(k, i, 0f));
                        break;
                    case 12:
                        GeneratePlacements(OutsideCornerBR, new Vector3(k, i, 0f));
                        break;
                    case 13:
                        GeneratePlacements(OutsideCornerTR, new Vector3(k, i, 0f));
                        break;
                    case 2: //  Outside Wall
                        GeneratePlacements(OutsideWall, new Vector3(k, i, 0f));
                        break;
                    case 21:
                        GeneratePlacements(OutsideWallH, new Vector3(k, i, 0f));
                        break;
                    case 3: //  Inside Corner
                        GeneratePlacements(InsideCorner, new Vector3(k, i, 0f));
                        break;
                    case 31:
                        GeneratePlacements(InsideCornerBL, new Vector3(k, i, 0f));
                        break;
                    case 32:
                        GeneratePlacements(InsideCornerBR, new Vector3(k, i, 0f));
                        break;
                    case 33:
                        GeneratePlacements(InsideCornerTR, new Vector3(k, i, 0f));
                        break;
                    case 4: //  Inside Wall
                        GeneratePlacements(InsideWall, new Vector3(k, i, 0f));
                        break;
                    case 41:
                        GeneratePlacements(InsideWallH, new Vector3(k, i, 0f));
                        break;
                    case 5: //  Small Dot
                        PlaceSmallDot(k, i);
                        break;
                    case 6: //  Big Dot
                        PlaceBigDot(k, i);
                        break;
                    case 7: //  A 'T' Junction
                        GeneratePlacements(TJunction, new Vector3(k, i, 0f));
                        //Debug.Log("T Junction");
                        break;
                    case 77: //  A 'T' Junction
                        GeneratePlacements(TJunctionU, new Vector3(k, i, 0f));
                        //Debug.Log("T Junction");
                        break;
                    case 8: //  Down Right
                        GeneratePlacements(DR, new Vector3(k, i, 0f));
                        PlaceSmallDot(k, i);
                        break;
                    case 81:    //  Up Right
                        GeneratePlacements(UR, new Vector3(k, i, 0f));
                        PlaceSmallDot(k, i);
                        break;
                    case 83:    //  Down Left
                        GeneratePlacements(DL, new Vector3(k, i, 0f));
                        PlaceSmallDot(k, i);
                        break;
                    case 84:    //  Up Left
                        GeneratePlacements(UL, new Vector3(k, i, 0f));
                        PlaceSmallDot(k, i);
                        break;
                    case 85:    //  Horizontal Up
                        GeneratePlacements(LRU, new Vector3(k, i, 0f));
                        PlaceSmallDot(k, i);
                        break;
                    case 86:    //  Horizontal Down
                        GeneratePlacements(LRD, new Vector3(k, i, 0f));
                        PlaceSmallDot(k, i);
                        break;
                    case 87:    //  Vertical Left
                        GeneratePlacements(UDL, new Vector3(k, i, 0f));
                        PlaceSmallDot(k, i);
                        break;
                    case 88:    //  Vertical Right
                        GeneratePlacements(UDR, new Vector3(k, i, 0f));
                        PlaceSmallDot(k, i);
                        break;
                    case 89:    //  Intersection
                        GeneratePlacements(ALL, new Vector3(k, i, 0f));
                        PlaceSmallDot(k, i);
                        break;
                    case -8: //  Down Right
                        GeneratePlacements(DR, new Vector3(k, i, 0f));
                        break;
                    case -1:    //  Up Right
                        GeneratePlacements(UR, new Vector3(k, i, 0f));
                        break;
                    case -3:    //  Down Left
                        GeneratePlacements(DL, new Vector3(k, i, 0f));
                        break;
                    case -4:    //  Up Left
                        GeneratePlacements(UL, new Vector3(k, i, 0f));
                        break;
                    case -5:    //  Horizontal Up
                        GeneratePlacements(LRU, new Vector3(k, i, 0f));
                        break;
                    case -6:    //  Horizontal Down
                        GeneratePlacements(LRD, new Vector3(k, i, 0f));
                        break;
                    case -7:    //  Vertical Left
                        GeneratePlacements(UDL, new Vector3(k, i, 0f));
                        break;
                    case -9:    //  Vertical Right
                        GeneratePlacements(UDR, new Vector3(k, i, 0f));
                        break;
                    case 9:
                        GeneratePlacements(PortL, new Vector3(k, i, 0f));
                        break;
                    case 91:
                        GeneratePlacements(PortR, new Vector3(k, i, 0f));
                        break;
                }
    }

    void GeneratePlacements(GameObject Object, Vector3 where)
    {
        GameObject go = Instantiate(Object, where * PIXEL_32, Quaternion.identity);
        MakeParent(go, WallHolder);
    }

    void PlaceSmallDot(int k, int i)
    {
        GameObject go = Instantiate(SmallDot, (new Vector3(k, i, 0f) * PIXEL_32), Quaternion.identity);
        MakeParent(go, PelletHolder);
    }

    void PlaceBigDot(int k, int i)
    {
        GameObject go = Instantiate(BigDot, (new Vector3(k, i, 0f) * PIXEL_32), Quaternion.identity);
        MakeParent(go, PelletHolder);
    }

    void MakeParent(GameObject Child, GameObject Parent)
    {
        Child.transform.parent = Parent.transform;
    }

    #endregion
}
