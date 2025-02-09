using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBricks : MonoBehaviour
{
    [SerializeField] private Transform CenterPoint;
    [SerializeField] private GameObject BrickPrefab;
    [SerializeField] private int count;
    [SerializeField] private List<Color> Colors;
    [SerializeField] private float _maxTimer = 200f;

    private Dictionary<Vector3, Color> brickRespawnList = new Dictionary<Vector3, Color>();
    private float _timer;

    void Start()
    {
        SpawnBrick();
    }

    void Update()
    {
        //if (_timer > _maxTimer)
        //{
        //    SpawnBrick();
        //    _timer = 0;
        //}
        //_timer += Time.deltaTime;
    }

    // tạo gạch trong thời gian quy định
    public void SpawnBrick()
    {
        float spacing = 1.5f; // Khoảng cách giữa các viên gạch
        int rowCount = Mathf.CeilToInt(Mathf.Sqrt(count)); // Số hàng/cột tối thiểu để chứa đủ số gạch

        // Tạo danh sách vị trí cố định
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < rowCount; j++)
            {
                if (positions.Count >= count) break;

                Vector3 position = new Vector3(
                    CenterPoint.position.x + (i - rowCount / 2) * spacing,
                    CenterPoint.position.y,
                    CenterPoint.position.z + (j - rowCount / 2) * spacing
                ) + Vector3.up;

                positions.Add(position);
            }
        }

        // Trộn danh sách vị trí ngẫu nhiên
        for (int i = 0; i < positions.Count; i++)
        {
            int randomIndex = Random.Range(i, positions.Count);
            (positions[i], positions[randomIndex]) = (positions[randomIndex], positions[i]);
        }

        // Tạo danh sách màu, đảm bảo số lượng màu bằng nhau
        List<Color> colorPool = new List<Color>();
        int colorsPerBrick = count / Colors.Count;
        int remainder = count % Colors.Count;

        for (int i = 0; i < Colors.Count; i++)
        {
            for (int j = 0; j < colorsPerBrick; j++)
            {
                colorPool.Add(Colors[i]);
            }
        }

        // Thêm các màu dư vào danh sách
        for (int i = 0; i < remainder; i++)
        {
            colorPool.Add(Colors[i]);
        }

        // Trộn danh sách màu
        for (int i = 0; i < colorPool.Count; i++)
        {
            int randomIndex = Random.Range(i, colorPool.Count);
            (colorPool[i], colorPool[randomIndex]) = (colorPool[randomIndex], colorPool[i]);
        }

        // Tạo gạch với vị trí và màu ngẫu nhiên
        for (int i = 0; i < count; i++)
        {
            Vector3 position = positions[i];
            var brick = Instantiate(BrickPrefab, position, Quaternion.identity);

            // Lấy màu từ danh sách màu đã trộn
            var color = colorPool[i];
            brick.GetComponent<Brick>().ChangeColor(color);
            //Destroy(brick, 10f);
        }
    }

    public void RegenerateBricks()
    {
        Debug.Log("Regenerating bricks...");
        SpawnBrick();
    }

    // ✅ Lưu vị trí viên gạch bị lấy đi và bắt đầu đếm thời gian hồi sinh
    public void MarkForRespawn(Vector3 position, Color color)
    {
        if (!brickRespawnList.ContainsKey(position))
        {
            brickRespawnList[position] = color;
            StartCoroutine(RespawnBrick(position, color));
        }
    }

    // 🔄 Hồi sinh viên gạch sau 5s
    private IEnumerator RespawnBrick(Vector3 position, Color color)
    {
        yield return new WaitForSeconds(5f); // Chờ 5 giây

        if (brickRespawnList.ContainsKey(position))
        {
            var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
            brick.GetComponent<Brick>().ChangeColor(color);
            brickRespawnList.Remove(position); // Xóa khỏi danh sách
        }
    }
}
