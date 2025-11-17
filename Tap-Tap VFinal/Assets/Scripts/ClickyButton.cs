using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickyButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image img;
    [SerializeField] private Sprite defaultSprite, pressed, hover;
    [SerializeField] private AudioClip compressClip, uncompressClip, hoverClip;
    [SerializeField] private AudioSource source;

    private bool isPressed = false; // Para saber si el bot�n est� presionado

    public void OnPointerDown(PointerEventData eventData)
    {
        img.sprite = pressed;
        source.PlayOneShot(compressClip);
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        img.sprite = hover; // Cambia a 'hover' si el puntero sigue encima
        source.PlayOneShot(uncompressClip);
        isPressed = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isPressed) // Cambia a 'hover' solo si no est� presionado
        {
            img.sprite = hover;
            source.PlayOneShot(hoverClip);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isPressed) // Cambia a 'defaultSprite' solo si no est� presionado
        {
            img.sprite = defaultSprite;
        }
    }
}
