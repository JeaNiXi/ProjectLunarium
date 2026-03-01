using Effect;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxBackgroundEffect : MonoBehaviour
{
    public List<ParalaxBackgroundSO> ParalaxBackgroundList;
    public int CurrentBackgroud;

    [Header("Настройки движения")]
    [Tooltip("Скорость (0.1 - далеко, 2.0 - близко)")]
    public float moveSpeed = 0.5f;

    private float _spriteWidth;
    private Vector3 _startPos;

    void Awake()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        _startPos = transform.position;

        // 1. АВТО-МАСШТАБИРОВАНИЕ родителя под экран
        Camera cam = Camera.main;
        float screenHeight = cam.orthographicSize * 2.0f;

        float spriteHeight = sr.sprite.bounds.size.y;
        float spriteRawWidth = sr.sprite.bounds.size.x;

        float scaleValue = screenHeight / spriteHeight;
        transform.localScale = new Vector3(scaleValue, scaleValue, 1);

        // Финальная ширина в юнитах мира для зацикливания
        _spriteWidth = spriteRawWidth * scaleValue;

        // 2. АВТОМАТИКА: Создаем дубликат справа
        GameObject childCopy = new GameObject(gameObject.name + "_Copy");
        childCopy.transform.SetParent(transform);

        // СБРОС МАСШТАБА: чтобы он не умножался на масштаб родителя
        childCopy.transform.localScale = Vector3.one;

        SpriteRenderer srCopy = childCopy.AddComponent<SpriteRenderer>();
        srCopy.sprite = sr.sprite;
        srCopy.sortingOrder = sr.sortingOrder;
        srCopy.material = sr.material;

        // СТЫКОВКА: Ставим ровно на ширину исходного спрайта (local X)
        childCopy.transform.localPosition = new Vector3(spriteRawWidth, 0, 0);
    }

    void Update()
    {
        // 3. Двигаем слой влево
        // Умножаем на TimeManager.Instance.GameSpeed, если хочешь связь со временем
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

        // 4. ЗАЦИКЛИВАНИЕ: Если проехали всю ширину спрайта — прыгаем назад
        if (transform.position.x <= _startPos.x - _spriteWidth)
        {
            transform.position = new Vector3(_startPos.x, transform.position.y, transform.position.z);
        }
    }
}
