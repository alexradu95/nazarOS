﻿using StereoKit;
using StereoKit.Framework;

namespace Nazar.Extension.ZDemos.Demos;

class DemoPhysics : IStepper
{
    Solid       floorSolid;
    Model       gltf;
    readonly List<Solid> objects     = new List<Solid>();
    Pose        optionsPose = new Pose(0.5f, 0,-0.5f, Quat.LookDir(-1,0,1));

    readonly Matrix      descPose    = Matrix.TR (-0.5f, 0, -0.5f, Quat.LookDir(1,0,1));
    readonly string      description = "StereoKit supports some basic rigidbody physics simulation. See the 'Solid' class in the docs.\n\nUse the panel to add or clear physics objects from the scene, and make a fist to interact with them yourself!";
    readonly Matrix      titlePose   = Matrix.TRS(V.XYZ(-0.5f, 0.05f, -0.5f), Quat.LookDir(1, 0, 1), 2);
    readonly string      title       = "Physics";
    public bool Enabled => throw new System.NotImplementedException();
    public bool Initialize()
    {
        gltf = Model.FromFile("DamagedHelmet.gltf");

        floorSolid = new Solid(World.HasBounds?World.BoundsPose.position:new Vec3(0, -1.5f, 0), Quat.Identity, SolidType.Immovable);
        floorSolid.AddBox(new Vec3(20, .01f, 20));

        Input.HandSolid(Handed.Max, false);
        Default.MaterialHand[MatParamName.ColorTint] = new Color(1, 1, 1, 0.4f);

        return true;
    }

    public void Step()
    {
        Pose solidPose;
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].GetPose(out solidPose);
            Renderer.Add(gltf, solidPose.ToMatrix(0.25f), Color.White);
        }
        for (int i = 0; i < (int)Handed.Max; i++)
        {
            Hand h = Input.Hand((Handed)i);
            if (h.IsJustGripped)
            {
                Input.HandSolid((Handed)i, true);
                Default.MaterialHand[MatParamName.ColorTint] = new Color(1, 1, 1, 1);
            }
            if (h.IsJustUngripped)
            {
                Input.HandSolid((Handed)i, false);
                Default.MaterialHand[MatParamName.ColorTint] = new Color(1, 1, 1, 0.4f);
            }
        }

        UI.WindowBegin("Objects", ref optionsPose);
        if (UI.Button("Add"))
        {
            objects.Add(new Solid(new Vec3(0, 3, -.6f), Quat.LookDir(0,0,1)));
            objects.Last().AddSphere(.45f, 40, V.XYZ(0,0,0.05f));
            objects.Last().AddBox(Vec3.One * .35f, 40, V.XYZ(0, 0, 0.05f));
        }
        if (UI.Button("Clear"))
            objects.Clear();
        UI.WindowEnd();

        Text.Add(title,       titlePose);
        Text.Add(description, descPose, V.XY(0.4f,0), TextFit.Wrap, TextAlign.TopCenter, TextAlign.TopLeft);
    }

    public void Shutdown()
    {
        Input.HandSolid(Handed.Max, true);
        Default.MaterialHand[MatParamName.ColorTint] = new Color(1, 1, 1, 1);
    }
}