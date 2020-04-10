using UnityEngine;
using BS;

namespace SpawnerSpell
{
    // This create an level module that can be referenced in the level JSON
    public class SpawnerSpellModule : LevelModule
    {
        public AudioSource leftSFX;
        public AudioSource rightSFX;

        public string leftItemID = "Sword1";
        public string rightItemID = "Sword1";
        string spellPrefabName = "SpawnerSpell";

        

        public ItemData leftItemData;
        public ItemData rightItemData;
        Item leftItem;
        Item rightItem;

        public override void OnLevelLoaded(LevelDefinition levelDefinition)
        {
            initialized = true;
            leftItemData = Catalog.current.GetData<ItemData>(leftItemID, true);
            rightItemData = Catalog.current.GetData<ItemData>(rightItemID, true);
        }


        public override void Update(LevelDefinition levelDefinition)
        {
            if(Player.local != null)
            {
                if(Player.local.body != null)
                {
                    if (Player.local.body.handRight)
                    {
                        if (CheckForSpell(Player.local.body.handRight))
                        {
                            if (PlayerControl.handRight.castPressed)
                            {
                                SpawnAndGrab(rightItemData, Player.local.body.handRight);
                            }
                        }
                        else
                        {
                            rightSFX = null;
                        }
                    }

                    if (Player.local.body.handLeft)
                    {

                        if (CheckForSpell(Player.local.body.handLeft))
                        {
                            if (PlayerControl.handLeft.castPressed)
                            {
                                SpawnAndGrab(leftItemData, Player.local.body.handLeft);
                            }
                        }
                        else
                        {
                            leftSFX = null;
                        }

                    }
                }
            }         
        



        }


        bool CheckForSpell(BodyHand hand)
        {

            if (hand.caster.currentSpell != null)
            {
                if (hand.caster.currentSpell.name == spellPrefabName + "(Clone)")
                {
                    switch (hand.side)
                    {
                        case Side.Left:
                            if (!leftSFX)
                            {
                                leftSFX = hand.caster.currentSpell.gameObject.GetComponentInChildren<AudioSource>();
                            }
                            break;

                        case Side.Right:
                            if (!rightSFX)
                            {
                                rightSFX = hand.caster.currentSpell.gameObject.GetComponentInChildren<AudioSource>();
                            }
                            break;

                    }


                    return true;
                }
            }
            return false;

        }

        public void SpawnAndGrab(ItemData _itemData, BodyHand hand)
        {
            if (hand.interactor.grabbedHandle)
            {               
                if (hand.interactor.grabbedHandle.item)
                {
                    Item _item = hand.interactor.grabbedHandle.item;

                    if (PlayerControl.GetHand(hand.side).alternateUsePressed)
                    {                        
                        switch (hand.side)
                        {
                            case Side.Left:
                                leftItemData = _item.data;
                                if (leftSFX)
                                {

                                    leftSFX.Play();
                                }
                                break;

                            case Side.Right:
                                rightItemData = _item.data;
                                if (rightSFX)
                                {

                                    rightSFX.Play();
                                }
                                break;
                        }

                        
                    }
                    
                }

            }
            else
            {
                if (hand.side == Side.Left)
                {
                    leftItem = _itemData.Spawn(true);
                    leftItem.gameObject.SetActive(true);
                    hand.interactor.Grab(leftItem.mainHandleRight, true);
                    if (leftSFX)
                    {
                        leftSFX.Play();
                    }



                }
                else if (hand.side == Side.Right)
                {
                    rightItem = _itemData.Spawn(true);
                    rightItem.gameObject.SetActive(true);
                    hand.interactor.Grab(rightItem.mainHandleRight, true);
                    if (rightSFX)
                    {
                        rightSFX.Play();
                    }

                }
            }


        }

    }
}
