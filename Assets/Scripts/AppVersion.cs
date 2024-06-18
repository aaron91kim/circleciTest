using TMPro;
using UnityEngine;
using XGolf.Service;

public class AppVersion : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] TMP_Text env;
    [SerializeField] TMP_Text version;
    [SerializeField] TMP_Text email;
    [SerializeField] TMP_Text password;

#pragma warning restore
    void Start()
    {
        UpdateVersion();
        UpdateEnv();
    }

    private void UpdateVersion()
    {
        version.text = Application.version;
    }

    private void UpdateEnv()
    {
        var envConfig = Resources.Load<EnvConfig>("ENV");
        env.text = envConfig.ENV;
        email.text = envConfig.SIMULATOR_EMAIL;
        password.text = envConfig.SIMULATOR_PASSWORD;
    }
}