using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LocalSaveSystem : MonoBehaviour
{
    [SerializeField] private string fileName= "save"; /* セーブファイル名(拡張子除く) */
    [SerializeField] private int saveVol = 0; /* セーブファイル数 */
    [SerializeField] private GameObject[] text;
    string path;
    LocalSave[] save;

    private void OnValidate() /* inspectorの値を変更した時呼び出される */
    {
        if (text.Length != saveVol) { text = new GameObject[saveVol]; } /* セーブファイル数を書き換えた時連動してtextのサイズも合わせられる */
    }
    // Start is called before the first frame update
    void Start()
    {
        path = Application.persistentDataPath + "/" + "save.json";
        save = new LocalSave[saveVol];
        for (int i = 0; i < saveVol; i++)
        {
            FileRead(i);
            //foreach(GameObject obj in text.)
        }
        //save[i].version = Application.version;
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
    public void FileRead(int num)
    {
        string directory = path + fileName + num.ToString() + ".json";
        if (File.Exists(directory))
        {
            StreamReader st = new StreamReader(directory);
            string json = st.ReadToEnd();
            st.Close();
            save[num] = JsonUtility.FromJson<LocalSave>(directory);
        }
    }
}
