using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Net.Configuration;
using System.Runtime.InteropServices;
using BS;
using UnityEngine;

namespace SpawnerSpell
{

    public class ItemSpawnerSpell : MonoBehaviour
    {

        //Spell Framework
        protected Item item;
        public ItemModuleSpawnerSpell module;
        public Interactor interactor;
        public bool isCasting;
        public bool isAltCasting;
        public string spellName;
        public SpellSelector spellSelect;
        public bool listenForSpellSelector;
        public static ItemSpawnerSpell leftSpell;
        public static ItemSpawnerSpell rightSpell;


        public AudioSource spellSFX;
        public AudioSource bindSFX;
        public ParticleSystem spellVFX;
        public ParticleSystem bindVFX;
        public ItemData summonData;
        public ItemData throwSummonData;


        protected void Start()
        {
            Debug.Log("----------SPAWNER SPELL ACTIVE--------------");
            item = gameObject.GetComponent<Item>();
            if (!item)
            {
                Debug.LogError("No item found");
            }

            module = item.data.GetModule<ItemModuleSpawnerSpell>();

            if (module == null)
            {
                Debug.LogError("No module found");
            }

            interactor = item.GetComponentInParent<Interactor>();
            spellName = interactor.bodyHand.caster.currentSpell.name;

            item.disallowDespawn = true;

            switch (interactor.side)
            {
                case Side.Left:

                    spellSelect = SpellSelector.left;

                    if (leftSpell)
                    {
                        leftSpell.Despawn();
                    }

                    leftSpell = this;

                    break;

                case Side.Right:
                    spellSelect = SpellSelector.right;

                    if (rightSpell)
                    {
                        rightSpell.Despawn();
                    }
                    rightSpell = this;

                    break;
            }

            if (module.canSummon)
            {
                spellSelect.enabled = false;
            }

            Align();
            Initialize();

        }

        public void Align()
        {          

            transform.LookAt(interactor.bodyHand.middleProximal);
        }


        void Update()
        {
            CheckSpell();   
            CheckInput(); 

        }

        public void CheckSpell()
        {
            if (interactor.bodyHand.caster.currentSpell) 
            {
                if(spellName != interactor.bodyHand.caster.currentSpell.name)
                {
                    Despawn();
                }
            }
            else
            {
                Despawn();
            }
        }

        public void CheckInput()
        {
            if (Pointer.GetPointer(interactor.side).isPointingUI)
            {
                return;
            }

            if (isCasting)
            {
                if (!PlayerControl.GetHand(interactor.side).usePressed)
                {
                    isCasting = false;
                    OnCastStop();
                    
                }
            }
            else
            {
                if (PlayerControl.GetHand(interactor.side).usePressed)
                {
                    isCasting = true;
                    OnCast();
                    DisableSpellSelect();

                }
            }            

            if (isAltCasting)
            {
                if (!PlayerControl.GetHand(interactor.side).alternateUsePressed)
                {
                    isAltCasting = false;
                    OnAltCastStop();
                }
            }
            else
            {
                if (PlayerControl.GetHand(interactor.side).alternateUsePressed)
                {
                    isAltCasting = true;                   

                    if (spellSelect.enabled)
                    {
                        if (module.canSummon)
                        {
                            CancelInvoke("DisableSpellSelect");
                            Despawn();
                        }
                    }
                    else
                    {
                        spellSelect.enabled = true;
                        Invoke("DisableSpellSelect", 0.3f);
                        
                    }

                    OnAltCast();

                }
            }
        }

        public void DisableSpellSelect()
        {
            if (module.canSummon)
            {
                spellSelect.enabled = false;
            }
            
        }
        public void Despawn()
        {
            spellSelect.enabled = true;
            item.disallowDespawn = false;
            item.Despawn();
        }

        public void Initialize()
        {
            spellSFX = transform.Find("SpellSFX").GetComponent<AudioSource>();
            bindSFX = transform.Find("BindSFX").GetComponent<AudioSource>();

            spellVFX = transform.Find("SpellVFX").GetComponent<ParticleSystem>();
            bindVFX = transform.Find("BindVFX").GetComponent<ParticleSystem>();

            switch (interactor.side)
            {
                case Side.Left:
                    summonData = Catalog.current.GetData<ItemData>(module.leftItemID, true);
                    throwSummonData = Catalog.current.GetData<ItemData>(module.leftThrowItemID, true);
                    break;

                default:
                    summonData = Catalog.current.GetData<ItemData>(module.rightItemID, true);
                    throwSummonData = Catalog.current.GetData<ItemData>(module.rightThrowItemID, true);
                    break;

            }

            if (summonData == null)
            {
                Debug.LogError("summonData is null");
            }

            Debug.Log("SummonData loaded: "+summonData.displayName);

            if(throwSummonData == null)
            {
                Debug.LogError("ThrowData is null");
            }

            Debug.Log("ThrowSummonData loaded: " + throwSummonData.displayName);

        }

        public void OnCastStop()
        {

        }

        public void OnCast()
        {
            if (interactor.grabbedHandle)
            {
                if (isAltCasting)
                {
                    if (!module.canBind)
                    {
                        return;
                    }

                    if (!module.canSummon)
                    {
                        return;
                    }

                    summonData = interactor.grabbedHandle.item.data;

                    if (bindSFX)
                    {
                        bindSFX.Play();
                    }

                    if (bindVFX)
                    {
                        bindVFX.Play();
                    }
                   
                    
                }

                return;

            }

            if (!module.canThrowSummon)
            {
                return;
            }

            if (spellSFX)
            {
                spellSFX.Play();

            }

            if (spellVFX)
            {
                spellVFX.Play();
            }

            SummonAndThrow();
        }

        public void OnAltCast()
        {
            if (interactor.grabbedHandle)
            {
                if (isCasting)
                {
                    if (!module.canBind)
                    {
                        return;
                    }

                    if (!module.canThrowSummon)
                    {
                        return;
                    }

                    throwSummonData = interactor.grabbedHandle.item.data;

                    if (bindSFX)
                    {
                        bindSFX.Play();
                    }

                    if (bindVFX)
                    {
                        bindVFX.Play();
                    }
                    
                }
                return;

            }

            if (isCasting)
            {
                Despawn();
            }
            else
            {

                if (!module.canSummon)
                {
                    return;
                }

                if (spellSFX)
                {
                    spellSFX.Play();
                }

                if (spellVFX)
                {
                    spellVFX.Play();
                }

                SummonAndGrab();
            }
        }
        
        public void OnAltCastStop()
        {

            
        }

        public void EnableSpellSelect()
        {
            spellSelect.enabled = true;     
        }




        public void SummonAndGrab()
        {

            Item item = summonData.Spawn(false);

            item.gameObject.SetActive(true);

            interactor.Grab(item.mainHandleRight, true);


        }

        public void SummonAndThrow()
        {
            Item item = throwSummonData.Spawn(false);

            item.gameObject.SetActive(true);

            item.IgnoreRagdollCollision(Creature.player.ragdoll);

            item.IgnoreObjectCollision(interactor.playerHand.itemHand.item);

            item.transform.rotation = transform.rotation;
            item.transform.position = transform.position;

            item.Throw(1, Item.FlyDetection.Forced);
            item.rb.AddForce(item.transform.forward * module.throwSpeed, ForceMode.VelocityChange);
            
            if(module.timeToDespawnThrownItem != 0)
            {
                //item.Despawn(module.timeToDespawnThrownItem);
                StartCoroutine(Deactivate(item, module.timeToDespawnThrownItem));
                
                
            }


        }

        IEnumerator Deactivate(Item item, float time)
        {

            yield return new WaitForSeconds(time);

            if (item)
            {
                item.gameObject.SetActive(false);
            }
           
        }


        



    }
}