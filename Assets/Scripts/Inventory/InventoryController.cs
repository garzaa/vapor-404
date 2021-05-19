﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour {
    public InventoryList items;   
    public InventoryUI inventoryUI;
    public MerchantUI merchantUI;
    bool inInventory = false;

    public Item moneyItem;
    public Merchant currentMerchant;
    public Text moneyUI;
    public AudioSource itemBuy;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        LoadFromSaveData(GlobalController.save);
    }

    void LoadFromSaveData(Save save) {
        // populate items from the save file, wherever that setting is
        // then add save.startingitems
    }


    public void Start() {
        // TODO: reconcile this with the full game
        // if it was loaded from another scene, then don't add the editor items
        // but if we're starting in this scene then it's OK
        if (!GlobalController.save.loadedOnce) {
        List<Item> startingItems = GlobalController.save.startingItems;
            if (startingItems != null) {
                foreach (Item item in startingItems) {
                    if (item != null) GlobalController.AddItem(new StoredItem(item), quiet: true);
                }
            }
        }
        /*
        if (startingItems == null || items.IsEmpty()) return;
        if (startingItems != null) {
            foreach (Item item in startingItems) {
                // get around me being lazy in the editor
                if (item != null) GlobalController.AddItem(new StoredItem(item), quiet:true);
            }
        }
        */
        UpdateMoneyUI();
    }

    public void Clear() {
        items.Clear();
    }

    public void ReactToItemSelect(StoredItem item) {
        if (this.currentMerchant == null)  {
            return;
        }
        TryToBuy(item);
    }

    public void AddItem(StoredItem s, bool quiet) {
		items.AddItem(s);
        if (inInventory) inventoryUI.PopulateItems(this.items);
		s.item.OnPickup(quiet);
        UpdateMoneyUI();
    }

    public void ShowInventory() {
        if (currentMerchant != null) {
            inventoryUI.PopulateItems(currentMerchant.baseInventory);
            inventoryUI.PropagateMerchantInfo(currentMerchant);
            inventoryUI.animator.SetBool("Merchant", true);
        } else {
            inventoryUI.animator.SetBool("Merchant", false);
            inventoryUI.PopulateItems(this.items);
        }
        inventoryUI.Show();
    }

    public void HideInventory() {
        SoundManager.InteractSound();
        currentMerchant = null;
        inventoryUI.animator.SetBool("Merchant", false);
        inventoryUI.Hide();
    }

    public int CheckMoney() {
        if (items.GetItem(moneyItem) != null) {
            return items.GetItem(moneyItem).count;
        }
        return 0;
    }

    public void UpdateMoneyUI() {
        moneyUI.text = "$ " + CheckMoney().ToString();
    }

    public void TakeMoney(int amount) {
        items.GetItem(moneyItem).count -= amount;
    }

    void TryToBuy(StoredItem s) {
        InventoryList merchantInventory = currentMerchant.baseInventory;
        Item item = s.item;
        bool hasMoney = item.cost <= GlobalController.inventory.CheckMoney();
        if (hasMoney) {
            // copy
            StoredItem toAdd = new StoredItem(merchantInventory.GetItem(item).item);
            TakeMoney(item.cost);
            if (merchantInventory.GetItem(item).item.stackable) {
                if (merchantInventory.GetItem(s).count > 1) {
                    merchantInventory.GetItem(s).count -= 1;
                } else {
                    merchantInventory.RemoveItem(s);
                }
            } else {
                merchantInventory.RemoveItem(s);
            }
            GlobalController.AddItem(toAdd, false);
            inventoryUI.merchantLine.text = currentMerchant.GetThanksDialogue(item);
            itemBuy.PlayOneShot(itemBuy.clip);
            UpdateMoneyUI();
            currentMerchant.ReactToBuy();
        } else {
            inventoryUI.merchantLine.text = currentMerchant.notEnoughMoneyDialogue;
        }
        inventoryUI.PopulateItems(currentMerchant.baseInventory);
    }
}
