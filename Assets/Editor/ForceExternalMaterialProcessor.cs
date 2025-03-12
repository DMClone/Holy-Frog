#if UNITY_EDITOR
// An asset postprocessor that sets the material setting of a model to "Use External Materials (Legacy)".
//
// It only processes an asset if it's a new one, that didn't exist in the project yet.
// Duplicating an asset inside Unity does not count as new asset in  this case.
// It counts as new asset if the .meta file is missing.
//
// Save as: Assets/Editor/ForceExternalMaterialProcessor.cs
using UnityEngine;
using UnityEditor;

public class ForceExternalMaterialProcessor : AssetPostprocessor
{
    void OnPreprocessModel()
    {
        var modelImporter = assetImporter as ModelImporter;
        modelImporter.materialLocation = ModelImporterMaterialLocation.External;
    }
}
#endif