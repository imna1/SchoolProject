using UnityEngine;
using TMPro;

public class ScenePanel : MonoBehaviour
{
    public TextMeshProUGUI Text;

    public void Load()
    {
        SceneLoader.instance.LoadScene(Text.text);
    }
}
