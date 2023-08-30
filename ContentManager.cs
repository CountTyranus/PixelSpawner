using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ContentManager : MonoBehaviour
{
    #region Singleton
    public static ContentManager Instance
    {
        get
        {
            if(_instance)
            {
                return _instance;
            }

            else
            {
                _instance = FindObjectOfType<ContentManager>();
                return _instance;
            }
        }
    }
    private static ContentManager _instance;
    #endregion

    [SerializeField]
    private TextMeshProUGUI _headline;

    [SerializeField]
    private TextMeshProUGUI _projectInformationHolder;

    [SerializeField]
    private Image[] _images;

    [SerializeField]
    private CanvasGroup _tableOfContentsGroup;

    [SerializeField]
    private CanvasGroup _projectInformationGroup;
    
    private string _defaultHeadlineText = "Projekte - Daniel Schneider";

    [SerializeField]
    private GameObject ButtonObject;

    private void Start()
    {
        BackToMainMenu();
    }

    public void DisplayProjectInformation(ProjectData data)
    {
        ToggleCanvasGroup(_tableOfContentsGroup, false, 0);
        ToggleCanvasGroup(_projectInformationGroup, true, 1);

        _headline.SetText(data.ProjectName);
        _projectInformationHolder.fontSize = 36;

        if (data.FontSizeOverride != 0)
        {
            _projectInformationHolder.fontSize = data.FontSizeOverride;
        }
    
        _projectInformationHolder.SetText(data.ProjectInfo);

        Color c = new Color(1, 1, 1, 0);

        foreach(Image i in _images)
        {
            i.color = c;
        }

        for (int i = 0; i < data.Screenshots.Length; i++)
        {
            _images[i].color = Color.white;
            _images[i].sprite = data.Screenshots[i];
            _images[i].preserveAspect = true;
        }

        if(data.HasButton)
        {
            VideoLoader loader = FindObjectOfType<VideoLoader>();
            ButtonObject.SetActive(true);
            ButtonObject.GetComponent<Button>().onClick.RemoveAllListeners();
            ButtonObject.GetComponent<Button>().onClick.AddListener( () => loader.LoadScene(data.TargetURL));
        }
    }

    void ToggleCanvasGroup(CanvasGroup group, bool interactable, float alpha)
    {
        group.blocksRaycasts = interactable;
        group.interactable = interactable;
        group.alpha = alpha;
    }

    public void BackToMainMenu()
    {
        ToggleCanvasGroup(_projectInformationGroup, false, 0);
        ToggleCanvasGroup(_tableOfContentsGroup, true, 1);
        _headline.SetText(_defaultHeadlineText);
        ButtonObject.SetActive(false);
    }

}
