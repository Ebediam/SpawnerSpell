using BS;

namespace SpawnerSpell
{
    // This create an item module that can be referenced in the item JSON
    public class ItemModuleSpawnerSpell : ItemModule
    {
        

        public string leftItemID = "Sword1";
        public string rightItemID = "Sword1";

        public string leftThrowItemID = "Sword1";
        public string rightThrowItemID = "Sword1";

        public float throwSpeed = 20f;
        public float timeToDespawnThrownItem = 5f;
        public bool canBind = false;
        public bool canSummon = true;
        public bool canThrowSummon = true;


        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<ItemSpawnerSpell>();
        }
    }
}
