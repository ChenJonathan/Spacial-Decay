using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFlow : MonoBehaviour
{
    private Camera mainCamera;

    public void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if(hit.collider != null && hit.collider.gameObject.tag.Equals("Level"))
            {
                GameController.Singleton.LoadLevel(hit.collider.gameObject.GetComponent<Level>());
            }
        }
    }
}