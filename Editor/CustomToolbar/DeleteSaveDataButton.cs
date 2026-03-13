using System.IO;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace KszUtil.Editor.CustomToolbar
{
    public static class DeleteSaveDataButton
    {
        [MainToolbarElement(
            "HighKing/DeleteSaveData",
            defaultDockPosition = MainToolbarDockPosition.Middle,
            defaultDockIndex = 300
        )]
        public static MainToolbarElement CreateButton()
        {
            var icon = (Texture2D)EditorGUIUtility.Load("d_SaveAs@2x");
            var content = new MainToolbarContent(icon, tooltip: "Delete all save data.");
            return new MainToolbarButton(content, DeleteAllSaveData);
        }

        private static void DeleteAllSaveData()
        {
            if (EditorUtility.DisplayDialog("Clear Persistent Data Path and PlayerPrefs", "Are you sure you wish to clear the persistent data path and PlayerPrefs?\n This action cannot be reversed.", "Clear", "Cancel"))
            {
                var directoryInfo = new DirectoryInfo(Application.persistentDataPath);

                foreach (var file in directoryInfo.GetFiles())
                {
                    file.Delete();
                }

                foreach (var directory in directoryInfo.GetDirectories())
                {
                    directory.Delete(true);
                }

                PlayerPrefs.DeleteAll();
            }
        }
    }
}