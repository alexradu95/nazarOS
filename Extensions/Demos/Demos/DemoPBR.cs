﻿using StereoKit;
using StereoKit.Framework;

namespace Nazar.Extension.ZDemos.Demos;

class DemoPBR : IStepper
{
    const int materialGrid = 6;
    Material[] pbrMaterials;
    Model pbrModel;
    Mesh sphereMesh;

    Tex oldSkyTex;
    SphericalHarmonics oldSkyLight;

    Pose modelPose = Pose.Identity;
    public bool Enabled => throw new System.NotImplementedException();
    public bool Initialize()
    {
        oldSkyTex = Renderer.SkyTex;
        oldSkyLight = Renderer.SkyLight;
        sphereMesh = Mesh.GenerateSphere(1, 7);
        Renderer.SkyTex = Tex.FromCubemapEquirectangular(@"old_depot.hdr");
        Renderer.SkyTex.OnLoaded += t => Renderer.SkyLight = t.CubemapLighting;

        pbrModel = Model.FromFile("DamagedHelmet.gltf");
        pbrMaterials = new Material[materialGrid * materialGrid];
        for (int y = 0; y < materialGrid; y++)
        {
            for (int x = 0; x < materialGrid; x++)
            {
                int i = x + y * materialGrid;
                pbrMaterials[i] = new Material(Default.ShaderPbr);
                pbrMaterials[i][MatParamName.ColorTint] = Color.HSV(0.3f, 0.8f, 0.8f);
                pbrMaterials[i][MatParamName.MetallicAmount] = x / (float)(materialGrid - 1);
                pbrMaterials[i][MatParamName.RoughnessAmount] = y / (float)(materialGrid - 1);
            }

  

        }

        /// :CodeSample: Material.GetAllParamInfo Material.GetParamInfo Material.ParamCount MatParamInfo MatParamInfo.name MatParamInfo.type
        /// ### Listing parameters in a Material

        // Iterate using a foreach
        Log.Info("Builtin PBR Materials contain these parameters:");
        foreach (MatParamInfo info in Material.PBR.GetAllParamInfo())
            Log.Info($"- {info.name} : {info.type}");

        // Or with a normal for loop
        Log.Info("Builtin Unlit Materials contain these parameters:");
        for (int i = 0; i < Material.Unlit.ParamCount; i += 1)
        {
            MatParamInfo info = Material.Unlit.GetParamInfo(i);
            Log.Info($"- {info.name} : {info.type}");
        }
        /// :End:
        /// 
        return true;
    }

    public void Shutdown()
    {
        Renderer.SkyTex = oldSkyTex;
        Renderer.SkyLight = oldSkyLight;
    }

    public void Step()
    {
     

        Hierarchy.Push(Matrix.T(0, 0, -1));

        UI.HandleBegin("Model", ref modelPose, pbrModel.Bounds * .2f);
        pbrModel.Draw(Matrix.TRS(Vec3.Zero, Quat.FromAngles(0, 180, 0), 0.2f));
        UI.HandleEnd();

        for (int y = 0; y < materialGrid; y++)
        {
            for (int x = 0; x < materialGrid; x++)
            {
                Renderer.Add(sphereMesh, pbrMaterials[x + y * materialGrid], Matrix.TS(new Vec3(x - materialGrid / 2.0f + 0.5f, y - materialGrid / 2.0f + 0.5f, -1) / 4.0f, 0.2f));
            }
        }
        Text.Add("Metallic -->", Matrix.TRS(new Vec3(0, materialGrid / 8.0f + 0.2f, -0.25f), Quat.FromAngles(0, 180, 0), 4));
        Text.Add("Roughness -->", Matrix.TRS(new Vec3(materialGrid / -8.0f - 0.2f, 0, -0.25f), Quat.FromAngles(0, 180, -90), 4));
        Hierarchy.Pop();
    }
}