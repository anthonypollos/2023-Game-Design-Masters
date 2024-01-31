using UnityEngine;

public class SetButtonLookTarget : MonoBehaviour
{
    private MenuManager mm;

    [SerializeField] [Tooltip("")] private float lookIndex;

    public void Start()
    {
        mm = FindObjectOfType<MenuManager>();
        GetComponent<Animator>().SetBool("MainMenu", true);
    }

    /// <summary>
    /// 
    /// </summary>
    public void SetLookTarget()
    {
        mm.SetLookIndex(lookIndex);
    }
}
