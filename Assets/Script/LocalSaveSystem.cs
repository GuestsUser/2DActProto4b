using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LocalSaveSystem : MonoBehaviour
{
    string path = Application.persistentDataPath + "/" + "save.json";
    LocalSave save;
    // Start is called before the first frame update
    void Start()
    {
        FileRead();
        FileWrite();
    }
    public void FileWrite()
    {
        string json = JsonUtility.ToJson(save);
        StreamWriter st = new StreamWriter(path);
        st.Write(json);
        st.Flush();
        st.Close();
    }
    public void FileRead()
    {
        if (File.Exists(path))
        {
            StreamReader st = new StreamReader(path);
            string json = st.ReadToEnd();
            st.Close();
            save = JsonUtility.FromJson<LocalSave>(json);
        }
        else { save = new LocalSave(); }
    }

    //public void FolderIni()
    //{

    //    if(Directory.Exists())
    //}
}
