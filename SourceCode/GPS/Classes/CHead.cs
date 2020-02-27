﻿using System;
using System.Collections.Generic;

namespace AgOpenGPS
{
    public class CHead
    {
        //copy of the mainform address
        private readonly FormGPS mf;

        public double singleSpaceHeadlandDistance;
        public bool isOn;
        public double leftToolDistance;
        public double rightToolDistance;
        private int A, B, C, D, Q;

        //generated box for finding closest point
        public vec2 downL = new vec2(9000, 9000), downR = new vec2(9000, 9002);
        public bool isToolUp, isToolInWorkArea, isToolInBothPlaces, isToolInHeadland;
        public bool isLookInWorkArea, isLookInBothPlaces, isLookInHealand;

        /// <summary>
        /// array of turns
        /// </summary>
        public List<CHeadLines> headArr = new List<CHeadLines>();

        //constructor
        public CHead(FormGPS _f)
        {
            mf = _f;
            singleSpaceHeadlandDistance = 18;
            isOn = false;
            leftToolDistance = 99999;
            rightToolDistance = 99999;
            isToolUp = true;

        }

        public bool FindHeadlandDistance()
        {
            if (mf.bnd.LastBoundary > -1 && headArr.Count > mf.bnd.LastBoundary)
            {
                leftToolDistance = 999999;
                rightToolDistance = 999999;

                double minDistA = 1000000, minDistB = 1000000;

                int ptCount = headArr[mf.bnd.LastBoundary].hdLine.Count;
                if (ptCount == 0) return false;
                //find the closest 2 points to current fix
                for (int t = 0; t < ptCount; t++)
                {
                    double dist = glm.DistanceSquared(mf.section[0].leftPoint, headArr[mf.bnd.LastBoundary].hdLine[t]);
                    if (dist < minDistA)
                    {
                        minDistB = minDistA;
                        B = A;
                        minDistA = dist;
                        A = t;
                    }
                    else if (dist < minDistB)
                    {
                        minDistB = dist;
                        B = t;
                    }
                }

                //just need to make sure the points continue ascending or heading switches all over the place
                if (A > B) { Q = A; A = B; B = Q; }

                double dx = headArr[mf.bnd.LastBoundary].hdLine[B].easting - headArr[mf.bnd.LastBoundary].hdLine[A].easting;
                double dz = headArr[mf.bnd.LastBoundary].hdLine[B].northing - headArr[mf.bnd.LastBoundary].hdLine[A].northing;

                if (Math.Abs(dx) < Double.Epsilon && Math.Abs(dz) < Double.Epsilon) return false;

                //abHeading = Math.Atan2(dz, dx);
                //double abHeading = curList[A].heading;

                //how far from current AB Line is fix
                leftToolDistance = ((dz * mf.section[0].leftPoint.easting) - (dx * mf.section[0].leftPoint.northing) + (headArr[mf.bnd.LastBoundary].hdLine[B].easting
                            * headArr[mf.bnd.LastBoundary].hdLine[A].northing) - (headArr[mf.bnd.LastBoundary].hdLine[B].northing * headArr[mf.bnd.LastBoundary].hdLine[A].easting))
                                / Math.Sqrt((dz * dz) + (dx * dx));

                //are we on the right side or not
                //bool isOnRightSideCurrentLine = leftToolDistance > 0;
                //absolute the distance
                //leftToolDistance = Math.Abs(leftToolDistance);

                //find the closest 2 points to current fixm for right side
                minDistA = 1000000;
                minDistB = 1000000;

                for (int t = 0; t < ptCount; t++)
                {
                    double dist = glm.DistanceSquared(mf.section[mf.tool.numOfSections-1].rightPoint, headArr[mf.bnd.LastBoundary].hdLine[t]);
                    if (dist < minDistA)
                    {
                        minDistB = minDistA;
                        D = C;
                        minDistA = dist;
                        C = t;
                    }
                    else if (dist < minDistB)
                    {
                        minDistB = dist;
                        D = t;
                    }
                }

                //just need to make sure the points continue ascending or heading switches all over the place
                if (C > D) { Q = C; C = D; D = Q; }

                dx = headArr[mf.bnd.LastBoundary].hdLine[D].easting - headArr[mf.bnd.LastBoundary].hdLine[C].easting;
                dz = headArr[mf.bnd.LastBoundary].hdLine[D].northing - headArr[mf.bnd.LastBoundary].hdLine[C].northing;

                if (Math.Abs(dx) < Double.Epsilon && Math.Abs(dz) < Double.Epsilon) return false;

                //abHeading = Math.Atan2(dz, dx);
                //double abHeading = curList[C].heading;

                //how far from current AB Line is fix
                rightToolDistance = ((dz * mf.section[mf.tool.numOfSections - 1].rightPoint.easting) - (dx * mf.section[mf.tool.numOfSections - 1].rightPoint.northing) + (headArr[mf.bnd.LastBoundary].hdLine[D].easting
                            * headArr[mf.bnd.LastBoundary].hdLine[C].northing) - (headArr[mf.bnd.LastBoundary].hdLine[D].northing * headArr[mf.bnd.LastBoundary].hdLine[C].easting))
                                / Math.Sqrt((dz * dz) + (dx * dx));

                //are we on the right side or not
                //bool isOnRightSideCurrentLine = leftToolDistance > 0;

                //absolute the distance
                //leftToolDistance = Math.Abs(leftToolDistance);
                //double scanWidthR = (mf.tool.toolWidth * 0.75);

                if (leftToolDistance > 0 && rightToolDistance > 0)
                {
                    isToolInWorkArea = true;
                    isToolInBothPlaces = isToolInHeadland = false;
                }
                else if (leftToolDistance < 0 && rightToolDistance < 0)
                {
                    isToolInHeadland  = true;
                    isToolInBothPlaces = isToolInWorkArea = false;
                }
                else
                {
                    isToolInBothPlaces = true;
                    isToolInHeadland = isToolInWorkArea = false;
                }

                vec3 toolFix = mf.toolPos;
                double headAB = toolFix.heading;

                downL.easting = mf.section[0].leftPoint.easting + (Math.Sin(headAB) * mf.vehicle.hydLiftLookAhead * mf.tool.toolFarLeftSpeed);
                downL.northing = mf.section[0].leftPoint.northing + (Math.Cos(headAB) * mf.vehicle.hydLiftLookAhead * mf.tool.toolFarLeftSpeed);

                downR.easting = mf.section[mf.tool.numOfSections - 1].rightPoint.easting + (Math.Sin(headAB) * mf.vehicle.hydLiftLookAhead * mf.tool.toolFarRightSpeed);
                downR.northing = mf.section[mf.tool.numOfSections - 1].rightPoint.northing + (Math.Cos(headAB) * mf.vehicle.hydLiftLookAhead * mf.tool.toolFarRightSpeed);

                //bool isToolFullyOut = 
                bool isLookRightIn = IsPointInsideHeadLine(downR);
                bool isLookLeftIn = IsPointInsideHeadLine(downL);


                if (isLookLeftIn && isLookRightIn)
                {
                    isLookInWorkArea = true;
                    isLookInBothPlaces = isLookInHealand = false;
                }
                else if (!isLookLeftIn && !isLookRightIn)
                {
                    isLookInHealand = true;
                    isLookInBothPlaces = isLookInWorkArea = false;
                }
                else
                {
                    isLookInBothPlaces = true;
                    isLookInHealand = isLookInWorkArea = false;
                }

                //isToolUp = false;

                if (!isLookInHealand && isToolUp) isToolUp = false;
                if (isLookInHealand && isToolInHeadland && !isToolUp) isToolUp = true;

                mf.mc.machineData[mf.mc.mdHydLift] = 0;

                if (mf.vehicle.isHydLiftOn)
                {
                    if (isToolUp) mf.mc.machineData[mf.mc.mdHydLift] = 2;
                    else mf.mc.machineData[mf.mc.mdHydLift] = 1;
                }

                return isToolUp;
            }
            else
            {
                leftToolDistance = 99999;
                rightToolDistance = 99999;
                return false;
            }
        }

        public void DrawHeadLinesBack()
        {
            for (int i = 0; i < mf.bnd.bndArr.Count; i++)
            {
                if (mf.bnd.LastBoundary >= 0 || (i == mf.bnd.LastBoundary || (!mf.bnd.bndArr[i].isOwnField && mf.bnd.bndArr[i].OuterField == -1) || mf.bnd.bndArr[i].OuterField == mf.bnd.LastBoundary))
                {
                    if (headArr[i].hdLine.Count > 0) headArr[i].DrawHeadLineBackBuffer();
                }
            }

        }
        public void DrawHeadLines()
        {
            for (int i = 0; i < mf.bnd.bndArr.Count; i++)
            {
                if (mf.bnd.LastBoundary >= 0 || (i == mf.bnd.LastBoundary || (!mf.bnd.bndArr[i].isOwnField && mf.bnd.bndArr[i].OuterField == -1) || mf.bnd.bndArr[i].OuterField == mf.bnd.LastBoundary))
                {
                    if (headArr[i].hdLine.Count > 0) headArr[i].DrawHeadLine(mf.ABLine.lineWidth);
                }
            }
        }

        public bool IsPointInsideHeadLine(vec2 pt)
        {
            //if inside outer boundary, then potentially add
            if (mf.bnd.LastBoundary > -1 && headArr.Count > mf.bnd.LastBoundary && headArr[mf.bnd.LastBoundary].IsPointInHeadArea(pt))
            {
                for (int b = 0; b < mf.bnd.bndArr.Count; b++)
                {
                    if (!mf.bnd.bndArr[b].isOwnField)
                    {
                        if (headArr[b].IsPointInHeadArea(pt))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}