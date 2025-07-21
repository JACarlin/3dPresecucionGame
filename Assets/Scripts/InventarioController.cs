using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class InventarioController : MonoBehaviour
{
    public GameObject inventoryPanel;
    public Image selectionCircle;
    public Button[] inventorySlots;
    public Transform handHolder;
    private bool isInventoryOpen = false;
    private Button selectedButton;
    private GameObject selectedItem;
    private Dictionary<Button, GameObject> slotItems = new Dictionary<Button, GameObject>();

    void Start()
    {
        inventoryPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UpdateInventoryUI();
        selectedButton = inventorySlots[0];
    }

    void Update()
    {
        if (Time.timeScale == 0)
            return;
        if (Input.GetKeyDown(KeyCode.Tab)) ToggleInventory(true);
        else if (Input.GetKeyUp(KeyCode.Tab)) SelectAndCloseInventory();

        if (isInventoryOpen) HighlightClosestItem();
        if (Input.GetKeyDown(KeyCode.E)) PickUpItem();
        if (Input.GetKeyDown(KeyCode.Q)) DropSelectedItem();
    }
    private void OnEnable()
    {
        LanguageManager.OnLanguageChanged += UpdateInventoryUI;
    }

    private void OnDisable()
    {
        LanguageManager.OnLanguageChanged -= UpdateInventoryUI;
    }


    void ToggleInventory(bool open)
    {
        isInventoryOpen = open;
        FirstPersonMovement.inventoryOpen = open;
        inventoryPanel.SetActive(open);
        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = open;
        if (!open) selectionCircle.rectTransform.anchoredPosition = Vector2.zero;
    }

    void HighlightClosestItem()
    {
        Vector2 mousePos = Input.mousePosition;
        selectedButton = inventorySlots.OrderBy(slot => Vector2.Distance(slot.GetComponent<RectTransform>().position, mousePos)).FirstOrDefault();
        foreach (var slot in inventorySlots)
        {
            slot.GetComponent<Image>().color = (slot == selectedButton) ? Color.black : new Color(1, 1, 1, 0);
            if (slot == selectedButton) slot.transform.SetAsLastSibling();
        }
    }

    void SelectAndCloseInventory()
    {
        if (selectedButton != null)
        {
            Debug.Log("Objeto seleccionado: " + selectedButton.name);
            EquipItem(selectedButton); // Llama a EquipItem cuando se seleccione el item
        }
        ToggleInventory(false);
    }

    void PickUpItem()
    {
        // Verifica que exista una cámara principal.
        if (Camera.main == null)
        {
            Debug.LogWarning("No se encontró Camera.main");
            return;
        }

        // Crear un raycast desde el centro de la pantalla.
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        // Distancia máxima para recoger el item.
        float rayDistance = 3f;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            if (hit.collider.CompareTag("Item"))
            {
                selectedItem = hit.collider.gameObject;
                AssignItemToInventory();
            }
        }
    }


    void AssignItemToInventory()
    {
        if (selectedItem == null) return;
        ItemData itemData = selectedItem.GetComponent<ItemPickup>()?.itemData;
        if (itemData == null) return;



        Button emptySlot = inventorySlots.Skip(1).FirstOrDefault(slot => !slotItems.ContainsKey(slot));
        if (emptySlot != null)
        {
            
            slotItems[emptySlot] = selectedItem;
            Debug.Log("1eliminando" + selectedItem.name);
            selectedItem.SetActive(false);
            Debug.Log($"Item {selectedItem.name} agregado a {emptySlot.name}");
            selectedButton = emptySlot;
            EquipItem(emptySlot);
        }
        else if (selectedButton != null && selectedButton != inventorySlots[0])
        {
            GameObject item = selectedItem;
            DropItem(selectedButton);
            
            slotItems[selectedButton] = item;

            Debug.Log("2eliminando" + item.name);
            item.SetActive(false);
            Debug.Log($"Item {item.name} reemplazó en {selectedButton.name}");
            EquipItem(selectedButton);
        }
        else if (selectedButton != null && selectedButton == inventorySlots[0])
        {
            selectedItem = null;
        }
        UpdateInventoryUI();
    }

    void DropSelectedItem()
    {
        if (selectedButton == null || selectedButton == inventorySlots[0]) return;
        DropItem(selectedButton);
    }

    void DropItem(Button slot)
    {
        if (!slotItems.ContainsKey(slot))
        {
            Debug.LogError("Slot vacío.");
            return;
        }

        GameObject droppedItem = slotItems[slot];
        if (droppedItem == null)
        {
            Debug.LogError("Ítem null.");
            return;
        }

        // Mover al mundo 3D
        Transform itemsParent = GameObject.Find("items")?.transform;
        if (itemsParent == null)
        {
            Debug.LogError("No existe 'Items' en la escena.");
            return;
        }

        droppedItem.transform.SetParent(itemsParent, true); // Importante moverlo del Canvas

        // Activar si estaba oculto
        droppedItem.SetActive(true);

        // Activar física si tiene
        Rigidbody rb = droppedItem.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = false;

        Collider col = droppedItem.GetComponent<Collider>();
        if (col != null)
            col.enabled = true;

        // Calcular la posición de soltado usando un raycast para evitar colisiones con obstáculos.
        float dropDistance = 1.5f;
        Vector3 origin = Camera.main.transform.position;
        Vector3 direction = Camera.main.transform.forward;
        Vector3 dropPosition;

        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, dropDistance))
        {
            // Si se detecta un obstáculo, se posiciona el item justo antes de éste.
            dropPosition = hit.point - direction * 0.1f;
        }
        else
        {
            dropPosition = origin + direction * dropDistance;
        }

        // Añadir el offset vertical deseado.
        dropPosition += Vector3.up * 1.0f;

        droppedItem.transform.position = dropPosition;
        droppedItem.transform.rotation = Quaternion.identity;
        droppedItem.transform.localScale = Vector3.one;

        slotItems.Remove(slot);
        selectedItem = null;
        Debug.Log($"Droppeado {droppedItem.name}");

        if (slot == inventorySlots[0])
        {
            ClearHand();
        }

        UpdateInventoryUI();
    }




    void UpdateInventoryUI()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            Button slot = inventorySlots[i];
            TextMeshProUGUI slotText = slot.GetComponentInChildren<TextMeshProUGUI>();
            Image slotChildImage = slot.GetComponentsInChildren<Image>().FirstOrDefault(img => img.gameObject != slot.gameObject);

            if (slotText != null)
                slotText.text = slotItems.ContainsKey(slot) ? slotItems[slot].GetComponent<ItemPickup>().itemData.LocalizedName : "";

            if (slotChildImage != null)
            {
                if (i == 0) slotChildImage.enabled = true;
                else
                {
                    slotChildImage.sprite = slotItems.ContainsKey(slot) ? slotItems[slot].GetComponent<ItemPickup>().itemData.itemIcon : null;
                    slotChildImage.enabled = slotChildImage.sprite != null;
                }
            }
        }
    }

    void EquipItem(Button slot)
    {
        // Si el slot seleccionado es el 1 (la mano), limpiamos la mano
        foreach (Transform child in handHolder) child.gameObject.SetActive(false);
        if (slot == inventorySlots[0])
        {
            ClearHand();
            return;
        }

        // Si el slot no tiene un item, no hacemos nada
        if (!slotItems.ContainsKey(slot)) return;

        GameObject item = slotItems[slot];

        // Establecemos el nuevo item como el item seleccionado
        selectedItem = item;
        selectedItem.SetActive(true);  // Activamos el nuevo item

        // Lo movemos a la posición de handHolder
        item.transform.SetParent(handHolder, false);  // Es hijo de handHolder
        item.transform.localPosition = Vector3.zero;  // Mantiene la misma posición
        item.transform.localRotation = Quaternion.identity;  // Sin rotación extra
        item.transform.localScale = Vector3.one;  // Escala original

        // Desactivamos la física (Rigidbody) y la colisión (Collider) del item
        Rigidbody rb = item.GetComponent<Rigidbody>();
        Collider col = item.GetComponent<Collider>();

        if (rb != null) rb.isKinematic = true;  // Desactivamos la física
        if (col != null) col.enabled = false;   // Desactivamos la colisión

        Debug.Log($"Item {item.name} equipado en la mano");
    }

    void ClearHand()
    {
        // Si la mano está vacía (slot 0), limpiamos el item
        if (selectedItem != null)
        {
            Debug.Log("4eliminando" + selectedItem.name);
            selectedItem.SetActive(false);  // Desactivamos el item
            selectedItem.transform.SetParent(null);  // Lo sacamos de la mano
            selectedItem = null;  // Limpiamos la referencia del item seleccionado
            Debug.Log("Mano vacía");
        }
    }
    public ItemData GetSelectedItemData()
    {
        if (selectedItem != null)
            return selectedItem.GetComponent<ItemPickup>().itemData;
        return null;
    }
 // Asegúrate de incluir este using para usar LINQ

public void RemoveSelectedItem()
{
    if (selectedItem != null)
    {
        // Buscar el slot donde está el ítem seleccionado.
        Button keySlot = slotItems.FirstOrDefault(x => x.Value == selectedItem).Key;
        if (keySlot != null)
            slotItems.Remove(keySlot);
        // Destruir el objeto o, si prefieres, inactivarlo permanentemente.
        Destroy(selectedItem);
        selectedItem = null;
        UpdateInventoryUI();
    }
}



}