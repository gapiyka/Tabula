using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private GameObject glidingBar;
    [SerializeField] private Text looseText;
    [SerializeField] private Text checkpointText;

    private const float CheckpointRange = 288.75f;
    private const float BarRange = 120f;

    private Vector3 barStartPos;

    public float playerDist = 0f;

    void Start()
    {
        barStartPos = glidingBar.transform.position;
    }

    void Update()
    {
        slider.value = (playerDist % CheckpointRange) / CheckpointRange;
    }

    public void MoveBarUp()
    {

        glidingBar.transform.Translate(Vector2.up * 10f);
        if (glidingBar.transform.position.y > barStartPos.y + BarRange)
            glidingBar.transform.position = 
                new Vector2(barStartPos.x, barStartPos.y + BarRange);
    }

    public void MoveBarDown()
    {
        glidingBar.transform.Translate(Vector2.down * 10f);
        if (glidingBar.transform.position.y < barStartPos.y - BarRange)
            glidingBar.transform.position =
                new Vector2(barStartPos.x, barStartPos.y - BarRange);
    }
    public void MoveBarToCenter()
    {
        glidingBar.transform.Translate((barStartPos - glidingBar.transform.position) * Time.deltaTime);
    }

    public IEnumerator LooseGame()
    {
        looseText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);//reload scene
    }

    public IEnumerator CheckPointTake()
    {
        checkpointText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        checkpointText.gameObject.SetActive(false);
    }
}
